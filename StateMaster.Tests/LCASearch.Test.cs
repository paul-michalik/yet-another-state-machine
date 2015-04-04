using StateMaster.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StateMaster.Tests
{

    public enum States : int {
        S0 = 0,
        S1,
        S2,
        S3,
        S4,
        S5,
        S6,
        S7,
        S8,
        S9,
        S10,
        S11,
        S12,
        S13
    }

    public static class StatesExt {
        public static Int32 ToInt(this States pState)
        {
            return (Int32)pState;
        }
    }

    /// <summary>
    ///This is a test class for LCASearch_Test and is intended
    ///to contain all LCASearch_Test Unit Tests
    ///</summary>
    [TestClass()]
    public class LCASearch_Test {

        class State : StateMaster.Core.State {

            public State(Int32 pID, State pParent, StateMaster.Core.TransitionTable pTable) 
                : base(pID, pParent, pTable) {}

            public override void Enter() {}
            public override void Exit() {}
        }

        List<Dictionary<Int32, State>> m_SUT_Trees = new List<Dictionary<Int32, State>>();

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
        // Use ClassInitialize to run code before running the first test in the class
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
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestInitialize()]
        public void SetUp_SUT_Trees()
        {
            // Tree_0
            this.m_SUT_Trees.Add(new Dictionary<Int32, State>());
            this.m_SUT_Trees[0].Add(0, new State(0, null, null));
            this.m_SUT_Trees[0].Add(1, new State(1, this.m_SUT_Trees[0][0], null));
            this.m_SUT_Trees[0].Add(2, new State(2, this.m_SUT_Trees[0][1], null));
            this.m_SUT_Trees[0].Add(3, new State(3, this.m_SUT_Trees[0][1], null));
            this.m_SUT_Trees[0].Add(4, new State(4, this.m_SUT_Trees[0][0], null));

            // Tree_1
            this.m_SUT_Trees.Add(new Dictionary<Int32, State>());
            this.m_SUT_Trees[1].Add(
                States.S0.ToInt(), new State(States.S0.ToInt(), null, null));
            this.m_SUT_Trees[1].Add(
                States.S1.ToInt(), new State(States.S1.ToInt(), this.m_SUT_Trees[1][0], null));
            this.m_SUT_Trees[1].Add(
                States.S2.ToInt(), new State(States.S2.ToInt(), this.m_SUT_Trees[1][1], null));
            this.m_SUT_Trees[1].Add(
                States.S5.ToInt(), new State(States.S5.ToInt(), this.m_SUT_Trees[1][1], null));
            this.m_SUT_Trees[1].Add(
                States.S3.ToInt(), new State(States.S3.ToInt(), this.m_SUT_Trees[1][5], null));
            this.m_SUT_Trees[1].Add(
                States.S4.ToInt(), new State(States.S4.ToInt(), this.m_SUT_Trees[1][5], null));
            this.m_SUT_Trees[1].Add(
                States.S6.ToInt(), new State(States.S6.ToInt(), this.m_SUT_Trees[1][1], null));
            this.m_SUT_Trees[1].Add(
                States.S13.ToInt(), new State(States.S13.ToInt(), this.m_SUT_Trees[1][0], null));
            this.m_SUT_Trees[1].Add(
                States.S7.ToInt(), new State(States.S7.ToInt(), this.m_SUT_Trees[1][13], null));
            this.m_SUT_Trees[1].Add(
                States.S12.ToInt(), new State(States.S12.ToInt(), this.m_SUT_Trees[1][13], null));
            this.m_SUT_Trees[1].Add(
                States.S8.ToInt(), new State(States.S8.ToInt(), this.m_SUT_Trees[1][12], null));
            this.m_SUT_Trees[1].Add(
                States.S11.ToInt(), new State(States.S11.ToInt(), this.m_SUT_Trees[1][12], null));
            this.m_SUT_Trees[1].Add(
                States.S9.ToInt(), new State(States.S9.ToInt(), this.m_SUT_Trees[1][11], null));
            this.m_SUT_Trees[1].Add(
                States.S10.ToInt(), new State(States.S10.ToInt(), this.m_SUT_Trees[1][11], null));
        }

        [TestCleanup()]
        public void TearDown_SUT_Trees()
        {
            this.m_SUT_Trees = null;
        }

        /// <summary>
        ///A test for ConstructPathToRoot
        ///</summary>
        [TestMethod()]
        public void ConstructPathToRoot_Test_If_State_Is_Null()
        {
            State p_State = null; // TODO: Initialize to an appropriate value
            var tActual = LCASearch.ConstructPathToRoot(p_State);
            Assert.IsNotNull(tActual);
            Assert.AreEqual(tActual.Count, 0);
        }

        [TestMethod()]
        public void ConstructPathToRoot_Test_If_State_Is_Root()
        {
            State p_State = new State(0, null, null);
            var tActual = LCASearch.ConstructPathToRoot(p_State);
            var tExpected = new List<State> { 
                p_State 
            };
            Assert.IsNotNull(tActual);
            Assert.AreEqual(tActual.Count, tExpected.Count);
        }

        [TestMethod()]
        public void ConstructPathToRoot_Test_For_State_Tree_0()
        {
            var tActual = LCASearch.ConstructPathToRoot(this.m_SUT_Trees[0][0]);
            Assert.IsNotNull(tActual);
            CollectionAssert.AreEqual(tActual, new List<State>() {
                this.m_SUT_Trees[0][0]
            });

            tActual = LCASearch.ConstructPathToRoot(this.m_SUT_Trees[0][1]);
            Assert.IsNotNull(tActual);
            CollectionAssert.AreEqual(tActual, new List<State>() {
                this.m_SUT_Trees[0][1],
                this.m_SUT_Trees[0][0]
            });

            tActual = LCASearch.ConstructPathToRoot(this.m_SUT_Trees[0][2]);
            Assert.IsNotNull(tActual);
            CollectionAssert.AreEqual(tActual, new List<State>() {
                this.m_SUT_Trees[0][2],
                this.m_SUT_Trees[0][1],
                this.m_SUT_Trees[0][0]
            });

            tActual = LCASearch.ConstructPathToRoot(this.m_SUT_Trees[0][3]);
            Assert.IsNotNull(tActual);
            CollectionAssert.AreEqual(tActual, new List<State>() {
                this.m_SUT_Trees[0][3],
                this.m_SUT_Trees[0][1],
                this.m_SUT_Trees[0][0]
            });

            tActual = LCASearch.ConstructPathToRoot(this.m_SUT_Trees[0][4]);
            Assert.IsNotNull(tActual);
            CollectionAssert.AreEqual(tActual, new List<State>() {
                this.m_SUT_Trees[0][4],
                this.m_SUT_Trees[0][0]
            });
        }

        /// <summary>
        ///A test for Execute
        ///</summary>
        [TestMethod()]
        public void Execute_Test_For_State_Tree_0()
        {
            LCASearch.Result tResult;
            var tTree = this.m_SUT_Trees[0];
            // 0, 0 -> 0
            tResult = LCASearch.Execute(tTree[0], tTree[0]);
            Assert.AreEqual(tResult.Valid, true);
            Assert.AreEqual(tResult.LCA, tTree[0]);
            CollectionAssert.AreEqual(tResult.PathFromSourceToLCA.ToList(), new State[] {
                tTree[0]
            });
            CollectionAssert.AreEqual(tResult.PathFromLCAToTarget.ToList(), new State[] {
                tTree[0]
            });
            // 1, 0 -> 0
            tResult = LCASearch.Execute(tTree[1], tTree[0]);
            Assert.AreEqual(tResult.Valid, true);
            Assert.AreEqual(tResult.LCA, tTree[0]);
            CollectionAssert.AreEqual(tResult.PathFromSourceToLCA.ToList(), new State[] {
                tTree[1],
                tTree[0]
            });
            CollectionAssert.AreEqual(tResult.PathFromLCAToTarget.ToList(), new State[] {
                tTree[0]
            });
            // 1, 4 -> 0
            tResult = LCASearch.Execute(tTree[1], tTree[4]);
            Assert.AreEqual(tResult.Valid, true);
            Assert.AreEqual(tResult.LCA, tTree[0]);
            CollectionAssert.AreEqual(tResult.PathFromSourceToLCA.ToList(), new State[] {
                tTree[1],
                tTree[0]
            });
            CollectionAssert.AreEqual(tResult.PathFromLCAToTarget.ToList(), new State[] {
                tTree[0],
                tTree[4]
            });
            // 2, 4 -> 0
            tResult = LCASearch.Execute(tTree[2], tTree[4]);
            Assert.AreEqual(tResult.Valid, true);
            Assert.AreEqual(tResult.LCA, tTree[0]);
            CollectionAssert.AreEqual(tResult.PathFromSourceToLCA.ToList(), new State[] {
                tTree[2],
                tTree[1],
                tTree[0]
            });
            CollectionAssert.AreEqual(tResult.PathFromLCAToTarget.ToList(), new State[] {
                tTree[0],
                tTree[4]
            });

            // 4, 2 -> 0
            tResult = LCASearch.Execute(tTree[4], tTree[2]);
            Assert.AreEqual(tResult.Valid, true);
            Assert.AreEqual(tResult.LCA, tTree[0]);
            CollectionAssert.AreEqual(tResult.PathFromSourceToLCA.ToList(), new State[] {
                tTree[4],
                tTree[0]
            });
            CollectionAssert.AreEqual(tResult.PathFromLCAToTarget.ToList(), new State[] {
                tTree[0],
                tTree[1],
                tTree[2]
            });

            // 2, 3 -> 1
            tResult = LCASearch.Execute(tTree[2], tTree[3]);
            Assert.AreEqual(tResult.Valid, true);
            Assert.AreEqual(tResult.LCA, tTree[1]);
            CollectionAssert.AreEqual(tResult.PathFromSourceToLCA.ToList(), new State[] {
                tTree[2],
                tTree[1]
            });
            CollectionAssert.AreEqual(tResult.PathFromLCAToTarget.ToList(), new State[] {
                tTree[1],
                tTree[3]
            });
        }

        [TestMethod()]
        public void ConstructPathToRoot_Test_For_State_Tree_1()
        {
            var tActual = LCASearch.ConstructPathToRoot(this.m_SUT_Trees[1][0]);
            Assert.IsNotNull(tActual);
            CollectionAssert.AreEqual(tActual, new List<State>() {
                this.m_SUT_Trees[1][0]
            });

            tActual = LCASearch.ConstructPathToRoot(this.m_SUT_Trees[1][1]);
            Assert.IsNotNull(tActual);
            CollectionAssert.AreEqual(tActual, new List<State>() {
                this.m_SUT_Trees[1][1],
                this.m_SUT_Trees[1][0]
            });

            tActual = LCASearch.ConstructPathToRoot(this.m_SUT_Trees[1][3]);
            Assert.IsNotNull(tActual);
            CollectionAssert.AreEqual(tActual, new List<State>() {
                this.m_SUT_Trees[1][3],
                this.m_SUT_Trees[1][5],
                this.m_SUT_Trees[1][1],
                this.m_SUT_Trees[1][0]
            });

            tActual = LCASearch.ConstructPathToRoot(this.m_SUT_Trees[1][9]);
            Assert.IsNotNull(tActual);
            CollectionAssert.AreEqual(tActual, new List<State>() {
                this.m_SUT_Trees[1][9],
                this.m_SUT_Trees[1][11],
                this.m_SUT_Trees[1][12],
                this.m_SUT_Trees[1][13],
                this.m_SUT_Trees[1][0]
            });
        }

        [TestMethod()]
        public void Execute_Test_For_State_Tree_1()
        {
            LCASearch.Result tResult;
            var tTree = this.m_SUT_Trees[1];
            // 0, 0 -> 0
            tResult = LCASearch.Execute(tTree[0], tTree[0]);
            Assert.AreEqual(tResult.Valid, true);
            Assert.AreEqual(tResult.LCA, tTree[0]);
            CollectionAssert.AreEqual(tResult.PathFromSourceToLCA.ToList(), new State[] {
                tTree[0]
            });
            CollectionAssert.AreEqual(tResult.PathFromLCAToTarget.ToList(), new State[] {
                tTree[0]
            });

            // 1, 0 -> 0
            tResult = LCASearch.Execute(tTree[1], tTree[0]);
            Assert.AreEqual(tResult.Valid, true);
            Assert.AreEqual(tResult.LCA, tTree[0]);
            CollectionAssert.AreEqual(tResult.PathFromSourceToLCA.ToList(), new State[] {
                tTree[1],
                tTree[0]
            });
            CollectionAssert.AreEqual(tResult.PathFromLCAToTarget.ToList(), new State[] {
                tTree[0]
            });

            // 3, 0 -> 0
            tResult = LCASearch.Execute(tTree[3], tTree[0]);
            Assert.AreEqual(tResult.Valid, true);
            Assert.AreEqual(tResult.LCA, tTree[0]);
            CollectionAssert.AreEqual(tResult.PathFromSourceToLCA.ToList(), new State[] {
                tTree[3],
                tTree[5],
                tTree[1],
                tTree[0]
            });
            CollectionAssert.AreEqual(tResult.PathFromLCAToTarget.ToList(), new State[] {
                tTree[0]
            });

            // 10, 7 -> 13
            tResult = LCASearch.Execute(tTree[10], tTree[7]);
            Assert.AreEqual(tResult.Valid, true);
            Assert.AreEqual(tResult.LCA, tTree[13]);
            CollectionAssert.AreEqual(tResult.PathFromSourceToLCA.ToList(), new State[] {
                tTree[10],
                tTree[11],
                tTree[12],
                tTree[13]
            });
            CollectionAssert.AreEqual(tResult.PathFromLCAToTarget.ToList(), new State[] {
                tTree[13],
                tTree[7]
            });

            // 10, 10 -> 10
            tResult = LCASearch.Execute(tTree[10], tTree[10]);
            Assert.AreEqual(tResult.Valid, true);
            Assert.AreEqual(tResult.LCA, tTree[10]);
            CollectionAssert.AreEqual(tResult.PathFromSourceToLCA.ToList(), new State[] {
                tTree[10]
            });
            CollectionAssert.AreEqual(tResult.PathFromLCAToTarget.ToList(), new State[] {
                tTree[10]
            });
        }

        [TestMethod()]
        public void Execute_Test_For_States_From_Tree_0_And_Tree_1()
        {
            LCASearch.Result tResult;
            var tTree0 = this.m_SUT_Trees[0];
            var tTree1 = this.m_SUT_Trees[1];

            tResult = LCASearch.Execute(tTree0[4], tTree1[12]);
            Assert.AreEqual(tResult.LCA, null);
            CollectionAssert.AreEqual(tResult.PathFromSourceToLCA.ToList(), new State[] {
                tTree0[4],
                tTree0[0],
            });
            CollectionAssert.AreEqual(tResult.PathFromLCAToTarget.ToList(), new State[] {
                tTree1[0],
                tTree1[13],
                tTree1[12]
            });
        }
    }
}
