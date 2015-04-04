using StateMaster;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StateMaster.Tests2
{
    
    
    /// <summary>
    ///This is a test class for Builder_Test and is intended
    ///to contain all Builder_Test Unit Tests
    ///</summary>
    [TestClass()]
    public class Builder_Test {

        Builder m_Builder;
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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //

        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void Builder_Test_SetUp()
        {
            m_Builder = new StateMaster.Builder();
        }
        
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void Builder_Test_TearDown()
        {
            m_Builder = null;
        }
        
        #endregion

        [TestMethod()]
        public void Builder_Machine0_Uses_Node_Builders()
        {
            var tM = m_Builder.StateMachine().ID(States.Machine0.Machine);
            Assert.AreEqual((Int32)States.Machine0.Machine, tM.Buildee.ID);
            Assert.AreEqual(null, tM.Buildee.Parent);
            tM.Enter(() => System.Console.WriteLine("Enter called"));

            var tS0 = m_Builder.Simple(tM).ID(States.Machine0.S0);
            Assert.AreEqual((Int32)States.Machine0.S0, tS0.Buildee.ID);
            Assert.AreEqual(tM, tS0.Buildee.Parent);

            var tS1 = m_Builder.Simple(tM).ID(States.Machine0.S1);
            Assert.AreEqual((Int32)States.Machine0.S1, tS1.Buildee.ID);
            Assert.AreEqual(tM, tS1.Buildee.Parent);
        }

        [TestMethod()]
        public void StateMachine_Test()
        {
            AbstractStates.StateMachine tM = m_Builder.StateMachine();
            Assert.AreEqual(Builder.DefaultIDMinValue + 0, tM.ID);
            Assert.AreEqual(null, tM.Parent);
            // the default transition to internal.terminal
            Assert.IsNotNull(tM.Transitions);
            // the default transition to internal.terminal is not enumerated
            Assert.AreEqual(0, tM.Transitions.Count());
            CollectionAssert.AreEqual(tM.Configuration.ToArray(), new AbstractStates.State[] { 
            });
        }

        [TestMethod()]
        public void StateMachine_Test_Add_Initial()
        {
            AbstractStates.StateMachine tM = m_Builder.StateMachine();
            AbstractStates.Pseudo tM_initial = m_Builder.Initial(tM);
            Assert.AreEqual(Builder.DefaultIDMinValue + 1, tM_initial.ID);
            Assert.AreEqual(tM, tM_initial.Parent);
            Assert.AreEqual(null, tM_initial.Transitions);
            CollectionAssert.AreEqual(new AbstractStates.State [] {
                tM, tM_initial
            }, tM.ToArray());
        }

        [TestMethod()]
        public void StateMachine_Test_Add_Initial_And_Simple()
        {
            var tM = m_Builder.StateMachine();
            var tM_initial = m_Builder.Initial(tM);
            Assert.AreEqual(Builder.DefaultIDMinValue + 1, tM_initial.Buildee.ID);
            Assert.AreEqual(tM, tM_initial.Buildee.Parent);
            Assert.AreEqual(null, tM_initial.Buildee.Transitions);

            var tS1 = m_Builder.Simple(tM);
            Assert.AreEqual(Builder.DefaultIDMinValue + 2, tS1.Buildee.ID);
            Assert.AreEqual(tM, tS1.Buildee.Parent);
            Assert.AreEqual(null, tS1.Buildee.Transitions);

            CollectionAssert.AreEqual(new AbstractStates.State [] {
                tM.Buildee, tM_initial.Buildee, tS1.Buildee
            }, tM.Buildee.ToArray());
        }

        [TestMethod()]
        public void StateMachine_Test0()
        {
            var tM = m_Builder.StateMachine();
            var S0 = m_Builder.Composite(tM);
            var S1 = m_Builder.Composite(S0);
            var S2 = m_Builder.Simple(S1);
            var S3 = m_Builder.Simple(S1);
            var S4 = m_Builder.Simple(S0);

            Assert.AreEqual(6, tM.Buildee.ToArray().Length);

            CollectionAssert.AreEqual(new AbstractStates.State[] {
                tM, S0, S1, S2, S3, S4
            }, tM.Buildee.ToArray());

            CollectionAssert.AreEqual(new [] {
                Builder.DefaultIDMinValue + 0, 
                Builder.DefaultIDMinValue + 1, 
                Builder.DefaultIDMinValue + 2, 
                Builder.DefaultIDMinValue + 3, 
                Builder.DefaultIDMinValue + 4, 
                Builder.DefaultIDMinValue + 5,
            }, tM.Buildee.Select(_1 => _1.ID).ToArray());
        }

        [TestMethod()]
        public void StateMachine_Test1()
        {
            var tM = m_Builder.StateMachine().ID(-1);
            var S0 = m_Builder.Composite(tM).ID(0);
            var S1 = m_Builder.Composite(S0).ID(1);
            var S2 = m_Builder.Simple(S1).ID(2);
            var S5 = m_Builder.Composite(S1).ID(5);
            var S3 = m_Builder.Simple(S5).ID(3);
            var S4 = m_Builder.Simple(S5).ID(4);
            var S6 = m_Builder.Simple(S1).ID(6);
            var S13 = m_Builder.Composite(S0).ID(13);
            var S7 = m_Builder.Simple(S13).ID(7);
            var S12 = m_Builder.Composite(S13).ID(12);
            var S8 = m_Builder.Simple(S12).ID(8);
            var S11 = m_Builder.Composite(S12).ID(11);
            var S9 = m_Builder.Simple(S11).ID(9);
            var S10 = m_Builder.Simple(S11).ID(10);

            Assert.AreEqual(15, tM.Buildee.ToArray().Length);

            CollectionAssert.AreEqual(new AbstractStates.State[] {
                tM, S0, S1, S2, S5, S3, S4, S6, S13, S7, S12, S8, S11, S9, S10
            }, tM.Buildee.ToArray());

            CollectionAssert.AreEqual(new[] {
                -1, 0, 1, 2, 5, 3, 4, 6, 13, 7, 12, 8, 11, 9, 10
            }, tM.Buildee.Select(_1 => _1.ID).ToArray());
        }

        [TestMethod()]
        public void StateMachine_Samek()
        {
            var Samek = m_Builder.StateMachine();
            var Samek_Init = m_Builder.Initial(Samek);
            var S = m_Builder.Composite(Samek);
            var S_Init = m_Builder.Initial(S);
            var S1 = m_Builder.Composite(S);
            var S1_Init = m_Builder.Initial(S1);
            var S11 = m_Builder.Simple(S1);
            var S2 = m_Builder.Composite(S);
            var S2_Init = m_Builder.Initial(S2);
            var S21 = m_Builder.Composite(S2);
            var S21_Init = m_Builder.Initial(S21);
            var S211 = m_Builder.Composite(S21);
            var Samek_Term = m_Builder.Terminal(Samek);

            Assert.AreEqual(13, Samek.Buildee.ToArray().Length);

            CollectionAssert.AreEqual(new AbstractStates.State[] {
                Samek, Samek_Init, S, S_Init, S1, S1_Init, S11, S2, S2_Init, S21, S21_Init, S211, Samek_Term
            }, Samek.Buildee.ToArray());

            CollectionAssert.AreEqual(new [] {
                Builder.DefaultIDMinValue + 0, 
                Builder.DefaultIDMinValue + 1, 
                Builder.DefaultIDMinValue + 2, 
                Builder.DefaultIDMinValue + 3, 
                Builder.DefaultIDMinValue + 4, 
                Builder.DefaultIDMinValue + 5, 
                Builder.DefaultIDMinValue + 6, 
                Builder.DefaultIDMinValue + 7, 
                Builder.DefaultIDMinValue + 8, 
                Builder.DefaultIDMinValue + 9, 
                Builder.DefaultIDMinValue + 10, 
                Builder.DefaultIDMinValue + 11, 
                Builder.DefaultIDMinValue + 12
            }, Samek.Buildee.Select(_1 => _1.ID).ToArray());
        }

        class TransitionData {
            public Int32 Trigger { get; set; }
            public Action<IMessage> Action { get; set; }
            public Predicate<Event> Guard { get; set; }
            public Int32 ID { get; set; }
            public TransitionKind Kind { get; set; }
            public AbstractStates.State Target { get; set; }
        }

        class TransitionComparer : IComparer {

            #region IComparer Members

            public int Compare(object x, object y)
            {
                TransitionData tX = x as TransitionData;
                TransitionData tY = y as TransitionData;
                Assert.IsNotNull(tX);
                Assert.IsNotNull(tY);
                Assert.AreEqual(tX.Guard, tY.Guard);
                Assert.AreEqual(tX.ID, tY.ID);
                Assert.AreEqual(tX.Kind, tY.Kind);
                Assert.AreEqual(tX.Target, tY.Target);
                Assert.AreEqual(tX.Trigger, tY.Trigger);
                return 0;
            }

            #endregion
        }

        [TestMethod()]
        public void StateMachine_Test_Add_Initial_And_Simple_And_Anonymous_Transition()
        {
            var tM = m_Builder.StateMachine();
            var tM_initial = m_Builder.Initial(tM);
            Assert.AreEqual(Builder.DefaultIDMinValue + 1, tM_initial.Buildee.ID);
            Assert.AreEqual(tM_initial.Buildee.Parent, tM);
            Assert.AreEqual(tM_initial.Buildee.Transitions, null);

            var tS1 = m_Builder.Simple(tM);

            Assert.AreEqual(Builder.DefaultIDMinValue + 2, tS1.Buildee.ID);
            Assert.AreEqual(tM, tS1.Buildee.Parent);
            Assert.AreEqual(null, tS1.Buildee.Transitions);

            CollectionAssert.AreEqual(new AbstractStates.State[] {
                tM, tM_initial, tS1
            }, tM.Buildee.ToArray());

            {
                var tTrans = m_Builder.AddTransition(tM_initial, tS1);
                Assert.AreEqual(tTrans.Buildee.Target, tS1.Buildee);
            }

            Assert.IsNotNull(tM_initial.Buildee.Transitions);
            CollectionAssert.AreEqual(new TransitionData[] {
                    new TransitionData {
                        Trigger = (Int32)Core.Constants.InternalEvents.Completion,
                        Action = null, 
                        Guard = null, 
                        ID = Builder.DefaultIDMinValue + 1, 
                        Kind = TransitionKind.Local, 
                        Target = tS1
                    }
                }, 
                tM_initial.Buildee.Transitions.Select(_1 => new TransitionData {
                    Trigger = _1.Key,
                    Action = _1.Value.Action, 
                    Guard = _1.Value.Guard, 
                    ID = _1.Value.ID, 
                    Kind = _1.Value.Kind, 
                    Target = _1.Value.Target
                }).ToArray(), new TransitionComparer());
        }

        [TestMethod()]
        public void StateMachine_Test_Add_Initial_S1_S2_With_Event0()
        {
            var tM = m_Builder.StateMachine().ID(0);
            var tM_initial = m_Builder.Initial(tM).ID(1);
            Assert.AreEqual(1, tM_initial.Buildee.ID);
            Assert.AreEqual(tM, tM_initial.Buildee.Parent);
            Assert.AreEqual(null, tM_initial.Buildee.Transitions);

            var tS1 = m_Builder.Simple(tM).ID(2);
            Assert.AreEqual(2, tS1.Buildee.ID);
            Assert.AreEqual(tM.Buildee, tS1.Buildee.Parent);
            Assert.AreEqual(null, tS1.Buildee.Transitions);

            Assert.AreEqual(3, tM.Buildee.Count());
            CollectionAssert.AreEqual(new AbstractStates.State[] {
                tM, tM_initial, tS1
            }, tM.Buildee.ToArray());

            {
                var tB = m_Builder.AddTransition(tM_initial, tS1);
                Assert.AreEqual(tS1.Buildee, tB.Buildee.Target);
                Assert.AreEqual((Int32)Core.Constants.InternalEvents.Completion,
                    tM_initial.Buildee.Transitions.First().Key);
            }

            Assert.IsNotNull(tM_initial.Buildee.Transitions);
            CollectionAssert.AreEqual(
                new TransitionData[] {
                    new TransitionData {
                        Trigger = (Int32)Core.Constants.InternalEvents.Completion,
                        Action = null, 
                        Guard = null, 
                        ID = Builder.DefaultIDMinValue + 1, 
                        Kind = TransitionKind.Local, 
                        Target = tS1
                    }
                },
                tM_initial.Buildee.Transitions
                .Select( _1 => new TransitionData {
                    Trigger = _1.Key,
                    Action = _1.Value.Action,
                    Guard = _1.Value.Guard,
                    ID = _1.Value.ID,
                    Kind = _1.Value.Kind,
                    Target = _1.Value.Target
                }).ToArray(), new TransitionComparer());
        }

    }
}
