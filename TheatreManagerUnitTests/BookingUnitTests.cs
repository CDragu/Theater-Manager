using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TheatreManagerApplication.Classes;
using System.Collections.Generic;

namespace TheatreManagerUnitTests
{
    [TestClass]
    public class BookingUnitTests
    {
        public static Booking book;
        [TestInitialize()]
        public void MyTestInitialize()
        {
            Performance performance = new Performance(0, DateTime.Now, 0);
            List<Seat> seats = new List<Seat>();
            Customer customer1 = new Customer("Paul", "Parrot@biscuit.bite", 0, true);
            book = new Booking(0, performance, seats, BookingState.Active, customer1, true, 10);
            
        }
        [TestMethod]
        public void RemoveSeatsThatAreBookedFromThisBookingTest()
        {
            Seat seat1 = new Seat(0, 'A', 20, SeatType.Dress, true);
            Seat seat2 = new Seat(2, 'B', 20, SeatType.Dress, true);
            book.AddSeatsToBookToThisBooking(seat1);
            book.AddSeatsToBookToThisBooking(seat2);

            book.RemoveSeatsThatAreBookedFromThisBooking("DA20");

            Assert.AreEqual(seat2, book.GetSeatsThatAreBookedInThisBooking()[0]);

        }

        [TestMethod]
        public void AddSeatsThatAreBookedFromThisBookingTest()
        {
            Seat seat1 = new Seat(0, 'A', 20, SeatType.Dress, true);
            Seat seat2 = new Seat(2, 'B', 20, SeatType.Dress, true);
            book.AddSeatsToBookToThisBooking(seat1);
            book.AddSeatsToBookToThisBooking(seat2);

           

            Assert.AreEqual(seat2, book.GetSeatsThatAreBookedInThisBooking()[1]);

        }
        [TestCleanup()]
        public void MyTestCleanup()
        {
            book = null;
        }
    }
}
