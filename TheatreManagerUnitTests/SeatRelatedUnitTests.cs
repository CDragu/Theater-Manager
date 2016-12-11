using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheatreManagerApplication.Classes;


namespace TheatreManagerUnitTests
{
    /// <summary>
    /// Summary description for SeatRelatedUnitTests
    /// </summary>
    [TestClass]
    public class SeatRelatedUnitTests
    {
        public static Seat seat;
        public SeatRelatedUnitTests()
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
        [TestInitialize()]
        public void MyTestInitialize() {
            seat = new Seat(10, 'A', 10, SeatType.Dress, true);
        }
        [TestMethod]
        public void SeatIsUsableTest()
        {
            seat = new Seat(10, 'A', 10, SeatType.Upper, true);
            Assert.AreEqual(true,seat.IsUsable());

            seat = new Seat(10, 'A', 10, SeatType.Upper, false);
            Assert.AreEqual(false, seat.IsUsable());
        }

        [TestMethod]
        public void CreateSeatRowColStringFromThisSeatTest()
        {
            seat = new Seat(10, 'A', 10, SeatType.Upper, true);
            Assert.AreEqual("UA10", seat.CreateSeatRowColStringFromThisSeat());

            seat = new Seat(10, 'X', 20, SeatType.Dress, true);
            Assert.AreEqual("DX20", seat.CreateSeatRowColStringFromThisSeat());

            seat = new Seat(10, 'B', 5, SeatType.Stall, true);
            Assert.AreEqual("SB05", seat.CreateSeatRowColStringFromThisSeat());
        }

        [TestMethod]
        public void InitializeSeatTest()
        {
            seat = new Seat(10, 'A', 10, SeatType.Upper, true);

            Assert.AreEqual(10, seat.GetSeatID());
            Assert.AreEqual('A', seat.GetSeatRow());
            Assert.AreEqual(10, seat.GetSeatNumber());
            Assert.AreEqual(SeatType.Upper, seat.GetSeatType());
            Assert.AreEqual(true, seat.IsUsable());
        }

        [TestMethod]
        public void CalculateSeatPriceTest()
        {

            float assumedSeatPrice;
            float actualSeatPrice;
            List<Performance> emptyList = new List<Performance>();

            Play play = new Play(1, "No Exit", PlayType.Major, "J.P. Sartre", 10.0f, emptyList);
            for (int i = 0; i < 200; i++)
            {
                actualSeatPrice = Seat.CalculateSeatPrice(play.GetBasePriceForSeat(), SeatType.Stall);
                assumedSeatPrice = (float)Math.Round((play.GetBasePriceForSeat() * 0.8f), 2);
                Assert.AreEqual(assumedSeatPrice, actualSeatPrice);

                actualSeatPrice = Seat.CalculateSeatPrice(play.GetBasePriceForSeat(), SeatType.Upper);
                assumedSeatPrice = play.GetBasePriceForSeat();
                Assert.AreEqual(assumedSeatPrice, actualSeatPrice);

                actualSeatPrice = Seat.CalculateSeatPrice(play.GetBasePriceForSeat(), SeatType.Dress);
                assumedSeatPrice = (float)Math.Round((play.GetBasePriceForSeat() * 1.2f), 2);
                Assert.AreEqual(assumedSeatPrice, actualSeatPrice);
            }
        }

        [TestMethod]
        public void CalculateThisSeatPriceTest()
        {
            float assumedSeatPrice;
            float actualSeatPrice;
            List<Performance> emptyList = new List<Performance>();

            Play play = new Play(1, "No Exit", PlayType.Major, "J.P. Sartre", 10.0f, emptyList);
            Seat cheapSeat = new Seat(10, 'A', 10, SeatType.Stall, true);
            Seat seat = new Seat(10, 'A', 10, SeatType.Upper, true);
            Seat expensiveSeat = new Seat(10, 'A', 10, SeatType.Dress, true);

            actualSeatPrice = cheapSeat.CalculateThisSeatPrice(play.GetBasePriceForSeat());
            assumedSeatPrice = (float)Math.Round((play.GetBasePriceForSeat() * 0.8f), 2);
            Assert.AreEqual(assumedSeatPrice, actualSeatPrice);

            actualSeatPrice = seat.CalculateThisSeatPrice(play.GetBasePriceForSeat());
            assumedSeatPrice = play.GetBasePriceForSeat();
            Assert.AreEqual(assumedSeatPrice, actualSeatPrice);

            actualSeatPrice = expensiveSeat.CalculateThisSeatPrice(play.GetBasePriceForSeat());
            assumedSeatPrice = (float)Math.Round((play.GetBasePriceForSeat() * 1.2f), 2);
            Assert.AreEqual(assumedSeatPrice, actualSeatPrice);
        }

        [TestCleanup()]
        public void MyTestCleanup() {
            seat = null;
        }

    }
}
