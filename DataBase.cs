using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace WPF_Devexpress_GridControl
{
    class Database
    {

        public SQLiteConnection myConnection;

        public Database()

        {
            this.myConnection = new SQLiteConnection("Data Source=wpf.db");
            if (!File.Exists("./wpf.db"))
            {
                SQLiteConnection.CreateFile("wpf.db");


                //string query = "CREATE TABLE Users ('ID' INTEGER NOT NULL UNIQUE,'FirstName' TEXT,'LastName'  TEXT,'Age'   INTEGER,'Address'   TEXT,PRIMARY KEY('ID' AUTOINCREMENT))";


            }
        }

        public void OpenConnection()
        {


            if (myConnection.State != System.Data.ConnectionState.Open)
            {
                myConnection.Open();
            }
        }
        public void CloseConnection()
        {


            if (myConnection.State != System.Data.ConnectionState.Closed)
            {
                myConnection.Close();
            }
        }


    }
}
