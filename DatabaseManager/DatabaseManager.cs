using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;

namespace TheatreManagerApplication.DatabaseManager
{
    public class DatabaseManager
    {
        private static string databasePath = @"data source=Assets\TheatreManager.db;datetimeformat=CurrentCulture";
        private static SQLiteConnection databaseConection;
        public static void Initialise()
        {
            databaseConection = new SQLiteConnection(databasePath);
            try
            {
                databaseConection.Open();
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public static string Path()
        {
            return databaseConection.FileName;
        }

        public static SQLiteDataReader DoQuery_Read(string sql, System.Data.CommandBehavior behaviour = System.Data.CommandBehavior.Default)
        {
            SQLiteDataReader dataReader = null;
            //try
            //{
                SQLiteCommand command = new SQLiteCommand(sql, databaseConection);
                dataReader = command.ExecuteReader(behaviour);
            //}
            //catch (Exception e)
            //{
            //    throw e;
            //}
            return dataReader;
        }

        public static DataSet DoQuery_Read(string sql)
        {
            DataSet dataSet = new DataSet();
            try
            {
                SQLiteCommand command = new SQLiteCommand(sql, databaseConection);
                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(command);
                dataAdapter.Fill(dataSet);
            }
            catch (Exception e)
            {
                throw e;
            }
            return dataSet;
        }

        public static void DoQuery_Write(string sql)
        {
            //try
            //{
                SQLiteCommand command = new SQLiteCommand(sql, databaseConection);
                command.ExecuteNonQuery();
            //}
            //catch (Exception e)
            //{
            //    throw e;
            //}
        }       
    }
}
