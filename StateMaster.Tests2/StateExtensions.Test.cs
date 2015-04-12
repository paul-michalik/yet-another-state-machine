using StateMaster;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using StateMaster.AbstractStates;
using System.Collections.Generic;
using System.Linq;
using StateMaster.Extensions;
using StateMaster.Core;

namespace StateMaster.Tests2
{
    /// <summary>
    ///This is a test class for StateExtensions_Test and is intended
    ///to contain all StateExtensions_Test Unit Tests
    ///</summary>
    [TestClass()]
    public class StateExtensions_Test {
        enum States {
            M, I, F, 
            P1, 
            R1, S11, S12, S13, 
            R2, S21, S22, 
            R3, S31, P2, 
            R4, I4, S41, S42, 
            R5, I5, S51, S52,
            J
        }

        enum Events {
            Undefined, a, b, c, d, e, f
        }

        enum Transitions {
            t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, t17, t18, t19
        }

        class Context {
            public AbstractStates.StateMachine Controller { get; set; }

            public Context()
            {
                Log = new List<string>();
            }

            public List<String> Log { get; private set; }

            public void OnTransition(AbstractStates.State pSource, AbstractStates.State pTarget)
            {
                Log.Add(String.Format("Transition:{0}->{1};",
                   (States)pSource.ID, (States)pTarget.ID));
            }

            public void OnEnter(AbstractStates.State pState)
            {
                Log.Add(String.Format("{0}-ENTER;",
                   (States)pState.ID));
            }

            public void OnExit(AbstractStates.State pState)
            {
                Log.Add(String.Format("{0}-EXIT;",
                   (States)pState.ID));
            }

            public void OnUnhandledEvent(Event p_Event)
            {
                Log.Add(String.Format("Event {0} Unhandled", (Events)p_Event.ID));
            }

            void OnAction(String p_Action, Event p_Event)
            {
                Events tE = Enum.IsDefined(typeof(States), p_Event.ID) ? (Events)p_Event.ID : Events.Undefined;
                Log.Add(String.Format("A: {0}, E: {1};", p_Action, tE.ToString()));
            }


            void OnAction(String p_Action, Event p_Event, String p_EventData)
            {
                Events tE = Enum.IsDefined(typeof(States), p_Event.ID) ? (Events)p_Event.ID : Events.Undefined;
                Log.Add(String.Format("A: {0}, E: {1}, D: {2};", p_Action, tE.ToString(), p_EventData));
            }
        }

        IDictionary<States, AbstractStates.State> AllStates
        {
            get
            {
                return m_Context.Controller.ToDictionary(_ => (States)_.ID);
            }
        }

        IDictionary<Transitions, Core.Transition> AllTransitions
        {
            get
            {
                return AllStates
                    .Where(pS => pS.Value.Transitions != null)
                    .SelectMany(pS => pS.Value.Transitions)
                    .Select(pP => pP.Value)
                    .Where(pT => Enum.IsDefined(typeof(Transitions), pT.ID))
                    .ToDictionary(pT => (Transitions)pT.ID);
            }
        }

        void WriteLog(bool p_Clear = false)
        {
            m_Context.Log.ForEach(_ => 
                Console.Write(String.Format("{0}{1}", _, Environment.NewLine)));
            if (p_Clear)
                m_Context.Log.Clear();
        }

        void RegisterLog(Context p_Context)
        {
            foreach (var tS in p_Context.Controller) {
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
            p_Context.Controller.UnhandledEvent += m_Context.OnUnhandledEvent;
        }

        StateMachine Create(Context p_Context)
        {
            var tB = new Builder();
            var M = tB.StateMachine().ID(States.M);
            var I = tB.Initial(M).ID(States.I);
            var F = tB.Fork(M).ID(States.F); // Fork
            
            var P1 = tB.Composite(M).ID(States.P1); // Parallel
            
            var R1 = tB.Region(P1).ID(States.R1); // Region
            var S11 = tB.Simple(R1).ID(States.S11);
            var S12 = tB.Simple(R1).ID(States.S12);
            var S13 = tB.Simple(R1).ID(States.S13);
            
            var R2 = tB.Region(P1).ID(States.R2); // Region
            var S21 = tB.Simple(R2).ID(States.S21);
            var S22 = tB.Simple(R2).ID(States.S22);
            
            var R3 = tB.Region(P1).ID(States.R3); // Region
            var S31 = tB.Simple(R3).ID(States.S31);
            
            var P2 = tB.Composite(R3).ID(States.P2); // Parallel
            
            var R4 = tB.Region(P2).ID(States.R4); // Region
            var I4 = tB.Initial(R4).ID(States.I4);
            var S41 = tB.Simple(R4).ID(States.S41);
            var S42 = tB.Simple(R4).ID(States.S42);

            var R5 = tB.Region(P2).ID(States.R5); // Region
            var I5 = tB.Initial(R5).ID(States.I5);
            var S51 = tB.Simple(R5).ID(States.S51);
            var S52 = tB.Simple(R5).ID(States.S52);

            var J = tB.Join(M).ID(States.J); // Join
            var Terminal = tB.Terminal(M);

            // transitions
            I.AddTransition(F).ID(Transitions.t1);
            F.AddTransition(S11).ID(Transitions.t2)
             .AddTransition(S21).ID(Transitions.t3)
             .AddTransition(S31).ID(Transitions.t4);
            S11.AddTransition(Events.a, S12).ID(Transitions.t5);
            S12.AddTransition(Events.a, S13).ID(Transitions.t6);
            S13.AddTransition(Events.e, J).ID(Transitions.t7);
            S21.AddTransition(Events.b, S22).ID(Transitions.t8);
            S22.AddTransition(Events.b, S21).ID(Transitions.t9)
               .AddTransition(Events.e, J).ID(Transitions.t10);
            S31.AddTransition(P2).ID(Transitions.t11);
            I4.AddTransition(S42).ID(Transitions.t19);
            S41.AddTransition(Events.d, S42).ID(Transitions.t12);
            S42.AddTransition(Events.d, S41).ID(Transitions.t13);
            I5.AddTransition(S51).ID(Transitions.t14);
            S51.AddTransition(Events.f, S52).ID(Transitions.t15);
            S52.AddTransition(Events.f, S51).ID(Transitions.t16);
            P2.AddTransition(Events.e, J).ID(Transitions.t17);
            J.AddTransition(Terminal).ID(Transitions.t18);

            // p_Context.Controller = tB.Build();
            p_Context.Controller = M;
            RegisterLog(p_Context);

            return M;
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
        //
        Context m_Context;
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            m_Context = new Context();
            Create(m_Context);
        }
        //
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            m_Context = null;
        }
        //
        #endregion

        /// <summary>
        ///A test for PathToParent
        ///</summary>
        [TestMethod()][TestProperty("Module", "State.Extensions")]
        public void PathToParent_From_Null_Test()
        {
            State tS = null;
            CollectionAssert.AreEqual(
                new State [] {},
                tS.PathToParent().ToArray());
        }

        [TestMethod()]
        [TestProperty("Module", "State.Extensions")]
        public void PathToParent_From_R4_Test()
        {
            var tAllStates = AllStates;
            CollectionAssert.AreEqual(new States[] {
                States.R4, States.P2, States.R3, States.P1, States.M
            }, tAllStates[States.R4].PathToParent().Select(_ => (States)_.ID).ToArray());
        }

        [TestMethod()][TestProperty("Module", "State.Extensions")]
        public void PathToParent_From_P1_Test()
        {
            var tAllStates = AllStates;

            CollectionAssert.AreEqual(new States[] {
                States.P1, States.M
            }, tAllStates[States.P1].PathToParent().Select(_ => (States)_.ID).ToArray());
        }

        [TestMethod()][TestProperty("Module", "State.Extensions")]
        public void PathToParent_From_S52_Test()
        {
            var tAllStates = AllStates;

            CollectionAssert.AreEqual(new States[] {
                States.S52, States.R5, States.P2, States.R3, States.P1, States.M
            }, tAllStates[States.S52].PathToParent().Select(_ => (States)_.ID).ToArray());
        }

        [TestMethod()]
        [TestProperty("Module", "State.Extensions")]
        public void PathToParent_From_S52_To_R3_Test()
        {
            var tAllStates = AllStates;

            CollectionAssert.AreEqual(new States[] {
                States.S52, States.R5, States.P2, States.R3
            }, tAllStates[States.S52]
            .PathToParent(tAllStates[States.R3] as Composite)
            .Select(_ => (States)_.ID).ToArray());
        }

        /// <summary>
        ///A test for TraverseBreadthFirst
        ///</summary>
        [TestMethod()][TestProperty("Module", "State.Extensions")]
        public void TraverseBreadthFirst_From_Null_Test()
        {
            AbstractStates.State tS = null;
            var tResult = tS.TraverseBreadthFirst().Select(_ => (States)_.ID);
            CollectionAssert.AreEqual(new States[0] {
            }, tResult.ToList());
        }

        [TestMethod()]
        [TestProperty("Module", "State.Extensions")]
        public void TraverseBreadthFirst_From_Composite_With_No_Children_Test()
        {
            var tB = new Builder();
            var M = tB.StateMachine();
            State C = tB.Composite(M).ID(24);
            var tResult = C.TraverseBreadthFirst().Select(_ => _.ID);
            CollectionAssert.AreEqual(new Int32[] {
                24
            }, tResult.ToList());
        }

        [TestMethod()]
        [TestProperty("Module", "State.Extensions")]
        public void TraverseBredthFirst_From_R4_Test()
        {
            var tAllStates = AllStates;
            var tResult = tAllStates[States.R4]
                .TraverseBreadthFirst()
                .Select(_ => (States)_.ID);
            
            tResult.ToList().ForEach(_ => Console.Write("{0} ", _.ToString()));

            CollectionAssert.AreEqual(new States[] {
                States.R4, States.I4, States.S41, States.S42
            }, tResult.ToList());
        }

        [TestMethod()]
        [TestProperty("Module", "State.Extensions")]
        public void TraverseBreadthFirst_From_I4_Test()
        {
            var tAllStates = AllStates;
            var tResult = tAllStates[States.I4]
                .TraverseBreadthFirst()
                .Select(_ => (States)_.ID);

            tResult.ToList().ForEach(_ => Console.Write("{0} ", _.ToString()));

            CollectionAssert.AreEqual(new States[] {
                States.I4
            }, tResult.ToList());
        }

        [TestMethod()]
        [TestProperty("Module", "State.Extensions")]
        public void TraverseBreadthFirst_From_S52_Test()
        {
            var tAllStates = AllStates;
            var tResult = tAllStates[States.S52]
                .TraverseBreadthFirst()
                .Select(_ => (States)_.ID);

            tResult.ToList().ForEach(_ => Console.Write("{0} ", _.ToString()));

            CollectionAssert.AreEqual(new States[] {
                States.S52
            }, tResult.ToList());
        }

        [TestMethod()]
        [TestProperty("Module", "State.Extensions")]
        public void TraverseBreadthFirst_From_P1_Test()
        {
            var tAllStates = AllStates;
            var tResult = tAllStates[States.P1]
                .TraverseBreadthFirst()
                .Select(_ => (States)_.ID);

            tResult.ToList().ForEach(_ => Console.Write("{0} ", _.ToString()));

            CollectionAssert.AreEqual(new States[] {
                States.P1,
                States.R1, States.R2, States.R3,
                States.S11, States.S12, States.S13,
                States.S21, States.S22, 
                States.S31, States.P2,
                States.R4, States.R5,
                States.I4, States.S41, States.S42,
                States.I5, States.S51, States.S52
            }, tResult.ToList());
        }

        /// <summary>
        ///A test for TraversePostOrder
        ///</summary>
        [TestMethod()]
        [TestProperty("Module", "State.Extensions")]
        public void TraversePostOrder_From_Null_Test()
        {
            AbstractStates.State tS = null;
            var tResult = tS.TraversePostOrder().Select(_ => (States)_.ID);
            CollectionAssert.AreEqual(new States[0] {
            }, tResult.ToList());
        }

        [TestMethod()]
        [TestProperty("Module", "State.Extensions")]
        public void TraversePostOrder_From_Composite_With_No_Children_Test()
        {
            var tB = new Builder();
            var M = tB.StateMachine();
            State C = tB.Composite(M).ID(24);
            var tResult = C.TraversePostOrder().Select(_ => _.ID);
            CollectionAssert.AreEqual(new Int32[] {
                24
            }, tResult.ToList());
        }

        [TestMethod()]
        [TestProperty("Module", "State.Extensions")]
        public void TraversePostOrder_From_R4_Test()
        {
            var tAllStates = AllStates;
            var tResult = tAllStates[States.R4]
                .TraversePostOrder()
                .Select(_ => (States)_.ID);
            
            tResult.ToList().ForEach(_ => Console.Write("{0} ", _.ToString()));

            CollectionAssert.AreEqual(new States[] {
                States.I4, States.S41, States.S42, States.R4
            }, tResult.ToList());
        }

        [TestMethod()]
        [TestProperty("Module", "State.Extensions")]
        public void TraversePostOrder_From_I4_Test()
        {
            var tAllStates = AllStates;
            var tResult = tAllStates[States.I4]
                .TraversePostOrder()
                .Select(_ => (States)_.ID);

            tResult.ToList().ForEach(_ => Console.Write("{0} ", _.ToString()));

            CollectionAssert.AreEqual(new States[] {
                States.I4
            }, tResult.ToList());
        }

        [TestMethod()]
        [TestProperty("Module", "State.Extensions")]
        public void TraversePostOrder_From_S52_Test()
        {
            var tAllStates = AllStates;
            var tResult = tAllStates[States.S52]
                .TraversePostOrder()
                .Select(_ => (States)_.ID);

            tResult.ToList().ForEach(_ => Console.Write("{0} ", _.ToString()));

            CollectionAssert.AreEqual(new States[] {
                States.S52
            }, tResult.ToList());
        }

        [TestMethod()]
        [TestProperty("Module", "State.Extensions")]
        public void TraversePostOrder_From_P1_Test()
        {
            var tAllStates = AllStates;
            var tResult = tAllStates[States.P1]
                .TraversePostOrder()
                .Select(_ => (States)_.ID);

            tResult.ToList().ForEach(_ => Console.Write("{0} ", _.ToString()));

            CollectionAssert.AreEqual(new States[] {
                States.S11, States.S12, States.S13, States.R1,
                States.S21, States.S22, States.R2,
                States.S31,
                States.I4, States.S41, States.S42, States.R4,
                States.I5, States.S51, States.S52, States.R5,
                States.P2,
                States.R3,
                States.P1
            }, tResult.ToList());
        }

        /// <summary>
        ///A test for TraversePreOrder
        ///</summary>
        [TestMethod()]
        [TestProperty("Module", "State.Extensions")]
        public void TraversePreOrder_From_Null_Test()
        {
            AbstractStates.State tS = null;
            var tResult = tS.TraversePreOrder().Select(_ => (States)_.ID);
            CollectionAssert.AreEqual(new States[0] {
            }, tResult.ToList());
        }

        [TestMethod()]
        [TestProperty("Module", "State.Extensions")]
        public void TraversePreOrder_From_Composite_With_No_Children_Test()
        {
            var tB = new Builder();
            var M = tB.StateMachine();
            State C = tB.Composite(M).ID(24);
            var tResult = C.TraversePreOrder().Select(_ => _.ID);
            CollectionAssert.AreEqual(new Int32[] {
                24
            }, tResult.ToList());
        }

        [TestMethod()]
        [TestProperty("Module", "State.Extensions")]
        public void TraversePreOrder_From_R4_Test()
        {
            var tAllStates = AllStates;
            var tResult = tAllStates[States.R4]
                .TraversePreOrder()
                .Select(_ => (States)_.ID);

            tResult.ToList().ForEach(_ => Console.Write("{0} ", _.ToString()));

            CollectionAssert.AreEqual(new States[] {
                States.R4,
                States.I4, States.S41, States.S42
            }, tResult.ToList());
        }

        [TestMethod()]
        [TestProperty("Module", "State.Extensions")]
        public void TraversePreOrder_From_I4_Test()
        {
            var tAllStates = AllStates;
            var tResult = tAllStates[States.I4]
                .TraversePreOrder()
                .Select(_ => (States)_.ID);

            tResult.ToList().ForEach(_ => Console.Write("{0} ", _.ToString()));

            CollectionAssert.AreEqual(new States[] {
                States.I4
            }, tResult.ToList());
        }

        [TestMethod()]
        [TestProperty("Module", "State.Extensions")]
        public void TraversePreOrder_From_S52_Test()
        {
            var tAllStates = AllStates;
            var tResult = tAllStates[States.S52]
                .TraversePreOrder()
                .Select(_ => (States)_.ID);

            tResult.ToList().ForEach(_ => Console.Write("{0} ", _.ToString()));

            CollectionAssert.AreEqual(new States[] {
                States.S52
            }, tResult.ToList());
        }

        [TestMethod()]
        [TestProperty("Module", "State.Extensions")]
        public void TraversePreOrder_From_P1_Test()
        {
            var tAllStates = AllStates;
            var tResult = tAllStates[States.P1]
                .TraversePreOrder()
                .Select(_ => (States)_.ID);

            tResult.ToList().ForEach(_ => Console.Write("{0} ", _.ToString()));

            CollectionAssert.AreEqual(new States[] {
                States.P1,
                States.R1,
                States.S11, States.S12, States.S13,
                States.R2,
                States.S21, States.S22,
                States.R3,
                States.S31, States.P2, 
                States.R4,
                States.I4, States.S41, States.S42,
                States.R5,
                States.I5, States.S51, States.S52
            }, tResult.ToList());
        }

        [TestMethod()]
        [TestProperty("Module", "State.Extensions")]
        public void CollectEnabledTransitions_If_Active_Is_I()
        {
            var tAllStates = AllStates;
            tAllStates[States.M].IsActive = true;
            tAllStates[States.I].IsActive = true;

            var tConfiguration = m_Context.Controller
                .TraversePostOrder()
                .Where(pS => pS.IsActive);

            {
                var tTmp = tConfiguration.ToList();
                tTmp.ForEach(pS => Console.WriteLine(((States)pS.ID).ToString()));
            }

            CollectionAssert.AreEqual(new States[] {
                States.I, States.M
            }, tConfiguration.Select(pS => (States)pS.ID).ToList());

            var tEvent = Core.Constants.Create(Core.Constants.InternalEvents.Completion);
            var tEnabledTransitionSet = tConfiguration
                .SelectMany(pS => pS.GetEnabledTransitionSet(tEvent));

            {
                var tTmp = tEnabledTransitionSet.ToList();
                tTmp.ForEach(pT => Console.WriteLine(((Transitions)pT.ID).ToString()));
            }

            CollectionAssert.AreEqual(new Transitions[] {
                Transitions.t1
            }, tEnabledTransitionSet.Select(pT => (Transitions)pT.ID).ToList());
        }

        [TestMethod()][TestProperty("Module", "State.Extensions")]
        public void CollectEnabledTransitions_If_Active_Is_F()
        {
            var tAllStates = AllStates;
            tAllStates[States.M].IsActive = true;
            tAllStates[States.F].IsActive = true;

            var tConfiguration = m_Context.Controller
                .TraversePostOrder()
                .Where(pS => pS.IsActive);

            {
                var tTmp = tConfiguration.ToList();
                tTmp.ForEach(pS => Console.WriteLine(((States)pS.ID).ToString()));
            }

            CollectionAssert.AreEqual(new States[] {
                States.F, States.M
            }, tConfiguration.Select(pS => (States)pS.ID).ToList());

            var tEvent = Core.Constants.Create(Core.Constants.InternalEvents.Completion);
            var tEnabledTransitionSet = tConfiguration
                .SelectMany(pS => pS.GetEnabledTransitionSet(tEvent));

            {
                var tTmp = tEnabledTransitionSet.ToList();
                tTmp.ForEach(pT => Console.WriteLine(((Transitions)pT.ID).ToString()));
            }

            CollectionAssert.AreEqual(new Transitions[] {
                Transitions.t2,
                Transitions.t3,
                Transitions.t4
            }, tEnabledTransitionSet.Select(pT => (Transitions)pT.ID).ToList());
        }

        [TestMethod()][TestProperty("Module", "State.Extensions")]
        public void TraverseCompoundForward_StartAt_t1()
        {
            var tAllStates = AllStates;
            var tAllTrans = AllTransitions;

            var tT = tAllTrans[Transitions.t1]
                .TraverseCompoundForward();

            {
                var tTmp = tT.ToList();
                tTmp.ForEach(pT => Console.WriteLine(((Transitions)pT.ID).ToString()));
            }

            CollectionAssert.AreEqual(new Transitions[] {
                Transitions.t1,
                Transitions.t2,
                Transitions.t3,
                Transitions.t4,
            }, tT.Select(pT => (Transitions)pT.ID).ToList());

        }

        [TestMethod()][TestProperty("Module", "State.Extensions")]
        public void TraverseCompoundForward_StartAt_t7()
        {
            var tAllStates = AllStates;
            var tAllTrans = AllTransitions;

            var tT = tAllTrans[Transitions.t7]
                .TraverseCompoundForward();

            {
                var tTmp = tT.ToList();
                tTmp.ForEach(pT => Console.WriteLine(((Transitions)pT.ID).ToString()));
            }

            CollectionAssert.AreEqual(new Transitions[] {
                Transitions.t7,
                Transitions.t18,
            }, tT.Select(pT => (Transitions)pT.ID).ToList());

        }

        [TestMethod()][TestProperty("Module", "State.Extensions")]
        public void TraverseCompoundForward_StartAt_t11()
        {
            var tAllStates = AllStates;
            var tAllTrans = AllTransitions;

            var tT = tAllTrans[Transitions.t11]
                .TraverseCompoundForward();

            {
                var tTmp = tT.ToList();
                tTmp.ForEach(pT => Console.WriteLine(((Transitions)pT.ID).ToString()));
            }

            CollectionAssert.AreEqual(new Transitions[] {
                Transitions.t11
            }, tT.Select(pT => (Transitions)pT.ID).ToList());

        }

        // [TestCategory("Config1=P1_R1_R2_R3_P2_R4_R5_S13_S22_S42_S52")]
        [TestMethod()][TestProperty("Module", "State.Extensions")]
        public void CollectEnabledTransitions_If_Active_Is_Config1()
        {
            var tAllStates = AllStates;
            tAllStates[States.M].IsActive = true;
            tAllStates[States.P1].IsActive = true;
            tAllStates[States.R1].IsActive = true;
            tAllStates[States.R2].IsActive = true;
            tAllStates[States.R3].IsActive = true;
            tAllStates[States.P2].IsActive = true;
            tAllStates[States.R4].IsActive = true;
            tAllStates[States.R5].IsActive = true;
            tAllStates[States.S13].IsActive = true;
            tAllStates[States.S22].IsActive = true;
            tAllStates[States.S42].IsActive = true;
            tAllStates[States.S52].IsActive = true;

            var tConfiguration = m_Context.Controller
                .TraversePostOrder()
                .Where(pS => pS.IsActive);

            {
                var tTmp = tConfiguration.ToList();
                tTmp.ForEach(pS => Console.WriteLine(((States)pS.ID).ToString()));
            }

            CollectionAssert.AreEqual(new States[] {
                States.S13, 
                States.R1,
                States.S22,
                States.R2,
                States.S42,
                States.R4,
                States.S52,
                States.R5,
                States.P2,
                States.R3,
                States.P1,
                States.M
            }, tConfiguration.Select(pS => (States)pS.ID).ToList());

            var tEvent = Event.Create(Events.e);
            var tEnabledTransitionSet = tConfiguration
                .SelectMany(pS => pS.GetEnabledTransitionSet(tEvent));

            {
                var tTmp = tEnabledTransitionSet.ToList();
                tTmp.ForEach(pT => Console.WriteLine(((Transitions)pT.ID).ToString()));
            }

            CollectionAssert.AreEqual(new Transitions[] {
                Transitions.t7,
                Transitions.t10,
                Transitions.t17
            }, tEnabledTransitionSet.Select(pT => (Transitions)pT.ID).ToList());
        }

        [TestMethod()]
        [TestProperty("Module", "State.Extensions")]
        public void EnableEventHandling_Should_Activate_Given_States()
        {
            var tAllStates = AllStates;
            m_Context.Controller.EnableEventHandling();

            WriteLog();

            CollectionAssert.AreEqual(new String[] {
                "I-ENTER;",
                "I-EXIT;",
                "Transition:I->F;",
                "F-ENTER;",
                "F-EXIT;",
                "Transition:F->S11;",
                "Transition:F->S21;",
                "Transition:F->S31;",
                "P1-ENTER;",
                "R1-ENTER;",
                "S11-ENTER;",
                "R2-ENTER;",
                "S21-ENTER;",
                "R3-ENTER;",
                "S31-ENTER;",
                "S31-EXIT;",
                "Transition:S31->P2;",
                "P2-ENTER;",
                "R4-ENTER;",
                "I4-ENTER;",
                "R5-ENTER;",
                "I5-ENTER;",
                "I4-EXIT;",
                "Transition:I4->S42;",
                "S42-ENTER;",
                "I5-EXIT;",
                "Transition:I5->S51;",
                "S52-ENTER;"
            }, m_Context.Log.ToList());
        }
    }
}
