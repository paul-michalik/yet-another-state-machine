using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StateMaster.AbstractStates;
using StateMaster.Extensions;

namespace StateMaster.Tests2 {
    [TestClass]
    public class Builder_LCASearch_Test {
        AbstractStates.StateMachine m_0, m_1, m_Samek;

        [TestInitialize()]
        public void SetUp_SUT_Trees()
        {
            {
                var tB = new Builder();
                var tM = tB.StateMachine().ID(States.Machine0.Machine);
                var S0 = tB.Composite(tM).ID(States.Machine0.S0);
                var S1 = tB.Composite(S0).ID(States.Machine0.S1);
                var S2 = tB.Simple(S1).ID(States.Machine0.S2);
                var S3 = tB.Simple(S1).ID(States.Machine0.S3);
                var S4 = tB.Simple(S0).ID(States.Machine0.S4);
                m_0 = tM;
            }

            {
                var tB = new Builder();
                var tM = tB.StateMachine().ID(States.Machine1.Machine);
                var S0 = tB.Composite(tM).ID(States.Machine1.S0);
                var S1 = tB.Composite(S0).ID(States.Machine1.S1);
                var S2 = tB.Simple(S1).ID(States.Machine1.S2);
                var S5 = tB.Composite(S1).ID(States.Machine1.S5);
                var S3 = tB.Simple(S5).ID(States.Machine1.S3);
                var S4 = tB.Simple(S5).ID(States.Machine1.S4);
                var S6 = tB.Simple(S1).ID(States.Machine1.S6);
                var S13 = tB.Composite(S0).ID(States.Machine1.S13);
                var S7 = tB.Simple(S13).ID(States.Machine1.S7);
                var S12 = tB.Composite(S13).ID(States.Machine1.S12);
                var S8 = tB.Simple(S12).ID(States.Machine1.S8);
                var S11 = tB.Composite(S12).ID(States.Machine1.S11);
                var S9 = tB.Simple(S11).ID(States.Machine1.S9);
                var S10 = tB.Simple(S11).ID(States.Machine1.S10);
                m_1 = tM;
            }

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
                m_Samek = Samek;
            }
        }

        void WriteLog<TID>(AbstractStates.State p_State, bool p_NewLine = true)
        {
            if (p_State != null) {
                Console.Write("{0}", ((TID)(object)p_State.ID).ToString());
                if (p_NewLine)
                    Console.Write(Environment.NewLine);
            }
        }

        void WriteLog<TID>(IEnumerable<AbstractStates.State> p_States)
        {
            p_States.ToList().ForEach(_ => WriteLog<TID>(_, false));
        }

        void WriteLog<TID>(Core.LCASearch.Result p_Result)
        {
            Console.WriteLine("LCA: {0}", ((TID)(object)p_Result.LCA.ID).ToString());
            Console.Write("PathFromSourceToLCA: ");
            WriteLog<TID>(p_Result.PathFromSourceToLCA);
            Console.Write(Environment.NewLine);
            Console.Write("PathFromLCAToTarget: ");
            WriteLog<TID>(p_Result.PathFromLCAToTarget);
            Console.Write(Environment.NewLine);
        }

        [TestCleanup()]
        public void TearDown_SUT_Trees()
        {
            this.m_0 = this.m_1 = this.m_Samek = null;
        }

        [TestMethod]
        public void Builder_LCASearch_ConstructPathToRoot_Test_If_State_Is_Null()
        {
            State p_State = null; // TODO: OnNewTransition to an appropriate value
            var tActual = Core.LCASearch.ConstructPathToRoot(p_State);
            Assert.IsNotNull(tActual);
            Assert.AreEqual(tActual.Count, 0);
        }

        [TestMethod()]
        public void Builder_LCASearch_ConstructPathToRoot_Test_If_State_Is_Root()
        {
            {
                State tS = m_0.ToDictionary(_1 => (States.Machine0)_1.ID)[States.Machine0.Machine];

                var tActual = Core.LCASearch.ConstructPathToRoot(tS);
                var tExpected = new List<State> {
                    tS 
                };
                Assert.IsNotNull(tActual);
                Assert.AreEqual(tExpected.Count, tActual.Count);

                // StatesExtensions
                CollectionAssert.AreEqual(tExpected, tS.PathToParent().ToList());
            }

            {
                State tS = m_1.ToDictionary(_1 => (States.Machine1)_1.ID)[States.Machine1.Machine];

                var tActual = Core.LCASearch.ConstructPathToRoot(tS);
                var tExpected = new List<State> {
                    tS 
                };
                Assert.IsNotNull(tActual);
                Assert.AreEqual(tExpected.Count, tActual.Count);

                // StatesExtensions
                CollectionAssert.AreEqual(tExpected, tS.PathToParent().ToList());
            }

            {
                State tS = m_Samek.ToDictionary(_1 => (States.Samek)_1.ID)[States.Samek.Samek];

                var tActual = Core.LCASearch.ConstructPathToRoot(tS);
                var tExpected = new List<State> {
                    tS 
                };
                Assert.IsNotNull(tActual);
                Assert.AreEqual(tExpected.Count, tActual.Count);

                // StatesExtensions
                CollectionAssert.AreEqual(tExpected, tS.PathToParent().ToList());
            }
        }

        [TestMethod()]
        public void Builder_LCASearch_ConstructPathToRoot_Test_For_State_Tree_1()
        {
            var tStates = m_1.ToDictionary(_1 => (States.Machine1)_1.ID);
            var tActual = Core.LCASearch.ConstructPathToRoot(tStates[States.Machine1.S2]);
            var tExpected = new List<State>() {
                tStates[States.Machine1.S2],
                tStates[States.Machine1.S1],
                tStates[States.Machine1.S0],
                tStates[States.Machine1.Machine]
            };
            Assert.IsNotNull(tActual);
            CollectionAssert.AreEqual(tExpected, tActual);
            // StatesExtensions
            WriteLog<States.Machine1>(tStates[States.Machine1.S2].PathToParent());
            CollectionAssert.AreEqual(tExpected, tStates[States.Machine1.S2].PathToParent().ToList());


            tActual = Core.LCASearch.ConstructPathToRoot(tStates[States.Machine1.Machine]);
            tExpected = new List<State>() {
                tStates[States.Machine1.Machine]
            };
            Assert.IsNotNull(tActual);
            CollectionAssert.AreEqual(tExpected, tActual);
            // StatesExtensions
            WriteLog<States.Machine1>(tStates[States.Machine1.Machine].PathToParent());
            CollectionAssert.AreEqual(tExpected, tStates[States.Machine1.Machine].PathToParent().ToList());

            tActual = Core.LCASearch.ConstructPathToRoot(tStates[States.Machine1.S10]);
            tExpected = new List<State>() {
                tStates[States.Machine1.S10],
                tStates[States.Machine1.S11],
                tStates[States.Machine1.S12],
                tStates[States.Machine1.S13],
                tStates[States.Machine1.S0],
                tStates[States.Machine1.Machine]
            };
            Assert.IsNotNull(tActual);
            CollectionAssert.AreEqual(tExpected, tActual);
            // StatesExtensions
            WriteLog<States.Machine1>(tStates[States.Machine1.S10].PathToParent());
            CollectionAssert.AreEqual(tExpected, tStates[States.Machine1.S10].PathToParent().ToList());
        }

        [TestMethod()]
        public void Builder_LCASearch_Execute_Test_For_State_Tree_0()
        {
            Core.LCASearch.Result tResult;
            AbstractStates.State tLCA = null;
            var tTree = m_0.ToDictionary(_1 => (States.Machine0)_1.ID);
            // 0, 0 -> 0
            tResult = Core.LCASearch.Execute(tTree[States.Machine0.S0], tTree[States.Machine0.S0]);
            WriteLog<States.Machine0>(tResult);
            Assert.AreEqual(true, tResult.Valid);
            Assert.AreEqual(tTree[States.Machine0.S0], tResult.LCA);
            CollectionAssert.AreEqual(new State[] {
                tTree[States.Machine0.S0]
            }, tResult.PathFromSourceToLCA.ToList());
            CollectionAssert.AreEqual(new State[] {
                tTree[States.Machine0.S0]
            }, tResult.PathFromLCAToTarget.ToList());
            // StateExtensions
            tLCA = Core.LCASearch.FindLCA(tTree[States.Machine0.S0], tTree[States.Machine0.S0]);
            WriteLog<States.Machine0>(tLCA);
            Assert.AreEqual(tTree[States.Machine0.S0], tLCA);


            // 1, 0 -> 0
            tResult = Core.LCASearch.Execute(tTree[States.Machine0.S1], tTree[States.Machine0.S0]);
            WriteLog<States.Machine0>(tResult);
            Assert.AreEqual(true, tResult.Valid);
            Assert.AreEqual(tTree[States.Machine0.S0], tResult.LCA);
            CollectionAssert.AreEqual(tResult.PathFromSourceToLCA.ToList(), new State[] {
                tTree[States.Machine0.S1],
                tTree[States.Machine0.S0]
            });
            CollectionAssert.AreEqual(tResult.PathFromLCAToTarget.ToList(), new State[] {
                tTree[States.Machine0.S0]
            });
            // StateExtensions
            tLCA = Core.LCASearch.FindLCA(tTree[States.Machine0.S1], tTree[States.Machine0.S0]);
            WriteLog<States.Machine0>(tLCA);
            Assert.AreEqual(tTree[States.Machine0.S0], tLCA);

            // 1, 4 -> 0
            tResult = Core.LCASearch.Execute(tTree[States.Machine0.S1], tTree[States.Machine0.S4]);
            WriteLog<States.Machine0>(tResult);
            Assert.AreEqual(tResult.Valid, true);
            Assert.AreEqual(tResult.LCA, tTree[States.Machine0.S0]);
            CollectionAssert.AreEqual(tResult.PathFromSourceToLCA.ToList(), new State[] {
                tTree[States.Machine0.S1],
                tTree[States.Machine0.S0]
            });
            CollectionAssert.AreEqual(tResult.PathFromLCAToTarget.ToList(), new State[] {
                tTree[States.Machine0.S0],
                tTree[States.Machine0.S4]
            });
            // StateExtensions
            tLCA = Core.LCASearch.FindLCA(tTree[States.Machine0.S1], tTree[States.Machine0.S4]);
            WriteLog<States.Machine0>(tLCA);
            Assert.AreEqual(tTree[States.Machine0.S0], tLCA);
            // StateExtensions
            tLCA = Core.LCASearch.FindLCA(tTree[States.Machine0.S4], tTree[States.Machine0.S1]);
            WriteLog<States.Machine0>(tLCA);
            Assert.AreEqual(tTree[States.Machine0.S0], tLCA);

            // 2, 4 -> 0
            tResult = Core.LCASearch.Execute(tTree[States.Machine0.S2], tTree[States.Machine0.S4]);
            WriteLog<States.Machine0>(tResult);
            Assert.AreEqual(tResult.Valid, true);
            Assert.AreEqual(tResult.LCA, tTree[States.Machine0.S0]);
            CollectionAssert.AreEqual(tResult.PathFromSourceToLCA.ToList(), new State[] {
                tTree[States.Machine0.S2],
                tTree[States.Machine0.S1],
                tTree[States.Machine0.S0]
            });
            CollectionAssert.AreEqual(tResult.PathFromLCAToTarget.ToList(), new State[] {
                tTree[States.Machine0.S0],
                tTree[States.Machine0.S4]
            });
            // StateExtensions
            tLCA = Core.LCASearch.FindLCA(tTree[States.Machine0.S2], tTree[States.Machine0.S4]);
            WriteLog<States.Machine0>(tLCA);
            Assert.AreEqual(tTree[States.Machine0.S0], tLCA);
            // StateExtensions
            tLCA = Core.LCASearch.FindLCA(tTree[States.Machine0.S4], tTree[States.Machine0.S2]);
            WriteLog<States.Machine0>(tLCA);
            Assert.AreEqual(tTree[States.Machine0.S0], tLCA);

            // 4, 2 -> 0
            tResult = Core.LCASearch.Execute(tTree[States.Machine0.S4], tTree[States.Machine0.S2]);
            WriteLog<States.Machine0>(tResult);
            Assert.AreEqual(tResult.Valid, true);
            Assert.AreEqual(tResult.LCA, tTree[States.Machine0.S0]);
            CollectionAssert.AreEqual(tResult.PathFromSourceToLCA.ToList(), new State[] {
                tTree[States.Machine0.S4],
                tTree[States.Machine0.S0]
            });
            CollectionAssert.AreEqual(tResult.PathFromLCAToTarget.ToList(), new State[] {
                tTree[States.Machine0.S0],
                tTree[States.Machine0.S1],
                tTree[States.Machine0.S2]
            });

            // 2, 3 -> 1
            tResult = Core.LCASearch.Execute(tTree[States.Machine0.S2], tTree[States.Machine0.S3]);
            WriteLog<States.Machine0>(tResult);
            Assert.AreEqual(tResult.Valid, true);
            Assert.AreEqual(tResult.LCA, tTree[States.Machine0.S1]);
            CollectionAssert.AreEqual(tResult.PathFromSourceToLCA.ToList(), new State[] {
                tTree[States.Machine0.S2],
                tTree[States.Machine0.S1]
            });
            CollectionAssert.AreEqual(tResult.PathFromLCAToTarget.ToList(), new State[] {
                tTree[States.Machine0.S1],
                tTree[States.Machine0.S3]
            });
            // StateExtensions
            tLCA = Core.LCASearch.FindLCA(tTree[States.Machine0.S2], tTree[States.Machine0.S3]);
            WriteLog<States.Machine0>(tLCA);
            Assert.AreEqual(tTree[States.Machine0.S1], tLCA);
            // StateExtensions
            tLCA = Core.LCASearch.FindLCA(tTree[States.Machine0.S3], tTree[States.Machine0.S2]);
            WriteLog<States.Machine0>(tLCA);
            Assert.AreEqual(tTree[States.Machine0.S1], tLCA);
        }

        [TestMethod()]
        public void Builder_LCASearch_Execute_Test_For_State_Tree_1()
        {
            Core.LCASearch.Result tResult;
            AbstractStates.State tLCA = null;
            var tTree = m_1.ToDictionary(_1 => (States.Machine1)_1.ID);
            // 0, 0 -> 0
            tResult = Core.LCASearch.Execute(tTree[States.Machine1.S0], tTree[States.Machine1.S0]);
            WriteLog<States.Machine1>(tResult);
            Assert.AreEqual(tResult.Valid, true);
            Assert.AreEqual(tTree[States.Machine1.S0], tResult.LCA);
            CollectionAssert.AreEqual(new State[] {
                tTree[States.Machine1.S0]
            }, tResult.PathFromSourceToLCA.ToList());
            CollectionAssert.AreEqual(new State[] {
                tTree[States.Machine1.S0]
            }, tResult.PathFromLCAToTarget.ToList());
            // StateExtensions
            tLCA = Core.LCASearch.FindLCA(tTree[States.Machine1.S0], tTree[States.Machine1.S0]);
            Assert.AreEqual(tTree[States.Machine1.S0], tLCA);

            // 3, 6 -> 1
            tResult = Core.LCASearch.Execute(tTree[States.Machine1.S3], tTree[States.Machine1.S6]);
            WriteLog<States.Machine1>(tResult);
            Assert.AreEqual(tResult.Valid, true);
            Assert.AreEqual(tTree[States.Machine1.S1], tResult.LCA);
            CollectionAssert.AreEqual(new State[] {
                tTree[States.Machine1.S3],
                tTree[States.Machine1.S5],
                tTree[States.Machine1.S1]
            }, tResult.PathFromSourceToLCA.ToList());
            CollectionAssert.AreEqual(new State[] {
                tTree[States.Machine1.S1],
                tTree[States.Machine1.S6]
            }, tResult.PathFromLCAToTarget.ToList());
            // StateExtensions
            tLCA = Core.LCASearch.FindLCA(tTree[States.Machine1.S3], tTree[States.Machine1.S6]);
            Assert.AreEqual(tTree[States.Machine1.S1], tLCA);
            tLCA = Core.LCASearch.FindLCA(tTree[States.Machine1.S6], tTree[States.Machine1.S3]);
            Assert.AreEqual(tTree[States.Machine1.S1], tLCA);

            // 6, 3 -> 1
            tResult = Core.LCASearch.Execute(tTree[States.Machine1.S6], tTree[States.Machine1.S3]);
            WriteLog<States.Machine1>(tResult);
            Assert.AreEqual(tResult.Valid, true);
            Assert.AreEqual(tTree[States.Machine1.S1], tResult.LCA);
            CollectionAssert.AreEqual(new State[] {
                tTree[States.Machine1.S6],
                tTree[States.Machine1.S1]
            }, tResult.PathFromSourceToLCA.ToList());
            CollectionAssert.AreEqual(new State[] {
                tTree[States.Machine1.S1],
                tTree[States.Machine1.S5],
                tTree[States.Machine1.S3]
            }, tResult.PathFromLCAToTarget.ToList());
        }

        [TestMethod()]
        public void Builder_LCASearch_Execute_Test_For_Samek()
        {
            Core.LCASearch.Result tResult;
            AbstractStates.State tLCA = null;
            var tTree = m_Samek.ToDictionary(_1 => (States.Samek)_1.ID);
            // Samek_Init, S11 -> Samek
            tResult = Core.LCASearch.Execute(tTree[States.Samek.Samek_Init], tTree[States.Samek.S11]);
            WriteLog<States.Samek>(tResult);
            Assert.AreEqual(tResult.Valid, true);
            Assert.AreEqual(tTree[States.Samek.Samek], tResult.LCA);

            CollectionAssert.AreEqual(new State[] {
                tTree[States.Samek.Samek_Init],
                tTree[States.Samek.Samek]
            }, tResult.PathFromSourceToLCA.ToList());
            CollectionAssert.AreEqual(new State[] {
                tTree[States.Samek.Samek],
                tTree[States.Samek.S],
                tTree[States.Samek.S1],
                tTree[States.Samek.S11]
            }, tResult.PathFromLCAToTarget.ToList());

            // StateExtensions
            tLCA = Core.LCASearch.FindLCA(tTree[States.Samek.Samek_Init], tTree[States.Samek.S11]);
            Assert.AreEqual(tTree[States.Samek.Samek], tLCA);
            tLCA = Core.LCASearch.FindLCA(tTree[States.Samek.S11], tTree[States.Samek.Samek_Init]);
            Assert.AreEqual(tTree[States.Samek.Samek], tLCA);

            // S11 -> S2
            tResult = Core.LCASearch.Execute(tTree[States.Samek.S11], tTree[States.Samek.S2]);
            WriteLog<States.Samek>(tResult);
            Assert.AreEqual(tResult.Valid, true);
            Assert.AreEqual(tTree[States.Samek.S], tResult.LCA);
            CollectionAssert.AreEqual(new State[] {
                tTree[States.Samek.S11],
                tTree[States.Samek.S1],
                tTree[States.Samek.S]
            }, tResult.PathFromSourceToLCA.ToList());
            CollectionAssert.AreEqual(new State[] {
                tTree[States.Samek.S],
                tTree[States.Samek.S2]
            }, tResult.PathFromLCAToTarget.ToList());
            // StateExtensions
            tLCA = Core.LCASearch.FindLCA(tTree[States.Samek.S11], tTree[States.Samek.S2]);
            Assert.AreEqual(tTree[States.Samek.S], tLCA);
            tLCA = Core.LCASearch.FindLCA(tTree[States.Samek.S2], tTree[States.Samek.S11]);
            Assert.AreEqual(tTree[States.Samek.S], tLCA);

            // S2 -> S11
            tResult = Core.LCASearch.Execute(tTree[States.Samek.S2], tTree[States.Samek.S11]);
            WriteLog<States.Samek>(tResult);
            Assert.AreEqual(tResult.Valid, true);
            Assert.AreEqual(tTree[States.Samek.S], tResult.LCA);
            CollectionAssert.AreEqual(new State[] {
                tTree[States.Samek.S2],
                tTree[States.Samek.S]
            }, tResult.PathFromSourceToLCA.ToList());
            CollectionAssert.AreEqual(new State[] {
                tTree[States.Samek.S],
                tTree[States.Samek.S1],
                tTree[States.Samek.S11]
            }, tResult.PathFromLCAToTarget.ToList());
        }
    }
}
