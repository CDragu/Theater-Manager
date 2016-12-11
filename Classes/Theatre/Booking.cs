using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheatreManagerApplication.Classes
{
    public enum BookingState
    {
        Active,
        Pending,
        Cancelled
    }

    public class Booking
    {
        private int bookingID;
        private Performance performance;
        private List<Seat> listOfBookedSeats;
        private BookingState bookingState;
        private Customer customer;
        private bool isPaid;
        private int price;

        /// <summary>
        /// Booking class constructor.
        /// </summary>
        /// <param name="inBookingID"></param>
        /// <param name="inPerformance"></param>
        /// <param name="inListOfBookedSeats"></param>
        /// <param name="inBookingState"></param>
        /// <param name="inCustomer"></param>
        /// <param name="inIsPaid"></param>
        public Booking(
            int inBookingID, 
            Performance inPerformance, 
            List<Seat> inListOfBookedSeats,
            BookingState inBookingState,
            Customer inCustomer,
            bool inIsPaid, int inPrice)
        {
            bookingID = inBookingID;
            performance = inPerformance;
            listOfBookedSeats = inListOfBookedSeats;
            bookingState = inBookingState;
            customer = inCustomer;
            isPaid = inIsPaid;
            price = inPrice;
        }

        public int GetBookingID()
        {
            return bookingID;
        }
        public int GetPriceFromBooking()
        {
            return price;
        }
        public Performance GetPerformanceThisBookingIsFor()
        {
            return performance;
        }

        public void AddSeatsToBookToThisBooking(Seat seatToAdd)
        {
            listOfBookedSeats.Add(seatToAdd);
        }

        /// <summary>
        /// Method that removes the specified Seat from the booking. 
        /// </summary>
        /// <param name="seatRowColParameter">Parameter specifying which seat to remove.</param>
        /// <returns>Returns boolean indicator of seat being removed or not. 
        /// Could return not removed if eg. wrong parameter was passed in.</returns>
        public bool RemoveSeatsThatAreBookedFromThisBooking(string seatRowColParameter)
        {
            Seat seatToRemove = null;
            bool hasRemovedSeat = false;

            foreach (Seat seat in listOfBookedSeats)
            {
                if (seat.CreateSeatRowColStringFromThisSeat() == seatRowColParameter)
                {
                    seatToRemove = seat;
                }
            }

            if (seatToRemove != null)
            {
                listOfBookedSeats.Remove(seatToRemove);
                hasRemovedSeat = true;
            }

            return hasRemovedSeat;
        }

        public List<Seat> GetSeatsThatAreBookedInThisBooking()
        {
            return listOfBookedSeats;
        }

        public void SetBookingState(BookingState newBookingState)
        {
            this.bookingState = newBookingState;
        }

        public BookingState GetBookingState()
        {
            return bookingState;
        }

        public void ChangeCustomerWhoMadeThisBooking(Customer newCustomer)
        {
            this.customer = newCustomer;
        }

        public Customer GetCustomerWhoMadeThisBooking()
        {
            return customer;
        }

        public void ChangeIsPaidStatus()
        {
            isPaid = !isPaid;
        }

        /// <summary>
        /// Method to find out if booking has been paid for.
        /// </summary>
        /// <returns>Returns true if booking has been paid for.</returns>
        public bool IsPaid()
        {
            return isPaid;
        }

        //STATIC METHOD USED DURING INSTANTIATION
        public static BookingState BookingStateIdToBookingStateConvertor(int bookingStateID)
        {
            switch (bookingStateID)
            {
                case 0:
                    return BookingState.Active;
                case 1:
                    return BookingState.Cancelled;
                case 2:
                    return BookingState.Pending;
                default:
                    throw new Exception("Wrong convertion method in BookingStateIdToBookingStateConvertor.");
            }
        }
    }
}
