using System;
using System.Data.SQLite;
using System.IO;

// ===================================================================
// Very important notes:
// ... To keep everything working smoothly, you should always
//     dispose of EVERY SQLiteCommand even if you recycle a 
//     SQLiteCommand variable later on.
//     EXAMPLE:
//            Database.newDatabase(GetSolutionDir() + "\\" + filename);
//            var cmd = new SQLiteCommand(Database.dbConnection);
//            cmd.CommandText = "INSERT INTO categoryTypes(Description) VALUES('Whatever')";
//            cmd.ExecuteNonQuery();
//            cmd.Dispose();
//
// ... also dispose of reader objects
//
// ... by default, SQLite does not impose Foreign Key Restraints
//     so to add these constraints, connect to SQLite something like this:
//            string cs = $"Data Source=abc.sqlite; Foreign Keys=1";
//            var con = new SQLiteConnection(cs);
//
// ===================================================================


namespace Budget
{
    internal class Database
    {

        public static SQLiteConnection dbConnection { get { return _connection; } }
        private static SQLiteConnection _connection;

        // ===================================================================
        // create and open a new database
        // ===================================================================
        /// <summary>
        /// Creates a new Homebudget Database at the location given by <paramref name="filename"/>. Creates a 
        /// connection to the created database.
        /// </summary>
        /// <param name="filename">the location where the database is to be created</param>
        public static void newDatabase(string filename)
        {
            // If there was a database open before, close it and release the lock
            CloseDatabaseAndReleaseFile();

            //string cs = @"URI=file:C:\Users\Documents\test.db";
            // open a connection to the database specified in passed filename string
            string cs = $"Data Source={filename}; Foreign Keys=1";

            _connection = new SQLiteConnection(cs);
            _connection.Open();

            using SQLiteCommand cmd = new SQLiteCommand(_connection);

            cmd.CommandText = "PRAGMA foreign_keys = OFF; DROP TABLE IF EXISTS categoryTypes; PRAGMA foreign_keys = ON";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE categoryTypes(Id INTEGER PRIMARY KEY,
                Type TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "PRAGMA foreign_keys = OFF; DROP TABLE IF EXISTS categories; PRAGMA foreign_keys = ON";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE categories(Id INTEGER PRIMARY KEY,
                TypeId INTEGER, Description TEXT,
                FOREIGN KEY(typeId) REFERENCES categoryTypes(Id)
            )";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "PRAGMA foreign_keys = OFF; DROP TABLE IF EXISTS expenses; PRAGMA foreign_keys = ON";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE expenses(Id INTEGER PRIMARY KEY,
                CategoryId INTEGER, Amount DOUBLE, Description TEXT, Date TEXT,
                FOREIGN KEY (CategoryId) REFERENCES categories(Id)
            )";
            cmd.ExecuteNonQuery();
        }

        // ===================================================================
        // open an existing database
        // ===================================================================
        /// <summary>
        /// Opens a connection to the existing database given in <paramref name="filename"/>
        /// </summary>
        /// <param name="filename">the filepath of the database to be connected to</param>
        public static void existingDatabase(string filename)
        {
            if (File.Exists(filename))
            {
                CloseDatabaseAndReleaseFile();

                // your code
                string cs = $"Data Source={filename}; Foreign Keys=1";

                _connection = new SQLiteConnection(cs);
                _connection.Open();
            }
            else
            {
                throw new IOException("Database file does not exist.");
            }

        }

        // ===================================================================
        // close existing database, wait for garbage collector to
        // release the lock before continuing
        // ===================================================================
        /// <summary>
        /// Closes the open connection to a database.
        /// </summary>
        static public void CloseDatabaseAndReleaseFile()
        {
            if (Database.dbConnection != null)
            {
                // close the database connection
                Database.dbConnection.Close();


                // wait for the garbage collector to remove the
                // lock from the database file
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }
    }

}
