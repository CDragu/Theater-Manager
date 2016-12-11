using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheatreManagerApplication.Classes;

namespace TheatreManagerUnitTests
{
    /// <summary>
    /// Summary description for TheatreRelatedUnitTests
    /// </summary>
    [TestClass]
    public class TheatreRelatedUnitTests
    {
        public static Theatre theater;
        public TheatreRelatedUnitTests()
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
            theater = new Theatre();
        }
        [TestMethod]
        public void AddCustomerFromTheListOfAllCustomersTest()
        {
           

            Customer customer1 = new Customer("Paul", "Parrot@biscuit.bite", 0, true);
            Customer customer2 = new Customer("Marry", "bird@branch.leaf", 1, true);

            theater.AddCustomerToTheListOfAllCustomers(customer1);
            theater.AddCustomerToTheListOfAllCustomers(customer2);

            List<Customer> customers = theater.GetListOfAllCustomers();

            Assert.AreEqual(customers[0], customer1);

            
        }

        [TestMethod]
        public void RemoveCustomerFromTheListOfAllCustomersTest()
        {
            

            Customer customer1 = new Customer("Paul", "Parrot@biscuit.bite", 0, true);
            Customer customer2 = new Customer("Marry", "bird@branch.leaf", 1, true);

            theater.AddCustomerToTheListOfAllCustomers(customer1);
            theater.AddCustomerToTheListOfAllCustomers(customer2);

            List<Customer> customers = theater.GetListOfAllCustomers();

            

            Assert.AreEqual(true, theater.RemoveCustomerFromTheListOfAllCustomers(0));

            Assert.AreEqual(customers[0], customer2);
        }
        [TestMethod]
        public void RemoveBookingFromTheListOfAllBookingsTest()
        {
            Performance performance = new Performance(0, DateTime.Now, 0);
            List<Seat> seats = new List<Seat>();
            Customer customer1 = new Customer("Paul", "Parrot@biscuit.bite", 0, true);
            Booking book1 = new Booking(0, performance, seats, BookingState.Active, customer1, true, 0);
            Booking book2 = new Booking(1, performance, seats, BookingState.Pending, customer1, false, 0);

            theater.AddBookingToTheListOfAllBookings(book1);
            theater.AddBookingToTheListOfAllBookings(book2);

            Assert.AreEqual(true, theater.RemoveBookingFromTheListOfAllBookings(0));

            Assert.AreEqual(book2, theater.GetListOfAllBookings()[0]);
        }
        [TestMethod]
        public void AddBookingFromTheListOfAllBookingsTest()
        {
            Performance performance = new Performance(0, DateTime.Now, 0);
            List<Seat> seats = new List<Seat>();
            Customer customer1 = new Customer("Paul", "Parrot@biscuit.bite", 0, true);
            Booking book1 = new Booking(0, performance, seats, BookingState.Active, customer1, true, 0);
            Booking book2 = new Booking(1, performance, seats, BookingState.Pending, customer1, false, 0);

            Assert.AreEqual(true, theater.AddBookingToTheListOfAllBookings(book1));
            Assert.AreEqual(true, theater.AddBookingToTheListOfAllBookings(book2));

            Assert.AreEqual(book1, theater.GetListOfAllBookings()[0]);
            Assert.AreEqual(book2, theater.GetListOfAllBookings()[1]);
        }
        [TestMethod]
        public void SearchListOfAllBookingsByBookingIDTest()
        {
            Performance performance = new Performance(0, DateTime.Now, 0);
            List<Seat> seats = new List<Seat>();
            Customer customer1 = new Customer("Paul", "Parrot@biscuit.bite", 0, true);
            Booking book1 = new Booking(0, performance, seats, BookingState.Active, customer1, true, 0);
            Booking book2 = new Booking(190, performance, seats, BookingState.Pending, customer1, false, 0);

            theater.AddBookingToTheListOfAllBookings(book1);
            theater.AddBookingToTheListOfAllBookings(book2);

            Assert.AreEqual(theater.GetListOfAllBookings()[1] , theater.SearchListOfAllBookingsByBookingID(190));
        }
        [TestCleanup()]
        public void MyTestCleanup()
        {
            theater = null;
        }
    }
}
