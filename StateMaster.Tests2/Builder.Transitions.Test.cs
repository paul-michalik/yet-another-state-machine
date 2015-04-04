using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StateMaster;

namespace StateMaster.Tests2 {
    /// <summary>
    /// Summary description for SourceBuilder
    /// </summary>
    [TestClass]
    public class Builder_Transitions_Test {

        class SamekContext {
            public SamekContext()
            {
                Log = new List<string>();
            }

            public List<String> Log { get; private set; }
            
            public Int32 Foo { get; set; }
        }

        SamekContext m_Context;

        public Builder_Transitions_Test()
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
        }

        //
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup() 
        {
            m_Context = null;
        }
        
        #endregion

        [TestMethod]
        public void Add_All_Internal_Transitions()
        {
            // Setup:
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
            var S211 = tB.Composite(S21).ID(States.Samek.S211);
            var Samek_Term = tB.Terminal(Samek).ID(States.Samek.Samek_Term);

            // Assert before transitions (must be empty except the default terminal transition):
            Assert.AreEqual(1,
                Samek.Buildee
                .Where(_ => _.Transitions != null)
                .Select(_ => _.Transitions)
                .Count());

            // I: States -> States, Guard, Action
            Core.Transition tT0 =
                S.AddTransition(Events.Samek.I) // use standard transition API
                .Guard(_1 => m_Context.Foo == 1)
                .Action(_1 => m_Context.Foo = 0)
                .Kind(TransitionKind.Internal);

            Assert.IsNotNull(S.Buildee.Transitions);
            Assert.AreEqual(S.Buildee, tT0.Target);
            Assert.AreEqual((Int32)States.Samek.S, tT0.Target.ID);
            Assert.AreEqual(Builder.DefaultIDMinValue + 1, tT0.ID);
            Assert.AreEqual(TransitionKind.Internal, tT0.Kind);
            CollectionAssert.AreEqual(
                new KeyValuePair<Int32, Core.Transition>[1] {
                    new KeyValuePair<Int32, Core.Transition>((Int32)Events.Samek.I, tT0)
                },
                S.Buildee.Transitions.ToArray());

            // I: S1 -> S1
            Core.Transition tT1 = tB
                .AddTransition(S1, S1)
                .Trigger(Events.Samek.I);

            Assert.IsNotNull(S1.Buildee.Transitions);
            Assert.AreEqual(S1.Buildee, tT1.Target);
            Assert.AreEqual((Int32)States.Samek.S1, tT1.Target.ID);
            Assert.AreEqual(Builder.DefaultIDMinValue + 2, tT1.ID);
            Assert.AreEqual(TransitionKind.Internal, tT1.Kind);
            Assert.AreEqual(1, S1.Buildee.Transitions.Count());
            Assert.AreEqual((Int32)Events.Samek.I, S1.Buildee.Transitions.First().Key);
            Assert.AreEqual(tT1, S1.Buildee.Transitions.First().Value);
            CollectionAssert.AreEquivalent(
                new KeyValuePair<Int32, Core.Transition>[1] {
                    new KeyValuePair<Int32, Core.Transition>((Int32)Events.Samek.I, tT1)
                },
                S1.Buildee.Transitions.ToArray());


            // I: S2 -> S2
            Core.Transition tT2 = tB
                .AddTransition(S2, S2)
                .Trigger(Events.Samek.I)
                .Guard(_1 => m_Context.Foo == 0)
                .Action(_1 => m_Context.Foo = 1);

            Assert.IsNotNull(S2.Buildee.Transitions);
            Assert.AreEqual(S2.Buildee, tT2.Target);
            Assert.AreEqual((Int32)States.Samek.S2, tT2.Target.ID);
            Assert.AreEqual(Builder.DefaultIDMinValue + 3, tT2.ID);
            Assert.AreEqual(TransitionKind.Internal, tT2.Kind);
            Assert.AreEqual(1, S2.Buildee.Transitions.Count());
            Assert.AreEqual((Int32)Events.Samek.I, S2.Buildee.Transitions.First().Key);
            Assert.AreEqual(tT2, S2.Buildee.Transitions.First().Value);
            CollectionAssert.AreEquivalent(
                new KeyValuePair<Int32, Core.Transition>[1] {
                    new KeyValuePair<Int32, Core.Transition>((Int32)Events.Samek.I, tT2)
                },
                S2.Buildee.Transitions.ToArray());

            // Assert:
            // there must be three transitions in the enumeration
            // tT0: States -> States, I/[G]/A, InternalEvents
            // tT1: S1 -> S1, I/null/null, InternalEvents
            // tT2: S2 -> S2, I/[G]/A, InternalEvents
            CollectionAssert.AreEqual(new KeyValuePair<Events.Samek, Core.Transition>[3] {
                new KeyValuePair<Events.Samek, Core.Transition>(Events.Samek.I, tT0),
                new KeyValuePair<Events.Samek, Core.Transition>(Events.Samek.I, tT1),
                new KeyValuePair<Events.Samek, Core.Transition>(Events.Samek.I, tT2)
            }, Samek.Buildee
            .Where(_1 => _1.Transitions != null)
            .SelectMany(_1 => _1.Transitions)
            .Select(_1 => new KeyValuePair<Events.Samek, Core.Transition>((Events.Samek)_1.Key, _1.Value))
            .ToArray());
        }

        [TestMethod]
        public void Add_Transitions_From_Samek_Init()
        {
            // Setup:
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
            var S211 = tB.Composite(S21).ID(States.Samek.S211);
            var Samek_Term = tB.Terminal(Samek).ID(States.Samek.Samek_Term);

            // Assert before transitions (must be empty except the default terminal transition):
            Assert.AreEqual(1,
                Samek.Buildee
                .Where(_ => _.Transitions != null)
                .Select(_ => _.Transitions)
                .Count());

            Core.Transition tT0 = tB
                .AddTransition(Samek_Init, S2)
                .Action(_1 => m_Context.Foo = 0);

            Assert.IsNotNull(Samek_Init.Buildee.Transitions);
            Assert.AreEqual(S2.Buildee, tT0.Target);
            Assert.AreEqual((Int32)States.Samek.S2, tT0.Target.ID);
            Assert.AreEqual(Builder.DefaultIDMinValue + 1, tT0.ID);
            Assert.AreEqual(TransitionKind.Local, tT0.Kind);
            Assert.AreEqual(1, Samek_Init.Buildee.Transitions.Count());
            Assert.AreEqual(
                (Int32)Core.Constants.InternalEvents.Completion, 
                Samek_Init.Buildee.Transitions.First().Key);
            Assert.AreEqual(tT0, Samek_Init.Buildee.Transitions.First().Value);
            CollectionAssert.AreEquivalent(
                new KeyValuePair<Int32, Core.Transition>[1] {
                    new KeyValuePair<Int32, Core.Transition>((Int32)Core.Constants.InternalEvents.Completion, tT0)
                },
                Samek_Init.Buildee.Transitions.ToArray());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_All_Transitions_From_Initial_States_Conflicting_IDs()
        {
            // Setup:
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
            var S211 = tB.Composite(S21).ID(States.Samek.S211);
            var Samek_Term = tB.Terminal(Samek).ID(States.Samek.Samek_Term);

            // Assert before transitions (must be empty except the default terminal transition):
            Assert.AreEqual(1,
                Samek.Buildee
                .Where(_ => _.Transitions != null)
                .Select(_ => _.Transitions)
                .Count());

            Samek_Init.AddTransition(S2).ID(Transitions.Samek.t1)
                .Action(_1 => m_Context.Foo = 0);
            S_Init.AddTransition(S11).ID(Transitions.Samek.t1);

        }

        [TestMethod]
        public void Add_All_Transitions_From_Initial_States()
        {
            // Setup:
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
            var S211 = tB.Composite(S21).ID(States.Samek.S211);
            var Samek_Term = tB.Terminal(Samek).ID(States.Samek.Samek_Term);

            // Assert before transitions (must be empty except the default terminal transition):
            Assert.AreEqual(1,
                Samek.Buildee
                .Where(_ => _.Transitions != null)
                .Select(_ => _.Transitions)
                .Count());

            // Act
            tB.AddTransition(Samek_Init, S2).ID(Transitions.Samek.t1)
                .Action(_1 => m_Context.Foo = 0)
              .AddTransition(S_Init, S11).ID(Transitions.Samek.t2)
              .AddTransition(S1_Init, S11).ID(Transitions.Samek.t3)
              .AddTransition(S2_Init, S211).ID(Transitions.Samek.t4)
              .AddTransition(S21_Init, S211).ID(Transitions.Samek.t5);

            // Assert
            CollectionAssert.AreEquivalent(
                new Transitions.Samek[] {
                    Transitions.Samek.t1,
                    Transitions.Samek.t2,
                    Transitions.Samek.t3,
                    Transitions.Samek.t4,
                    Transitions.Samek.t5
                },
                Samek.Buildee
                    .Where(_1 => _1.Transitions != null)
                    .SelectMany(_1 => _1.Transitions)
                    .Select(_1 => (Transitions.Samek)_1.Value.ID)
                    .ToArray());

            // Assert transitions
            var tTransitionsByID = Samek.Buildee
                .Where(_1 => _1.Transitions != null)
                .SelectMany(_1 => _1.Transitions)
                .ToDictionary(_1 => (Transitions.Samek)_1.Value.ID, _1 => _1.Value);

            CollectionAssert.AreEquivalent(
                new Transitions.Samek[] {
                    Transitions.Samek.t1,
                    Transitions.Samek.t2,
                    Transitions.Samek.t3,
                    Transitions.Samek.t4,
                    Transitions.Samek.t5
                },
                tTransitionsByID.Select(_1 => _1.Key).ToArray());

            CollectionAssert.AreEquivalent(
                new AbstractStates.State[] {
                    S2, S11, S11, S211, S211
                },
                tTransitionsByID.Select(_1 => _1.Value.Target).ToArray());
        }
    }
}
