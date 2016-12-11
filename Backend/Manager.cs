using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using TheatreManagerApplication.Classes;

namespace TheatreManagerApplication.Backend
{
    public enum DBResult { Complete, AlreadyExists, TooGeneral, NotFound }
    public static class Manager
    {
        static Manager()
        {
            DBManager.Initialise();
        }

        static bool AlreadyExists(string sql)
        {
            SQLiteDataReader reader = DBManager.DoQuery_Read(sql);
            while (reader.Read())
            {
                return true;
            }
            return false;
        }

        #region Customer
        public static DBResult Customer_Create(string name, string email, bool gold)
        {
            string sql = "INSERT INTO Customers (Customer_name,Membership_start_date,Membership_last_renewed_date,"
                + "Contact_email,isGoldClubMember) VALUES('" + name + "','" + DateTime.Now.ToString() + "','" + DateTime.Now.ToString()
                + "','" + email + "','" + gold.ToString() + "');";

            DBManager.DoQuery_Write(sql);

            return DBResult.Complete;
        }

        public static int Customer_GetId(string name, string email)
        {
            string sql = "SELECT * FROM Customers WHERE Customer_name LIKE '%" + name + "%' AND Contact_email LIKE '%"
                + email + "%';";
            SQLiteDataReader reader = DBManager.DoQuery_Read(sql);
            if (reader.StepCount != 1)
                return -1;
            else
            {
                reader.Read();
                return int.Parse(reader["Customer_id"].ToString());
            }
        }

        public static Customer Customer_Get(int id)
        {
            string sql = "SELECT * FROM Customers WHERE Customer_id = '" + id + "';";
            SQLiteDataReader reader = DBManager.DoQuery_Read(sql);
            if (reader.StepCount != 1)
                return null;
            else
            {
                reader.Read();
                return new Customer((string)reader["Customer_name"],
                    (string)reader["Contact_email"], id, (bool)reader["isGoldClubMember"]);
            }
        }

        public static Customer[] Customer_Search(string name, string email)
        {
            string sql = "SELECT * FROM Customers WHERE Customer_name LIKE '%" + name + "%' AND Contact_email LIKE '%" + email + "%';";
            SQLiteDataReader reader = DBManager.DoQuery_Read(sql);
            List<Customer> customers = new List<Customer>();
            while (reader.Read())
                customers.Add(new Customer((string)reader["Customer_name"], (string)reader["Contact_email"],
                    (int)(Int64)reader["Customer_id"], (bool)reader["isGoldClubMember"]));
            return customers.ToArray();
        }

        public static DBResult Customer_Delete(int id)
        {
            if (!AlreadyExists("SELECT * FROM Customers WHERE Customer_id = '" + id + "';"))
                return DBResult.NotFound;
            string sql = "DELETE FROM Customers WHERE Customer_id = '" + id + "';";
            DBManager.DoQuery_Write(sql);
            return DBResult.Complete;
        }
        #endregion
        #region Plays
        public static DBResult Play_Create(string namex, string authorx, float basePricex, List<Performance> performancesx, PlayType playtypex)
        {
            string name = namex;
            string author = authorx;
            float basePrice = basePricex;
            List<Performance> performances = performancesx;
            int typeID = (int)playtypex;

            if (AlreadyExists("SELECT * FROM Plays WHERE Play_name='" + name + "';"))
            {
                return DBResult.AlreadyExists;
            }

            string sql = @"INSERT INTO Plays (Play_name,Play_type_id,Author,Base_price)
                VALUES('" + name + "','" + typeID + "','" + author + "','" + basePrice.ToString() + "');";
            DBManager.DoQuery_Write(sql);

            //foreach (Performance performance in performances)
            //{
            //    Performance_Create(play.GetPlayID(), performance.GetPerformanceDate());
            //}

            return DBResult.Complete;
        }

        public static Play[] Play_Search(string name, string author)
        {
            List<Play> plays = new List<Play>();
            string sql = @"SELECT * FROM Plays WHERE Play_name LIKE '%" + name
                + "%' AND Author LIKE '%" + author + "%';";
            SQLiteDataReader reader = DBManager.DoQuery_Read(sql);
            while (reader.Read())
            {
                int playID = (int)(Int64)reader["Play_id"];
                string playName = (string)reader["Play_name"];
                PlayType playType = (PlayType)(int)(Int64)reader["Play_type_id"];
                string playAuthor = (string)reader["Author"];
                float basePrice = (float)(double)reader["Base_price"];
                List <Performance> performancesList = new List<Performance>();

                plays.Add(new Play(playID, playName, playType, playAuthor, basePrice, performancesList));
            }
            return plays.ToArray();
        }

        public static Play Play_Get(string name, string author)
        {
            Play[] plays = Play_Search(name, author);
            if (plays.Length != 1)
                return null;
            else
                return plays[0];
        }

        public static Play Play_Get(int id)
        {
            string sql = "SELECT * FROM Plays WHERE Play_id = '" + id + "';";
            SQLiteDataReader reader = DBManager.DoQuery_Read(sql);
            if (reader.Read())
            {
                int playID = (int)(Int64)reader["Play_id"];
                string playName = (string)reader["Play_name"];
                PlayType playType = (PlayType)(int)(Int64)reader["Play_type_id"]; ;
                string playAuthor = (string)reader["Author"];
                float basePrice = (float)(double)reader["Base_price"];
                List<Performance> performancesList = new List<Performance>();

                return new Play(playID, playName, playType, playAuthor, basePrice, performancesList);
            }
            return null;
        }

        public static int Play_GetID(string name, string author)
        {
            string sql = "SELECT * FROM Plays WHERE Play_name = '" + name + "' AND Author='" + author +"';";
            SQLiteDataReader reader = DBManager.DoQuery_Read(sql);
            if (reader.Read())
            {
                return int.Parse(reader["Play_id"].ToString());
            }
            return -1;
        }

        public static bool Play_Unique(Play play)
        {
            Play[] results = Play_Search(play.GetPlayName(), play.GetAuthor());
            return results.Length == 1;
        }

        public static DBResult Play_Update(int playId, string name, string author, float basePrice)
        {
            if (!AlreadyExists("SELECT * FROM Plays WHERE Play_id = '" + playId + "';"))
                return DBResult.NotFound;

            string sql = "UPDATE Plays SET Play_name = '" + name + "', "
                + "Author = '" + author + "', "
                + "Base_price = '" + basePrice.ToString() + "' "
                + "WHERE Play_id = '" + playId + "';";

            DBManager.DoQuery_Write(sql);

            return DBResult.Complete;
        }

        public static DBResult Play_Delete(int playId)
        {
            if (!AlreadyExists("SELECT * FROM Plays WHERE Play_id ='" + playId + "';"))
                return DBResult.NotFound;
            string sql = "DELETE FROM Plays WHERE Play_id = '" + playId + "';";
            DBManager.DoQuery_Write(sql);

            return DBResult.Complete;
        }
        #endregion
        #region Performances
        public static DBResult Performance_Create(int playId, DateTime dateTime)
        {
            if (AlreadyExists("SELECT * FROM Performances WHERE Play_id = '" + playId + "' AND Date = '" + dateTime + "';"))
                return DBResult.AlreadyExists;

            string sql = "INSERT INTO Performances (Play_id,Date) VALUES('" + playId + "','" + dateTime.ToString() + "');";
            DBManager.DoQuery_Write(sql);

            return DBResult.Complete;
        }

        /*public static DBResult Performance_Update(int performanceID, Performance newPerformance)
        {
            if (!AlreadyExists("SELECT * FROM Performances WHERE Performance_id ='" + performanceID + "';"))
                return DBResult.NotFound;

            string sql = "UPDATE Performances SET Play_id = '" + newPerformance.GetPlayID() + "', Date = '" + newPerformance.GetPerformanceDate()
                + "' WHERE Performance_id ='" + performanceID + "';";
            DBManager.DoQuery_Write(sql);

            return DBResult.Complete;

        }*/

        public static DBResult Performances_Delete(int performanceID)
        {
            if (!AlreadyExists("SELECT * FROM Performances WHERE Performance_id ='" + performanceID + "';"))
                return DBResult.NotFound;
            
            string sql = "DELETE FROM Performances WHERE Performance_id = '" + performanceID + "';";
            DBManager.DoQuery_Write(sql);

            return DBResult.Complete;
        }

        public static List<Performance> Performances_Fetch(int playId)
        {
            List<Performance> performances = new List<Performance>();

            string sql = "SELECT * FROM Performances WHERE Play_id = '" + playId + "' ORDER BY Date;";
            SQLiteDataReader reader = DBManager.DoQuery_Read(sql);

            while (reader.Read())
            {
                performances.Add(new Performance(playId, (DateTime)reader["Date"], (int)(Int64)reader["Play_id"]));
            }

            return performances;
        }

        public static Performance Performances_Get(int id)
        {
            string sql = "SELECT * FROM Performances WHERE Performance_id = '" + id + "'";
            SQLiteDataReader reader = DBManager.DoQuery_Read(sql);
            reader.Read();

            return new Performance((int)(Int64)reader["Performance_id"], (DateTime)reader["Date"], (int)(Int64)reader["Play_id"]);
        }
        #endregion
        #region Booking
        public static DBResult Bookings_Create(int performanceIDs, int customerIDs, int bookingStates, bool isPaids, int price)
        {
            int performanceID = performanceIDs;
            int customerID = customerIDs;
            int bookingState = bookingStates;
            bool isPaid = isPaids;

            string sql = "INSERT INTO Bookings (Performance_id,Customer_id,Booking_state,isPaid,Price) "
                + "VALUES('" + performanceID + "','" + customerID + "','" + bookingState + "','" + isPaid + "','" + price + "');";
            DBManager.DoQuery_Write(sql);

            return DBResult.Complete;
        }

        public static DBResult Bookings_Update(int id, Booking booking)
        {
            if (!AlreadyExists("SELECT * FROM Bookings WHERE Booking_id ='" + id + "';"))
                return DBResult.NotFound;

            int performanceID = booking.GetPerformanceThisBookingIsFor().GetPerformanceID();
            int customerID = booking.GetCustomerWhoMadeThisBooking().GetCustomerID();
            int bookingState = (int)booking.GetBookingState();
            bool isPaid = booking.IsPaid();

            string sql = "UPDATE Bookings SET Performance_id = '" + performanceID + "', Customer_id = '"
                + customerID + "', Booking_state = '" + bookingState + "', isPaid = '" + isPaid
                + "' WHERE Booking_id = '" + id + "';";
            DBManager.DoQuery_Write(sql);
            return DBResult.Complete;
        }

        public static Booking Bookings_Get(int id)
        {
            string sql = "SELECT * FROM Bookings WHERE Booking_id = '" + id + "';";
            SQLiteDataReader reader = DBManager.DoQuery_Read(sql);
            reader.Read();

            int bookingID = (int)(Int64)reader["Booking_id"];
            Performance performance = Performances_Get((int)(Int64)reader["Performance_id"]);
            List<Seat> seats = Seats_GetReserved(bookingID).ToList<Seat>();
            BookingState bookingState = (BookingState)(int)(Int64)reader["Booking_state"];
            Customer customer = Manager.Customer_Get((int)(Int64)reader["Customer_id"]);
            bool isPaid = (bool)reader["isPaid"];
            int price = (int)(Int64)reader["Price"];

            return new Booking(bookingID, performance, seats, bookingState, customer, isPaid,price );
        }

        public static int Booking_LastID()
        {
            string sql = "SELECT Booking_id FROM Bookings ORDER BY Booking_id desc";
            SQLiteDataReader reader = DBManager.DoQuery_Read(sql);
            while (reader.Read())
            {
                return int.Parse((string)reader[0]);
            }

            return -1;
        }
           
        #endregion
        #region Seats
        public static Seat[] Seats_Fetch()
        {
            List<Seat> seats = new List<Classes.Seat>();
            string sql = "SELECT * FROM Seats";
            SQLiteDataReader reader = DBManager.DoQuery_Read(sql);

            while (reader.Read())
            {
                seats.Add(new Classes.Seat((int)reader["seat_ID"], (char)reader["seatRowCol"], -1, new SeatType(), (bool)reader["isUsable"]));
            }

            return seats.ToArray();
        }

        public static Seat Seats_FromID(int id)
        {
            string sql = "SELECT * FROM Seats WHERE seat_ID = '" + id + "';";
            SQLiteDataReader reader = DBManager.DoQuery_Read(sql);
            reader.Read();
            return new Classes.Seat(id, ((string)reader["seatRowCol"])[1], -1, new SeatType(), (bool)reader["isUsable"]);
        }

        public static string Seats_GetRowCol(int id)
        {
            string sql = "SELECT seatRowCol FROM Seats WHERE seat_ID ='" + id + "';";
            SQLiteDataReader reader = DBManager.DoQuery_Read(sql);
            reader.Read();
            return (string)reader["seatRowCol"];
        }

        public static DBResult Seats_Reserve(int bookingID, int performanceID, int seatID)
        {
            if (AlreadyExists("SELECT * FROM [Reserved Seats] WHERE Booking_id ='" + bookingID
                + "' AND Performance_id ='" + performanceID + "' AND seat_id = '" + seatID + "';"))
                return DBResult.AlreadyExists;

            string sql = "INSERT INTO [Reserved Seats] (Booking_id,Performance_id,seat_id) "
                + "VALUES('" + bookingID + "','" + performanceID + "','" + seatID + "');";
            DBManager.DoQuery_Write(sql);

            return DBResult.Complete;
        }

        public static Seat[] Seats_GetReserved(int bookingID)
        {
            List<Seat> seats = new List<Classes.Seat>();
            string sql = "SELECT * FROM [Reserved Seats] WHERE Booking_id = '" + bookingID + "';";
            SQLiteDataReader reader = DBManager.DoQuery_Read(sql);

            while (reader.Read())
            {
                seats.Add(Seats_FromID((int)(Int64)reader["seat_ID"]));
            }

            return seats.ToArray();
        }
        #endregion
    }
}
