using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using TheatreManagerApplication.Backend;

namespace TheatreManagerApplication.Classes
{
    //This is the master clas that keeps track and control of everything internally.
    public class Theatre
    {
        private string theatreName;
        private List<Play> listOfAllPlays;
        private List<Booking> listOfAllBookings;
        private List<Customer> listOfAllCustomers;
        private List<Seat> listOfAllSeats;

        //dictionaries to hold row lengths
        private Dictionary<char, int> dressRowLengthsDictionary;
        private Dictionary<char, int> upperRowLengthsDictionary;
        private Dictionary<char, int> stallsRowLengthsDictionary;

        public Theatre()
        {
            theatreName = "Jean-Paul Sartre Variety Theatre";
            listOfAllPlays = new List<Play> ();
            listOfAllBookings = new List<Booking>();
            listOfAllCustomers = new List<Customer>();
            listOfAllSeats = new List<Seat>();
            dressRowLengthsDictionary = new Dictionary<char, int>();
            upperRowLengthsDictionary = new Dictionary<char, int>(); 
            stallsRowLengthsDictionary = new Dictionary<char, int>(); 
    }

        #region TheatreInstantiators
        public void InstantiateListOfAllPlays()
        {
            int playID;
            string playName;
            string author;
            float basePriceForSeat;

            int playTypeID;
            PlayType playType;

            int performanceID;
            DateTime performanceDate;
            List<Performance> listOfPerformances;

            string getAllPerformancesOfPlayQuery = @"SELECT * FROM Performances WHERE Play_id = ";
            string getAllPlaysQuery = @"SELECT * FROM Plays;";
            SQLiteDataReader playReader = DBManager.DoQuery_Read(getAllPlaysQuery);

            while (playReader.Read())
            {
                //getting play data
                playID = (int)(Int64)playReader["Play_id"];
                playTypeID = (int)(Int64)playReader["Play_type_id"];
                playName = (string)playReader["Play_name"];
                author = (string)playReader["Author"];
                basePriceForSeat = (float)(double)playReader["Base_price"];

                playType = Play.PlayTypeIdToPlayTypeConverter(playTypeID);

                //initializing their respective lists of performances
                listOfPerformances = new List<Performance>();
                SQLiteDataReader performanceReader =
                    DBManager.DoQuery_Read(getAllPerformancesOfPlayQuery + playID.ToString() + ";");
                while (performanceReader.Read())
                {
                    performanceID = (int)(Int64)performanceReader["Performance_id"];
                    performanceDate = (DateTime)performanceReader["Date"];
                    playID = (int)(Int64)performanceReader["Play_id"];
                    listOfPerformances.Add(new Performance(performanceID, performanceDate, playID));
                }

                listOfAllPlays.Add(new Play(playID, playName, playType, author, basePriceForSeat, listOfPerformances));
            }
        }

        public void InstantiateListOfAllBookings()
        {
            //variables to hold data new Booking will be created from
            int bookingID;
            Performance performance;
            List<Seat> bookedSeatsList;
            BookingState bookingState;
            Customer customer;
            bool isPaid;

            //variables to hold Performance information
            int performanceID;
            int playIDformPerformance;
            DateTime performanceDate;

            //variables to hold Seat information
            int seatID;
            char seatRow;
            int seatNumber;
            SeatType seatType;
            bool isUsable;
            int price = 0;
            List<Seat> listOfBookedSeats = new List<Seat>();

            //variables to hold BookingState information
            int bookingStateID;

            //variables to hold Customer information
            int customerID;
            string customerName;
            string customerContactEmail;
            bool customerIsGoldClubMember;
            List<Play> listOfPlaysSeen = new List<Play>();
            DateTime membershipStartDate;
            DateTime membershipLastRenewed;

            //variables to hold query information
            string getAllBookingsQuery = @"SELECT * FROM Bookings";
            SQLiteDataReader bookingsReader = DBManager.DoQuery_Read(getAllBookingsQuery);
            string getPerformanceQuery = @"SELECT * FROM Performances WHERE Performance_id = ";
            string getReservedSeatsQuery = @"SELECT * FROM ReservedSeats WHERE Booking_id = ";
            string getCustomerQuery = @"SELECT * FROM Customers WHERE Customer_id = ";


            while (bookingsReader.Read())
            {
                //getting raw booking data
                bookingID = (int)(Int64)bookingsReader["Booking_id"];
                performanceID = (int)(Int64)bookingsReader["Performance_id"];
                //playIDformPerformance = (int)(Int64)bookingsReader["Play_id"];
                customerID = (int)(Int64)bookingsReader["Customer_id"];
                bookingStateID = (int)(Int64)bookingsReader["Booking_state"];
                isPaid = (bool)bookingsReader["isPaid"];

                //converting raw data to appropriate data types for Booking object construction
                bookingState = Booking.BookingStateIdToBookingStateConvertor(bookingStateID);

                //creating Performance based on ID
                SQLiteDataReader performanceReader =
                    DBManager.DoQuery_Read(getPerformanceQuery + performanceID.ToString());
                if (performanceReader.Read())
                {
                    performanceDate = (DateTime)performanceReader["Date"];
                    performance = new Performance(performanceID, performanceDate, 0);
                }
                else
                {
                    throw new Exception("Failed reading Performance from Database in Instantiating Bookings List.");
                }

                //creating Customer based on ID
                SQLiteDataReader customerReader =
                    DBManager.DoQuery_Read(getCustomerQuery + customerID.ToString());
                if (customerReader.Read())
                {
                    customerName = (string)customerReader["Customer_name"];
                    customerContactEmail = (string)customerReader["Contact_email"];
                    customerIsGoldClubMember = (bool)customerReader["isGoldClubMember"];
                    if (!customerIsGoldClubMember)
                    {
                        customer = new Customer(customerName, customerContactEmail,
                            customerID, customerIsGoldClubMember);
                    }
                    else
                    {
                        //SQLiteDataReader gcMemberPlaysSeenReader = new SQLiteDataReader(getGCMemberPlaysSeenListFirstQuery);
                        //while (gcMemberPlaysSeenReader.Read())
                        //{
                        //    /*FILL THIS IN AFTER REFACTORING THE METHODS IN THIS MONSTER
                        //    USE InstantiateListOfAllPlays AS BASE FOR THAT.
                        //    listOfPlaysSeen.Add();
                            
                        //    CAN BE HELPFUL:
                        //    string getGCMemberPlaysSeenListFirstQuery = @"SELECT Performance_id FROM Bookings WHERE Customer_id = ";
                        //    string getGCMemberPlaysSeenListSecondQuery = @"SELECT Play_id FROM Performances WHERE Performance_id = ";
                        //    */
                        //}
                        membershipStartDate = (DateTime)customerReader["Membership_start_date"];
                        membershipLastRenewed = (DateTime)customerReader["Membership_last_renewed"];
                        customer = new GoldClubMember(customerName, customerContactEmail, customerID,
                            listOfPlaysSeen, membershipStartDate, membershipLastRenewed);
                    }
                }
                else
                {
                    throw new Exception("Failed reading Customer from Database in Instantiating Bookings List.");
                }

                //populating listOfBookedSeats based on ID
                SQLiteDataReader bookedSeatsReader =
                    DBManager.DoQuery_Read(getCustomerQuery + customerID.ToString());
                while (bookedSeatsReader.Read())
                {
                    seatID = (int)(Int64)bookedSeatsReader["seat_id"];
                    seatRow = Seat.GetSeatRowFromString((string)bookedSeatsReader["seatRowCol"]);
                    seatNumber = Seat.GetSeatNumberFromString((string)bookedSeatsReader["seatRowCol"]);
                    seatType = Seat.GetSeatTypeFromString((string)bookedSeatsReader["seatRowCol"]);
                    isUsable = (bool)bookedSeatsReader["isUsable"];
                    listOfBookedSeats.Add(new Seat(seatID, seatRow, seatNumber, seatType, isUsable));
                    price = (int)(Int64)bookedSeatsReader["Price"];
                }
                if(performanceDate.Add(new TimeSpan(6,0,0,0,0)) < DateTime.Now)
                {
                    bookingState = BookingState.Cancelled;
                }
                listOfAllBookings.Add(
                    new Booking(bookingID, performance, listOfBookedSeats, bookingState, customer, isPaid, price));
            }
        }

        public void InstantiateListOfAllCustomers()
        {
            Customer customer;
            int customerID;
            string customerName;
            string customerContactEmail;
            bool isGoldClubMember;
            List<Play> listOfPlaysSeen = new List<Play>();
            DateTime membershipStartDate;
            DateTime membershipLastRenewed;

            string getCustomerQuery = @"SELECT * FROM Customers;";
            SQLiteDataReader customerReader =
                    DBManager.DoQuery_Read(getCustomerQuery);

            while (customerReader.Read())
            {
                customerID = (int)(Int64)customerReader["Customer_id"];
                customerName = (string)customerReader["Customer_name"];
                customerContactEmail = (string)customerReader["Contact_email"];
                membershipStartDate = (DateTime)customerReader["Membership_start_date"];
                membershipLastRenewed = (DateTime)customerReader["Membership_last_renewed_date"];
                isGoldClubMember = (bool)customerReader["isGoldClubMember"];
                if (isGoldClubMember)
                {
                    if (membershipLastRenewed.Add(new TimeSpan(364,0,0,0,0)) < DateTime.Now)
                    {
                        isGoldClubMember = false;
                        Manager.Customer_Delete(customerID);
                        Manager.Customer_Create(customerName, customerContactEmail, isGoldClubMember);
                    }
                }
                else
                {
                    //SQLiteDataReader gcMemberPlaysSeenReader = new SQLiteDataReader(getGCMemberPlaysSeenListFirstQuery);
                    //while (gcMemberPlaysSeenReader.Read())
                    //{
                    //    /*FILL THIS IN AFTER REFACTORING THE METHODS IN THIS MONSTER
                    //    USE InstantiateListOfAllPlays AS BASE FOR THAT.
                    //    listOfPlaysSeen.Add();

                    //    CAN BE HELPFUL:
                    //    string getGCMemberPlaysSeenListFirstQuery = @"SELECT Performance_id FROM Bookings WHERE Customer_id = ";
                    //    string getGCMemberPlaysSeenListSecondQuery = @"SELECT Play_id FROM Performances WHERE Performance_id = ";
                    //    */
                    //}
                    //membershipStartDate = (DateTime)customerReader["Membership_start_date"];
                    //membershipLastRenewed = (DateTime)customerReader["Membership_last_renewed"];
                    //customer = new GoldClubMember(customerName, customerContactEmail, customerID,
                    //    listOfPlaysSeen, membershipStartDate, membershipLastRenewed);
                }
               
                    }
        }

        public void InstantiateListOfAllSeats()
        {
            int seatID;
            char seatRow;
            int seatNumber;
            SeatType seatType;
            bool isUsable;

            string sql = @"SELECT * FROM Seats";
            SQLiteDataReader reader = DBManager.DoQuery_Read(sql);

            while (reader.Read())
            {
                seatID = (int)(Int64)reader["seat_ID"];
                seatRow = Seat.GetSeatRowFromString((string)reader["seatRowCol"]);
                seatNumber = Seat.GetSeatNumberFromString((string)reader["seatRowCol"]);
                seatType = Seat.GetSeatTypeFromString((string)reader["seatRowCol"]);
                isUsable = (bool)reader["isUsable"];
                listOfAllSeats.Add(new Seat(seatID, seatRow, seatNumber, seatType, isUsable));
            }
        }

        public void InstantiateRowLengthDictionaries()
        {
            //starting point - based on the database order
            SeatType currentSeatType = SeatType.Upper;
            char currentRow = 'E';
            int currentLength = 0;

            //variables to keep hold of the next seat in the list values
            Seat nextSeat;
            SeatType nextSeatType;
            char nextSeatRow;
            int nextSeatNumber;

            for(int i = listOfAllSeats.Count-1; i >= 0; i--)
            {
                nextSeat = listOfAllSeats[i];
                nextSeatType = nextSeat.GetSeatType();
                nextSeatRow = nextSeat.GetSeatRow();
                nextSeatNumber = nextSeat.GetSeatNumber();

                if (currentSeatType.Equals(nextSeatType))
                {
                    if (currentRow.Equals(nextSeatRow))
                    {
                        currentLength++;
                    }
                    else
                    {
                        RowLengthDictionariesSwitch(currentSeatType, currentRow, currentLength);
                        currentLength = 1;
                        currentRow = nextSeatRow;
                    }
                    if (i == 1)
                    {
                        RowLengthDictionariesSwitch(currentSeatType, currentRow, currentLength);
                    }
                }
                else
                {
                    RowLengthDictionariesSwitch(currentSeatType, currentRow, currentLength);
                    currentLength = 1;
                    currentRow = nextSeatRow;
                    currentSeatType = nextSeatType;
                }
            }
        }

        private void RowLengthDictionariesSwitch(SeatType seatType, char currentRow, int currentLength)
        {
            switch (seatType)
            {
                case SeatType.Dress:
                    this.dressRowLengthsDictionary.Add(currentRow, currentLength);
                    break;
                case SeatType.Upper:
                    this.upperRowLengthsDictionary.Add(currentRow, currentLength);
                    break;
                case SeatType.Stall:
                    this.stallsRowLengthsDictionary.Add(currentRow, currentLength);
                    break;
                default:
                    throw new Exception("Error in switch statement in InstantiateRowLengthDicitonaries in Seat class.");
            }
        }
        #endregion TheatreInstantiators

        #region TheatreAccessorsAndModifiers
        public string GetTheatreName()
        {
            return theatreName;
        }

        /// <summary>
        /// If for any reason new play could not be added, bool indicator is returned.
        /// </summary>
        /// <param name="playToAdd">Play to add to the list.</param>
        /// <returns>Boolean indicator. False mean that Play could not be added.</returns>
        public bool AddPlayToTheListOfAllPlays(Play playToAdd)
        {
            try
            {
                listOfAllPlays.Add(playToAdd);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Method that removes the specified Play from the booking. 
        /// </summary>
        /// <param name="playToRemoveID">Integer parameter specifying which Play to remove.</param>
        /// <returns>Returns boolean indicator of Play being removed or not. 
        /// Could return false if eg. wrong parameter was passed in.</returns>
        public bool RemovePlayFromTheListOfAllPlays(int playToRemoveID)
        {
            Play playToRemove = null;
            bool hasRemovedPlay = false;

            foreach (Play play in listOfAllPlays)
            {
                if (play.GetPlayID() == playToRemoveID)
                {
                    playToRemove = play;
                }
            }

            if (playToRemove != null)
            {
                listOfAllPlays.Remove(playToRemove);
                hasRemovedPlay = true;
            }

            return hasRemovedPlay;
        }


        public List<Play> GetListOfAllPlays()
        {
            return listOfAllPlays;
        }

        /// <summary>
        /// If for any reason new booking could not be added, bool indicator is returned.
        /// </summary>
        /// <param name="bookingToAdd">Booking to add to the list.</param>
        /// <returns>Boolean indicator. False mean that Booking could not be added.</returns>
        public bool AddBookingToTheListOfAllBookings(Booking bookingToAdd)
        {
            try
            {
                listOfAllBookings.Add(bookingToAdd);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Method that searches list of all bookings by given ID.
        /// </summary>
        /// <param name="bookingID">ID parameter to search by.</param>
        /// <returns>Returns Booking if found, null if nothing matched ID.</returns>
        public Booking SearchListOfAllBookingsByBookingID(int bookingID)
        {
            Booking bookingToReturn = null;
            bookingToReturn = listOfAllBookings.Find(booking => booking.GetBookingID() == bookingID);
            return bookingToReturn;
        }

        /// <summary>
        /// Method that searches list of all bookings by given Performance.
        /// </summary>
        /// <param name="performance">Performance to search for..</param>
        /// <returns>Returns List of Bookings if found, empty List<Booking> otherwise.</returns>
        public List<Booking> SearchListOfAllBookingsByPerformance(Performance performance)
        {
            List<Booking> listOfBookingsToReturn = new List<Booking>();
            listOfBookingsToReturn = listOfAllBookings.FindAll(
                booking => booking.GetPerformanceThisBookingIsFor().GetPerformanceID().Equals(performance.GetPerformanceID()));
            return listOfBookingsToReturn;
        }

        /// <summary>
        /// Method that searches list of all bookings by given Customer.
        /// </summary>
        /// <param name="customer">Customer to search for..</param>
        /// <returns>Returns List of Bookings if found, empty List<Booking> otherwise.</returns>
        public List<Booking> SearchListOfAllBookingsByCustomer(Customer customer)
        {
            List<Booking> listOfBookingsToReturn = new List<Booking>();
            listOfBookingsToReturn = listOfAllBookings.FindAll(
                booking => booking.GetCustomerWhoMadeThisBooking().GetCustomerID().Equals(customer.GetCustomerID()));
            return listOfBookingsToReturn;
        }

        /// <summary>
        /// Method that removes the specified Booking from the booking. 
        /// </summary>
        /// <param name="bookingToRemoveID">Integer parameter specifying which Booking to remove.</param>
        /// <returns>Returns boolean indicator of Booking being removed or not. 
        /// Could return false if eg. wrong parameter was passed in.</returns>
        public bool RemoveBookingFromTheListOfAllBookings(int bookingToRemoveID)
        {
            Booking bookingToRemove = null;
            bool hasRemovedBooking = false;

            foreach (Booking booking in listOfAllBookings)
            {
                if (booking.GetBookingID() == bookingToRemoveID)
                {
                    bookingToRemove = booking;
                }
            }

            if (bookingToRemove != null)
            {
                listOfAllBookings.Remove(bookingToRemove);
                hasRemovedBooking = true;
            }

            return hasRemovedBooking;
        }


        public List<Booking> GetListOfAllBookings()
        {
            return listOfAllBookings;
        }

        /// <summary>
        /// If for any reason new customer could not be added, bool indicator is returned.
        /// </summary>
        /// <param name="customerToAdd">Customer to add to the list.</param>
        /// <returns>Boolean indicator. False mean that Customer could not be added.</returns>
        public bool AddCustomerToTheListOfAllCustomers(Customer customerToAdd)
        {
            try
            {
                listOfAllCustomers.Add(customerToAdd);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<Customer> SearchListOfAllCustomersByName(string nameToSearchFor)
        {
            List<Customer> listOfFoundCustomers = new List<Customer>();
            listOfFoundCustomers = listOfAllCustomers.FindAll(customer => customer.GetName().Contains(nameToSearchFor));
            return listOfFoundCustomers;
        }

        public Customer SearchListOfAllCustomersByID(int customerID)
        {
            Customer customerToReturn = null;
            customerToReturn = listOfAllCustomers.Find(customer => customer.GetCustomerID() == customerID);
            return customerToReturn;
        }

        /// <summary>
        /// Method that removes the specified Customer from the booking. 
        /// </summary>
        /// <param name="customerToRemoveID">Integer parameter specifying which Customer to remove.</param>
        /// <returns>Returns boolean indicator of Customer being removed or not. 
        /// Could return false if eg. wrong parameter was passed in.</returns>
        public bool RemoveCustomerFromTheListOfAllCustomers(int customerToRemoveID)
        {
            Customer customerToRemove = null;
            bool hasRemovedCustomer = false;

            foreach (Customer customer in listOfAllCustomers)
            {
                if (customer.GetCustomerID() == customerToRemoveID)
                {
                    customerToRemove = customer;
                }
            }

            if (customerToRemove != null)
            {
                listOfAllCustomers.Remove(customerToRemove);
                hasRemovedCustomer = true;
            }

            return hasRemovedCustomer;
        }


        public List<Customer> GetListOfAllCustomers()
        {
            return listOfAllCustomers;
        }

        /// <summary>
        /// IF for any reason new seat could not be added, bool indicator is returned.
        /// </summary>
        /// <param name="seatToAdd">Seat to add to the list.</param>
        /// <returns>Boolean indicator. False mean that Seat could not be added.</returns>
        public bool AddSeatToTheListOfAllSeats(Seat seatToAdd)
        {
            try
            {
                listOfAllSeats.Add(seatToAdd);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Method that removes the specified Seat from the list of all seats. 
        /// </summary>
        /// <param name="seatRowColParameter">Parameter specifying which seat to remove.</param>
        /// <returns>Returns boolean indicator of seat being removed or not. 
        /// Could return not removed if eg. wrong parameter was passed in.</returns>
        public bool RemoveSeatFromTheListOfAllSeats(string seatRowColParameter)
        {
            Seat seatToRemove = null;
            bool hasRemovedSeat = false;

            foreach (Seat seat in listOfAllSeats)
            {
                if (seat.CreateSeatRowColStringFromThisSeat() == seatRowColParameter)
                {
                    seatToRemove = seat;
                }
            }

            if (seatToRemove != null)
            {
                listOfAllSeats.Remove(seatToRemove);
                hasRemovedSeat = true;
            }

            return hasRemovedSeat;
        }


        public List<Seat> GetListOfAllSeats()
        {
            return listOfAllSeats;
        }

        public Dictionary<char, int> GetDressRowLengthDictionary()
        {
            return dressRowLengthsDictionary;
        }

        public Dictionary<char, int> GetUpperRowLengthDictionary()
        {
            return upperRowLengthsDictionary;
        }

        public Dictionary<char, int> GetStallsRowLengthDictionary()
        {
            return stallsRowLengthsDictionary;
        }
        #endregion TheatreAccessorsAndModifiers
    }
}
