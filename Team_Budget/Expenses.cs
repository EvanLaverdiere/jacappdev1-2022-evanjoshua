﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{
    // ====================================================================
    // CLASS: expenses
    //        - A collection of expense items,
    //        - Read / write to database
    //        - etc
    // ====================================================================
    /// <summary>
    /// Class representing a collection of Expenses within a budget. 
    /// Has functionality to create collection by reading from a file, save collection to a file, and add or delete individual <see cref="Expense"/> objects.
    /// </summary>
    /// <seealso cref="Expense"/>
    /// <example>
    /// In this example, an empty Expenses object is created using the default constructor.
    /// <code>
    /// <![CDATA[
    /// Expenses expenses = new Expenses(connection);
    /// ]]>
    /// </code>
    /// </example>
    public class Expenses
    {
        private SQLiteConnection _connection;

        #region Constructors
        /// <summary>
        /// Creates a connection to the database of expenses. Modifications made to the Expenses object
        /// reflect directly on the Expenses database.
        /// </summary>
        /// <param name="conn">connection to be used to communicate with the database</param>
        /// <code>
        /// <![CDATA[
        /// String folder = "C:\\Abdel\\Desktop\\AppDev";
        /// String dbName = "newDatabase.db";
        /// string file = folder + "\\" + dbName;
        /// 
        /// Database.newDatabase(file);
        /// SQLiteConnection connection = Database.dbConnection;
        /// 
        /// Expenses expenses = new Expenses(connection);
        /// ]]>
        /// </code>
        public Expenses(SQLiteConnection conn)
        {
            _connection = conn;
        }

        #endregion

        #region Properties
        #endregion
        #region Add
        /// <summary>
        /// Creates a new <see cref="Expense"/> object based on passed values and adds it to the 
        /// Expenses database. An ID number is assigned automatically by the database.
        /// </summary>
        /// <param name="date">The date on which the expense occurred.</param>
        /// <param name="category">A number representing category of the expense.</param>
        /// <param name="amount">The cost or monetary amount of the expense.</param>
        /// <param name="description">A brief description of the expense.</param>
        /// <example>
        /// In this example, a blank list of Expenses is created. A new Expense object is then added directly
        /// to the list.
        /// <code>
        /// <![CDATA[
        ///     Expenses expenses = new Expenses(connection);
        ///     expenses.Add(DateTime.Now, 1, 450, "Electrician Consultation");
        /// ]]>
        /// </code
        /// </example>
        public void Add(DateTime date, int category, Double amount, String description)
        {
            try
            {
                using var cmd = new SQLiteCommand(_connection);
                cmd.CommandText = "INSERT INTO expenses(CategoryId, Amount, Description, Date) VALUES(@categoryId, @amount, @description, @date)";
                cmd.Parameters.AddWithValue("@categoryId", category);
                cmd.Parameters.AddWithValue("@amount", -amount);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                throw new Exception($"Expense [{date}, {category}, {amount}, {description}] could not be added: {error}");
            }
        }
        #endregion
        #region Delete
        /// <summary>
        /// Deletes the <see cref="Expense"/> object with the passed ID from the database.
        /// </summary>
        /// <param name="Id">The ID number of the expense to be deleted.</param>
        /// <example>
        /// In this example, an existing list of Expenses is loaded from a file. The expense with the ID of 3 is then deleted from the list.
        /// <code>
        /// <![CDATA[
        /// Expenses expenses = new Expenses(connection);
        /// int idToDelete = 3;
        /// 
        /// expenses.Delete(idToDelete);
        /// ]]>
        /// </code>
        /// </example>
        public void Delete(int Id)
        {
            try
            {
                using var cmd = new SQLiteCommand(_connection);
                cmd.CommandText = "DELETE from expenses where Id=@Id";
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                throw new Exception($"Expense [{Id}] could not be deleted: {error}");
            }
        }

        #endregion
        #region List
        /// <summary>
        /// Returns a list of expenses extracted from the database. Any changes made to this copy will not 
        /// affect the database.
        /// </summary>
        /// <returns>The list of expenses.</returns>
        /// <example>
        /// In this example [based on one written by Helen Katalifos and/or Sandy Bultena], a blank list 
        /// of Expenses is created. An Expense object is then added to the list. The list is then returned,
        /// and its contents are printed to the console.
        /// <code>
        /// <![CDATA[
        /// Expenses expenses = new Expenses(connection);
        /// expenses.Add(DateTime.Now, 1, 450, "Electrician Consultation");
        /// List&lt;Expense> list = expenses.List();
        /// foreach(Expense exp in list)
        ///     Console.WriteLine(exp.Description);
        /// ]]>
        /// </code>
        /// </example>
        public List<Expense> List()
        {
            try
            {
                List<Expense> newList = new List<Expense>();

                using SQLiteCommand command = new SQLiteCommand(_connection);
                string stm = "SELECT Id, CategoryId, Amount, Description, Date FROM expenses ORDER BY Id ASC";

                using var cmd = new SQLiteCommand(stm, _connection);
                using SQLiteDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int categoryId = reader.GetInt32(1);
                    double amount = reader.GetDouble(2);
                    string description = reader.GetString(3);
                    DateTime date = reader.GetDateTime(4);
                    newList.Add(new Expense(id, date, categoryId, amount, description));
                }

                return newList;
            }
            catch (Exception error)
            {
                throw new Exception($"Expense list could not be made: {error}");
            }
        }
        #endregion
        #region Update
        /// <summary>
        /// Changes the value of a given expense inside the database
        /// </summary>
        /// <param name="id">the id of the element to be changed</param>
        /// <param name="date">the new date to begiven to the expense</param>
        /// <param name="categoryId">the category d to be given to the expense</param>
        /// <param name="amount">the new amount to be given to the expense</param>
        /// <param name="description">the new description to be given to the expense</param>
        public void UpdateProperties(int id, DateTime date, int categoryId, double amount, string description)
        {
            try
            {
                using var cmd = new SQLiteCommand(_connection);

                cmd.CommandText = "UPDATE expenses SET Date=@newDate, CategoryId=@newCategryId, Amount=@newAmount, Description=@newDescription WHERE Id=@id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@newDate", date);
                cmd.Parameters.AddWithValue("@newCategryId", categoryId);
                cmd.Parameters.AddWithValue("@newAmount", amount);
                cmd.Parameters.AddWithValue("@newDescription", description);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                throw new Exception($"Expense [{id}] could not be updated: {error}");
            }
        }
        #endregion
    }
}
