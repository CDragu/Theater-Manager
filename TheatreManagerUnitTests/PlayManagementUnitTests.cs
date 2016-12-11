using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheatreManagerApplication.Classes;

namespace TheatreManagerUnitTests
{
    /// <summary>
    /// Summary description for PlayManagementUnitTests
    /// </summary>
    [TestClass]
    public class PlayManagementUnitTests
    {
        public static Play play;
        public static List<Performance> inListOfPerformances;
        public PlayManagementUnitTests()
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
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            play = new Play(0, "Paul", PlayType.Major, "Mike", 10f, inListOfPerformances);
            inListOfPerformances = new List<Performance>();
            
        }

        [TestMethod]
        public void AddPerformanceToTheListOfPerformancesTest()
        {
            play = new Play(0, "Paul", PlayType.Major, "Mike", 10f, inListOfPerformances);
            Performance performance = new Performance(1, DateTime.Now, 0);
            Assert.AreEqual(true, play.AddPerformanceToTheListOfPerformances(performance));

            Assert.AreEqual(performance, inListOfPerformances[0]);
        }

        [TestMethod]
        public void ChangeAutorTest()
        {
            play = new Play(0, "Paul", PlayType.Major, "Mike", 10f, inListOfPerformances);
            play.ChangeAuthor("KOKO");

            Assert.AreEqual("KOKO", play.GetAuthor());
        }

        [TestMethod]
        public void ChangeBasePriceForSeatTest()
        {
            play = new Play(0, "Paul", PlayType.Major, "Mike", 10f, inListOfPerformances);
            play.ChangeBasePriceForSeat(12f);

            Assert.AreEqual(12f, play.GetBasePriceForSeat());
        }

        [TestMethod]
        public void ChangePlayNameTest()
        {
            play = new Play(0, "Paul", PlayType.Major, "Mike", 10f, inListOfPerformances);
            play.ChangePlayName("Balad");

            Assert.AreEqual("Balad", play.GetPlayName());
        }

        [TestMethod]
        public void RemovePerformanceFromTheListOfPerformancesTest()
        {
            inListOfPerformances = new List<Performance>();
            play = new Play(0, "Paul", PlayType.Major, "Mike", 10f, inListOfPerformances);
            Performance performance = new Performance(0, DateTime.Now, 0);
            Performance performance2 = new Performance(1, DateTime.Now, 0);

            Assert.AreEqual(true, play.AddPerformanceToTheListOfPerformances(performance));
            Assert.AreEqual(true, play.AddPerformanceToTheListOfPerformances(performance2));

            play.RemovePerformanceFromTheListOfPerformances(0);

            if(inListOfPerformances[0] == performance)
            {
                Assert.Fail();
            }
        }

        [TestCleanup()]
        public void MyTestCleanup()
        {
            play = null;
            inListOfPerformances = null;
        }
    }
}
