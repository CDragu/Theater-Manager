using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheatreManagerApplication.Classes.ToFile
{
    static class ToFile
    {
        public static void WriteToFile(string[] Labels, string[,] Content,int arrayrows, int arraycolums, string NameofFile)
        {
            string filePath = "../"+ NameofFile+ ".csv";

            var csv = new StringBuilder();

            for(int i = 0; i < Labels.Length; i++)
            {
                var newLine = string.Format("{0},", Labels[i]);
                csv.Append(newLine);
            }

            csv.AppendLine();

            for (int i = 0; i < arrayrows; i++)
            {
                for(int j = 0; j < arraycolums; j++)
                {
                var newLine = string.Format("{0},", Content[i,j]);
                csv.Append(newLine);
                }
                csv.AppendLine();
            }
            try
            {

            File.WriteAllText(filePath, csv.ToString());
            }
            catch
            {
                
            }
            
        }
        public static void MakeSalesReport(string year = "2016")
        {
            string filePath = "../Sales.csv";
            StringBuilder csv = new StringBuilder();

            int[] bookingsCompleted = new int[13];
            int[] bookingsCanceled = new int[13];
            int[] actualProfit = new int[13];
            int[] potentialProfit = new int[13];

            // Completed Bookings
            for (int i = 1; i < 13; i++)
            {
                int bookingsCompletedCount = 0;
                int bookingsCompletedPrice = 0;
                string sql_booking_completed = string.Format("SELECT Price FROM Bookings INNER JOIN Performances ON Bookings.Performance_id=Performances.Performance_id AND Performances.Date like '__/{0}/{1}%' AND Bookings.Booking_state=0",
                    ConvertIntToString(i), year);
                var reader = DatabaseManager.DatabaseManager.DoQuery_Read(sql_booking_completed, System.Data.CommandBehavior.Default);
                while (reader.Read())
                {
                    bookingsCompletedCount++;
                    bookingsCompletedPrice += (int)(Int64)reader["Price"];
                }
                bookingsCompleted[i] = bookingsCompletedCount;
                actualProfit[i] = bookingsCompletedPrice;
            }

            // Canceled Bookings
            for (int i = 1; i <= 12; i++)
            {
                int bookingsCanceledCount = 0;
                int bookingsCanceledPrice = 0;
                string sql_booking_canceled = string.Format("SELECT Price FROM Bookings INNER JOIN Performances ON Bookings.Performance_id=Performances.Performance_id AND Performances.Date like '__/{0}/{1}%' AND Bookings.Booking_state=1",
                    ConvertIntToString(i), year);
                var reader = DatabaseManager.DatabaseManager.DoQuery_Read(sql_booking_canceled, System.Data.CommandBehavior.Default);
                while (reader.Read())
                {
                    bookingsCanceledCount++;
                    bookingsCanceledPrice += (int)(Int64)reader["Price"];
                }
                bookingsCanceled[i] = bookingsCanceledCount;
                potentialProfit[i] = actualProfit[i] + bookingsCanceledPrice;
            }

            #region Writing To File

            // Writing the labels for csv data
            for (int i = 0; i <= 12; i++)
            {
                if (i == 0) csv.Append(string.Empty + ",");
                else csv.Append("Month:" + i.ToString() + ",");
            }
            csv.AppendLine();

            // Writing the number of completed bookings
            for (int i = 0; i <= 12; i++)
            {
                if (i == 0) csv.Append("Completed Bookings,");
                else csv.Append(bookingsCompleted[i].ToString() + ",");
            }
            csv.AppendLine();

            // Writing the actual profit
            for (int i = 0; i <= 12; i++)
            {
                if (i == 0) csv.Append("Actual Profit,");
                else csv.Append(actualProfit[i].ToString() + ",");
            }
            csv.AppendLine();

            // Writing the number of canceled bookings
            for (int i = 0; i <= 12; i++)
            {
                if (i == 0) csv.Append("Canceled Bookings,");
                else csv.Append(bookingsCanceled[i].ToString() + ",");
            }
            csv.AppendLine();

            // Writing the potential profit
            for (int i = 0; i <= 12; i++)
            {
                if (i == 0) csv.Append("Potential Profit,");
                else csv.Append(potentialProfit[i].ToString() + ",");
            }
            File.WriteAllText(filePath, csv.ToString());
            #endregion
        }

        /// <summary>
        /// Converts an integer to a double digit number in string format
        /// </summary>
        /// <returns></returns>
        private static string ConvertIntToString(int number)
        {
            return number < 10 ? "0" + number.ToString() : number.ToString();

        }

    }
}
