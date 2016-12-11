using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using TheatreManagerApplication.DatabaseManager;

namespace TheatreManagerApplication.Backend
{
    public class DBManager
    {
        public static void Initialise()
        {
            DatabaseManager.DatabaseManager.Initialise();
        }

        public static SQLiteDataReader DoQuery_Read(string sql, CommandBehavior behaviour = CommandBehavior.Default)
        {
            return DatabaseManager.DatabaseManager.DoQuery_Read(sql, behaviour);
        }

        public static void DoQuery_Write(string sql)
        {
            DatabaseManager.DatabaseManager.DoQuery_Write(sql);
        }
        

        public static string path()
        {
            return DatabaseManager.DatabaseManager.Path();
        }
    }
}
