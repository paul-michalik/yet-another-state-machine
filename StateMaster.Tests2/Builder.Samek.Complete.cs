using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StateMaster.Tests2 {
    /// <summary>
    /// Summary description for Builder
    /// </summary>
    [TestClass]
    public class Builder_Samek_Complete {

        public class SamekContext {

            public AbstractStates.StateMachine Controller { get; set; }

            public SamekContext()
            {
                Log = new List<string>();
            }

            public List<String> Log { get; private set; }

            public void OnTransition(AbstractStates.State pSource, AbstractStates.State pTarget)
            {
                Log.Add(String.Format("Transition:{0}->{1};", 
                   (States.Samek)pSource.ID, (States.Samek)pTarget.ID));
            }

            public void OnEnter(AbstractStates.State pState)
            {
                Log.Add(String.Format("{0}-ENTER;",
                   (States.Samek)pState.ID));
            }

            public void OnExit(AbstractStates.State pState)
            {
                Log.Add(String.Format("{0}-EXIT;",
                   (States.Samek)pState.ID));
            }

            public void OnUnhandledEvent(Event p_Event)
            {
                Log.Add(String.Format("Event {0}", p_Event.ID));
            }

            public Int32 Foo { get; set; }
        }

        AbstractStates.StateMachine m_Machine;
        SamekContext m_Context;

        IDictionary<States.Samek, AbstractStates.State> AllStates
        {
            get
            {
                return m_Machine.ToDictionary(_ => (States.Samek)_.ID);
            }
        }

        static AbstractStates.StateMachine Create(SamekContext p_Context)
        {
            var tB = new Builder();
            var Samek = tB.StateMachine().ID(States.Samek.Samek);
            var Samek_Init = tB.Initial(Samek).ID(States.Samek.Samek_Init);
            var S = tB.Composite(Samek).ID(States.Samek.S);
            var S_Init = tB.Initial(S).ID(States.Samek.S_Init);
            var S1 = tB.Composite(S).ID(States.Samek.S1);
            var S1_Init = tB.Initial(S1).ID(States.Samek.S1_Init);
            var S11 = tB.Simple(S1).ID(States.Samek.S11);
            var S2 = tB.Composite(S).ID(States.Samek.S2);
            var S2_Init = tB.Initial(S2).ID(States.Samek.S2_Init);
            var S21 = tB.Composite(S2).ID(States.Samek.S21);
            var S21_Init = tB.Initial(S21).ID(States.Samek.S21_Init);
            var S211 = tB.Simple(S21).ID(States.Samek.S211);
            var Samek_Term = tB.Terminal(Samek).ID(States.Samek.Samek_Term);

            // Initial transitions
            Samek_Init
                .AddTransition(S2)
                .Action(_1 => p_Context.Foo = 0);
            S_Init
                .AddTransition(S11);
            S1_Init
                .AddTransition(S11);
            S2_Init
                .AddTransition(S211);
            S21_Init
                .AddTransition(S211);

            // States
            S.AddTransition(Events.Samek.I)
                    .Guard(_1 => p_Context.Foo == 1)
                    .Action(_1 => p_Context.Foo = 0)
                .AddTransition(Events.Samek.E, S11)
                .AddTransition(Events.Samek.TERMINATE, Samek_Term);
            // S1
            S1.AddTransition(Events.Samek.A)
                    .Kind(TransitionKind.External)
                .AddTransition(Events.Samek.B, S11)
                .AddTransition(Events.Samek.C, S2)
                .AddTransition(Events.Samek.D, S)
                    .Guard(_1 => p_Context.Foo == 0)
                    .Action(_1 => p_Context.Foo = 1)
                .AddTransition(Events.Samek.I);
            // S11
            S11.AddTransition(Events.Samek.D, S1)
                    .Guard(_1 => p_Context.Foo == 1)
                    .Action(_1 => p_Context.Foo = 0)
                .AddTransition(Events.Samek.H, S)
                .AddTransition(Events.Samek.G, S211);
            // S2
            S2.AddTransition(Events.Samek.I)
                    .Guard(_1 => p_Context.Foo == 0)
                    .Action(_1 => p_Context.Foo = 1)
                .AddTransition(Events.Samek.C, S1)
                .AddTransition(Events.Samek.F, S11);
            // S21
            S21.AddTransition(Events.Samek.G, S1)
                .AddTransition(Events.Samek.A)
                    .Kind(TransitionKind.External)
                .AddTransition(Events.Samek.B, S211);
            // S211
            S211.AddTransition(Events.Samek.H, S)
                .AddTransition(Events.Samek.D, S21);

            foreach (var tS in Samek.Buildee) {
                var tTmpS1 = tS;
                tTmpS1.Enter += () => p_Context.OnEnter(tTmpS1);
                tTmpS1.Exit += () => p_Context.OnExit(tTmpS1);
                if (tTmpS1.Transitions != null) {
                    tTmpS1.Transitions
                        .Select(_ => _.Value)
                        .ToList()
                        .ForEach(_1 => _1.Action += (_2) => p_Context.OnTransition(tTmpS1, _1.Target));
                }
            }

            return Samek;
        }

        public Builder_Samek_Complete()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void SetUp_SUT_Samek()
        {
            m_Context = new SamekContext();

            // Setup:
            m_Machine = Create(m_Context);
            m_Context.Controller = m_Machine;
        }

        //
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void TearDown_SUT_Samek()
        {
            m_Context = null;
            m_Machine = null;
        }

        #endregion

        [TestMethod]
        public void Samek_State_Tree_Is_As_Expected()
        {
            Assert.AreEqual(13, m_Machine.ToArray().Length);

            CollectionAssert.AreEqual(new States.Samek[] {
                States.Samek.Samek,
                States.Samek.Samek_Init, 
                States.Samek.S, 
                States.Samek.S_Init, 
                States.Samek.S1, 
                States.Samek.S1_Init, 
                States.Samek.S11, 
                States.Samek.S2, 
                States.Samek.S2_Init, 
                States.Samek.S21, 
                States.Samek.S21_Init, 
                States.Samek.S211, 
                States.Samek.Samek_Term
            }, m_Machine.Select(_1 => (States.Samek)_1.ID).ToArray());

            var tS = this.AllStates;
            Assert.AreEqual(tS[States.Samek.Samek_Init].Parent, tS[States.Samek.Samek]);
            Assert.AreEqual(tS[States.Samek.S].Parent, tS[States.Samek.Samek]);
            Assert.AreEqual(tS[States.Samek.Samek_Term].Parent, tS[States.Samek.Samek]);

            Assert.AreEqual(tS[States.Samek.S_Init].Parent, tS[States.Samek.S]);
            Assert.AreEqual(tS[States.Samek.S1].Parent, tS[States.Samek.S]);
            Assert.AreEqual(tS[States.Samek.S2].Parent, tS[States.Samek.S]);

            Assert.AreEqual(tS[States.Samek.S1_Init].Parent, tS[States.Samek.S1]);
            Assert.AreEqual(tS[States.Samek.S11].Parent, tS[States.Samek.S1]);

            Assert.AreEqual(tS[States.Samek.S2_Init].Parent, tS[States.Samek.S2]);
            Assert.AreEqual(tS[States.Samek.S21].Parent, tS[States.Samek.S2]);

            Assert.AreEqual(tS[States.Samek.S21_Init].Parent, tS[States.Samek.S21]);
            Assert.AreEqual(tS[States.Samek.S211].Parent, tS[States.Samek.S21]);

            // transitions
            { // Samek_Init
                Assert.IsNotNull(tS[States.Samek.S_Init].Transitions);
                var tT = tS[States.Samek.Samek_Init].Transitions
                    .Select(_ => _.Value.Target)
                    .ToArray();
                CollectionAssert.AreEqual(new AbstractStates.State [] {
                    tS[States.Samek.S2]
                }, tT);
            }
        }

        [TestMethod]
        public void LCA_For_Disable_Event_Handling()
        {
            Assert.AreEqual(m_Machine, m_Context.Controller);
            m_Context.Log.Clear();
            Assert.AreEqual(false, m_Machine.IsBusy);
            Assert.AreEqual(false, m_Machine.IsEnabled);
            Assert.AreEqual(0, m_Context.Log.Count);

            var tAllStates = this.AllStates;
            var tInternalTerminal = m_Machine.Children.FirstOrDefault(_ => _ is InternalStates.Terminal);
            Assert.IsNotNull(tInternalTerminal);
            
            var tLCAResult = Core.LCASearch.Execute(tAllStates[States.Samek.S211], tInternalTerminal);
            Assert.AreEqual(m_Machine, tLCAResult.LCA);
            CollectionAssert.AreEqual(new AbstractStates.State [] {
                tAllStates[States.Samek.S211],
                tAllStates[States.Samek.S21],
                tAllStates[States.Samek.S2],
                tAllStates[States.Samek.S],
                tAllStates[States.Samek.Samek]
            }, tLCAResult.PathFromSourceToLCA.ToArray());

            CollectionAssert.AreEqual(new AbstractStates.State[] {
                tAllStates[States.Samek.Samek],
                tInternalTerminal
            }, tLCAResult.PathFromLCAToTarget.ToArray());
        }

        [TestMethod]
        public void Enable_And_Disable_Event_Handling()
        {   
            Assert.AreEqual(m_Machine, m_Context.Controller);
            m_Context.Log.Clear();
            Assert.AreEqual(false, m_Machine.IsBusy);
            Assert.AreEqual(false, m_Machine.IsEnabled);
            Assert.AreEqual(0, m_Context.Log.Count);
            var tAllStates = this.AllStates;

            m_Machine.EnableEventHandling();
            
            Assert.AreEqual(false, m_Machine.IsBusy);
            Assert.AreEqual(true, m_Machine.IsEnabled);
            Assert.AreEqual(1, m_Machine.Configuration.Count);
            Assert.AreEqual(tAllStates[States.Samek.S211], m_Machine.Configuration.FirstOrDefault());

            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            
            CollectionAssert.AreEqual(new String[] {
                "Samek_Init-ENTER;",
                "Samek_Init-EXIT;",
                "Transition:Samek_Init->S2;",
                "S-ENTER;",
                "S2-ENTER;", 
                "S2_Init-ENTER;", 
                "S2_Init-EXIT;", 
                "Transition:S2_Init->S211;", 
                "S21-ENTER;", 
                "S211-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();

            m_Machine.DisableEventHandling();
            
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);

            CollectionAssert.AreEqual(new String[] {
                "S211-EXIT;",
                "S21-EXIT;",
                "S2-EXIT;",
                "S-EXIT;"
            }, m_Context.Log);
            m_Context.Log.Clear();

            Assert.AreEqual(false, m_Machine.IsBusy);
            Assert.AreEqual(false, m_Machine.IsEnabled);
            Assert.AreEqual(0, m_Machine.Configuration.Count);
        }

        [TestMethod]
        public void Samek_Test_Protocol_Enter_Machine()
        {
            m_Machine.EnableEventHandling();
        
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);

            Assert.AreEqual(0, m_Context.Foo);
            CollectionAssert.AreEqual(new String[] {
                "Samek_Init-ENTER;",
                "Samek_Init-EXIT;",
                "Transition:Samek_Init->S2;",
                "States-ENTER;",
                "S2-ENTER;", 
                "S2_Init-ENTER;", 
                "S2_Init-EXIT;", 
                "Transition:S2_Init->S211;", 
                "S21-ENTER;", 
                "S211-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();

            CollectionAssert.AreEqual(new AbstractStates.State[] {
                    this.AllStates[States.Samek.S211]
                }, m_Context.Controller.Configuration.ToArray());
        }

        [TestMethod]
        public void Samek_Test_Protocol_Fire_G()
        {
            m_Context.Controller.EnableEventHandling();

            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine); 
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.G));

            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);

            CollectionAssert.AreEqual(new String[] {
                "S211-EXIT;",
                "S21-EXIT;",
                "S2-EXIT;",
                "Transition:S21->S1;",
                "S1-ENTER;",
                "S1_Init-ENTER;",
                "S1_Init-EXIT;",
                "Transition:S1_Init->S11;",
                "S11-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();

            CollectionAssert.AreEqual(new AbstractStates.State[] {
                    this.AllStates[States.Samek.S11]
                }, m_Context.Controller.Configuration.ToArray());
        }

        [TestMethod]
        public void Samek_Test_Protocol_Fire_I()
        {
            m_Context.Controller.EnableEventHandling();

            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.G));

            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.I));

            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            CollectionAssert.AreEqual(new String[] {
                "Transition:S1->S1;"
            }, m_Context.Log);
            m_Context.Log.Clear();

            CollectionAssert.AreEqual(new AbstractStates.State[] {
                    this.AllStates[States.Samek.S11]
                }, m_Context.Controller.Configuration.ToArray());
        }

        [TestMethod]
        public void Samek_Test_Protocol_Fire_A()
        {
            m_Context.Controller.EnableEventHandling();

            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.G));

            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.I));

            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.A));

            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            CollectionAssert.AreEqual(new String[] {
                "S11-EXIT;",
                "S1-EXIT;",
                "Transition:S1->S1;",
                "S1-ENTER;",
                "S1_Init-ENTER;",
                "S1_Init-EXIT;",
                "Transition:S1_Init->S11;",
                "S11-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();

            CollectionAssert.AreEqual(new AbstractStates.State[] {
                    this.AllStates[States.Samek.S11]
                }, m_Context.Controller.Configuration.ToArray());
        }

        [TestMethod]
        public void Samek_Test_Protocol_Fire_D()
        {
            m_Context.Controller.EnableEventHandling();

            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.G));

            Console.Write("{0}: ", Events.Samek.G);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.I));

            Console.Write("{0}: ", Events.Samek.I);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.A));

            Console.Write("{0}: ", Events.Samek.A);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.D));

            Console.Write("{0}: ", Events.Samek.D);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);

            Assert.AreEqual(1, m_Context.Foo);

            CollectionAssert.AreEqual(new String[] {
                "S11-EXIT;",
                "S1-EXIT;",
                "Transition:S1->States;",
                "S_Init-ENTER;",
                "S_Init-EXIT;",
                "Transition:S_Init->S11;",
                "S1-ENTER;",
                "S11-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();

            CollectionAssert.AreEqual(new AbstractStates.State[] {
                    this.AllStates[States.Samek.S11]
                }, m_Context.Controller.Configuration.ToArray());
        }

        [TestMethod]
        public void Samek_Test_Protocol_Fire_D2()
        {
            m_Context.Controller.EnableEventHandling();

            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.G));

            Console.Write("{0}: ", Events.Samek.G);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.I));

            Console.Write("{0}: ", Events.Samek.I);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.A));

            Console.Write("{0}: ", Events.Samek.A);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.D));

            Console.Write("{0}: ", Events.Samek.D);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(1, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.D));

            Console.Write("{0}: ", Events.Samek.D);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);

            Assert.AreEqual(0, m_Context.Foo);

            CollectionAssert.AreEqual(new String[] {
                "S11-EXIT;",
                "Transition:S11->S1;",
                "S1_Init-ENTER;",
                "S1_Init-EXIT;",
                "Transition:S1_Init->S11;",
                "S11-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();

            CollectionAssert.AreEqual(new AbstractStates.State[] {
                    this.AllStates[States.Samek.S11]
                }, m_Context.Controller.Configuration.ToArray());
        }

        [TestMethod]
        public void Samek_Test_Protocol_Fire_C()
        {
            m_Context.Controller.EnableEventHandling();

            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.G));

            Console.Write("{0}: ", Events.Samek.G);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.I));

            Console.Write("{0}: ", Events.Samek.I);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.A));

            Console.Write("{0}: ", Events.Samek.A);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.D));

            Console.Write("{0}: ", Events.Samek.D);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(1, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.D));

            Console.Write("{0}: ", Events.Samek.D);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.C));

            Console.Write("{0}: ", Events.Samek.C);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);

            CollectionAssert.AreEqual(new String[] {
                "S11-EXIT;",
                "S1-EXIT;",
                "Transition:S1->S2;",
                "S2-ENTER;",
                "S2_Init-ENTER;",
                "S2_Init-EXIT;",
                "Transition:S2_Init->S211;",
                "S21-ENTER;",
                "S211-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();

            CollectionAssert.AreEqual(new AbstractStates.State[] {
                    this.AllStates[States.Samek.S211]
                }, m_Context.Controller.Configuration.ToArray());
        }

        [TestMethod]
        public void Samek_Test_Protocol_Fire_E()
        {
            m_Context.Controller.EnableEventHandling();

            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.G));

            Console.Write("{0}: ", Events.Samek.G);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.I));

            Console.Write("{0}: ", Events.Samek.I);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.A));

            Console.Write("{0}: ", Events.Samek.A);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.D));

            Console.Write("{0}: ", Events.Samek.D);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(1, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.D));

            Console.Write("{0}: ", Events.Samek.D);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.C));

            Console.Write("{0}: ", Events.Samek.C);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.E));

            Console.Write("{0}: ", Events.Samek.E);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);

            CollectionAssert.AreEqual(new String[] {
                "S211-EXIT;",
                "S21-EXIT;",
                "S2-EXIT;",
                "Transition:States->S11;",
                "S1-ENTER;",
                "S11-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();

            CollectionAssert.AreEqual(new AbstractStates.State[] {
                    this.AllStates[States.Samek.S11]
                }, m_Context.Controller.Configuration.ToArray());
        }

        [TestMethod]
        public void Samek_Test_Protocol_Fire_E2()
        {
            m_Context.Controller.EnableEventHandling();

            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.G));

            Console.Write("{0}: ", Events.Samek.G);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.I));

            Console.Write("{0}: ", Events.Samek.I);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.A));

            Console.Write("{0}: ", Events.Samek.A);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.D));

            Console.Write("{0}: ", Events.Samek.D);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(1, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.D));

            Console.Write("{0}: ", Events.Samek.D);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.C));

            Console.Write("{0}: ", Events.Samek.C);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.E));

            Console.Write("{0}: ", Events.Samek.E);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.E));
            Console.Write("{0}: ", Events.Samek.E);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);

            CollectionAssert.AreEqual(new String[] {
                "S11-EXIT;",
                "S1-EXIT;",
                "Transition:States->S11;",
                "S1-ENTER;",
                "S11-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();

            CollectionAssert.AreEqual(new AbstractStates.State[] {
                    this.AllStates[States.Samek.S11]
                }, m_Context.Controller.Configuration.ToArray());
        }

        [TestMethod]
        public void Samek_Test_Protocol_Fire_G2()
        {
            m_Context.Controller.EnableEventHandling();

            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.G));

            Console.Write("{0}: ", Events.Samek.G);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.I));

            Console.Write("{0}: ", Events.Samek.I);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.A));

            Console.Write("{0}: ", Events.Samek.A);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.D));

            Console.Write("{0}: ", Events.Samek.D);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(1, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.D));

            Console.Write("{0}: ", Events.Samek.D);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.C));

            Console.Write("{0}: ", Events.Samek.C);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.E));

            Console.Write("{0}: ", Events.Samek.E);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.E));
            
            Console.Write("{0}: ", Events.Samek.E);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.G));

            Console.Write("{0}: ", Events.Samek.G);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);

            Assert.AreEqual(0, m_Context.Foo);

            CollectionAssert.AreEqual(new String[] {
                "S11-EXIT;",
                "S1-EXIT;",
                "Transition:S11->S211;",
                "S2-ENTER;",
                "S21-ENTER;",
                "S211-ENTER;"
            }, m_Context.Log);
            m_Context.Log.Clear();

            CollectionAssert.AreEqual(new AbstractStates.State[] {
                    this.AllStates[States.Samek.S211]
                }, m_Context.Controller.Configuration.ToArray());
        }

        [TestMethod]
        public void Samek_Test_Protocol_Fire_I2()
        {
            m_Context.Controller.EnableEventHandling();

            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.G));

            Console.Write("{0}: ", Events.Samek.G);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.I));

            Console.Write("{0}: ", Events.Samek.I);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.A));

            Console.Write("{0}: ", Events.Samek.A);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.D));

            Console.Write("{0}: ", Events.Samek.D);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(1, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.D));

            Console.Write("{0}: ", Events.Samek.D);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.C));

            Console.Write("{0}: ", Events.Samek.C);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.E));

            Console.Write("{0}: ", Events.Samek.E);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.E));

            Console.Write("{0}: ", Events.Samek.E);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.G));

            Console.Write("{0}: ", Events.Samek.G);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.I));

            Console.Write("{0}: ", Events.Samek.I);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);

            Assert.AreEqual(1, m_Context.Foo);

            CollectionAssert.AreEqual(new String[] {
                "Transition:S2->S2;"
            }, m_Context.Log);
            m_Context.Log.Clear();

            CollectionAssert.AreEqual(new AbstractStates.State[] {
                    this.AllStates[States.Samek.S211]
                }, m_Context.Controller.Configuration.ToArray());
        }

        [TestMethod]
        public void Samek_Test_Protocol_Fire_I3()
        {
            m_Context.Controller.EnableEventHandling();

            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.G));

            Console.Write("{0}: ", Events.Samek.G);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.I));

            Console.Write("{0}: ", Events.Samek.I);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.A));

            Console.Write("{0}: ", Events.Samek.A);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.D));

            Console.Write("{0}: ", Events.Samek.D);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(1, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.D));

            Console.Write("{0}: ", Events.Samek.D);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.C));

            Console.Write("{0}: ", Events.Samek.C);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.E));

            Console.Write("{0}: ", Events.Samek.E);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.E));

            Console.Write("{0}: ", Events.Samek.E);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.G));

            Console.Write("{0}: ", Events.Samek.G);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.I));

            Console.Write("{0}: ", Events.Samek.I);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(1, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.I));

            Console.Write("{0}: ", Events.Samek.I);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);

            Assert.AreEqual(0, m_Context.Foo);

            CollectionAssert.AreEqual(new String[] {
                "Transition:States->States;"
            }, m_Context.Log);
            m_Context.Log.Clear();

            CollectionAssert.AreEqual(new AbstractStates.State[] {
                    this.AllStates[States.Samek.S211]
                }, m_Context.Controller.Configuration.ToArray());
        }

        [TestMethod]
        public void Samek_Test_Protocol_Fire_TERMINATE()
        {
            m_Context.Controller.EnableEventHandling();

            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.G));

            Console.Write("{0}: ", Events.Samek.G);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.I));

            Console.Write("{0}: ", Events.Samek.I);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.A));

            Console.Write("{0}: ", Events.Samek.A);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.D));

            Console.Write("{0}: ", Events.Samek.D);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(1, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.D));

            Console.Write("{0}: ", Events.Samek.D);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.C));

            Console.Write("{0}: ", Events.Samek.C);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.E));

            Console.Write("{0}: ", Events.Samek.E);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.E));

            Console.Write("{0}: ", Events.Samek.E);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.G));

            Console.Write("{0}: ", Events.Samek.G);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.I));

            Console.Write("{0}: ", Events.Samek.I);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(1, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.I));

            Console.Write("{0}: ", Events.Samek.I);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);
            m_Context.Log.Clear();

            Assert.AreEqual(0, m_Context.Foo);

            m_Context.Controller.HandleEvent(StateMaster.Event.Create(Events.Samek.TERMINATE));

            Console.Write("{0}: ", Events.Samek.TERMINATE);
            m_Context.Log.ForEach(_ => Console.Write(_));
            Console.Write(Environment.NewLine);

            CollectionAssert.AreEqual(new String[] {
                "S211-EXIT;",
                "S21-EXIT;",
                "S2-EXIT;",
                "States-EXIT;",
                "Transition:States->Samek_Term;",
                "Samek_Term-ENTER;",
            }, m_Context.Log);
            m_Context.Log.Clear();

            CollectionAssert.AreEqual(new AbstractStates.State[] {
                    this.AllStates[States.Samek.Samek_Term]
                }, m_Context.Controller.Configuration.ToArray());
        }
    }
}
