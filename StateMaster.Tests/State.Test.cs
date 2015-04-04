using StateMaster.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using StateMaster.Core.Interfaces;

namespace StateMaster.Tests
{
    
    
    /// <summary>
    ///This is a test class for State_Test and is intended
    ///to contain all State_Test Unit Tests
    ///</summary>
    [TestClass()]
    public class State_Test {


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


        internal virtual State CreateState()
        {
            // TODO: Instantiate an appropriate concrete class.
            State target = null;
            return target;
        }

        /// <summary>
        ///A test for HandleEvent
        ///</summary>
        [TestMethod()]
        public void HandleEvent_Test()
        {
            State target = CreateState(); // TODO: Initialize to an appropriate value
            IEvent p_Event = null; // TODO: Initialize to an appropriate value
            ITransitor expected = null; // TODO: Initialize to an appropriate value
            ITransitor actual;
            actual = target.HandleEvent(p_Event);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
