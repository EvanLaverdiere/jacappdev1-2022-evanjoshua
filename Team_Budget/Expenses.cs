using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
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
    //        - Read / write to file
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
    /// Expenses expenses = new Expenses();
    /// </code>
    /// </example>
    public class Expenses
    {
        //private static String DefaultFileName = "budget.txt";
        private List<Expense> _Expenses = new List<Expense>();
        private string _FileName;
        private string _DirName;

        // ====================================================================
        // Properties
        // ====================================================================
        /// <summary>
        /// Gets the name of the file where the list of expenses is saved.
        /// </summary>
        public String FileName { get { return _FileName; } }
        /// <summary>
        /// Gets the directory containing the expenses file.
        /// </summary>
        public String DirName { get { return _DirName; } }

        SQLiteConnection _connection;

        public Expenses(SQLiteConnection conn, bool newDB = false)
        {
            _connection = conn;

            // if user specified they want a new database, set the list of expenses to defaults.
            if (newDB)
            {
                // empty out the old expenses
                ClearExpenses();

            }
        }

        // ====================================================================
        // populate categories from a file
        // if filepath is not specified, read/save in AppData file
        // Throws System.IO.FileNotFoundException if file does not exist
        // Throws System.Exception if cannot read the file correctly (parsing XML)
        // ====================================================================
        /// <summary>
        /// Reads a passed XML file to create a list of <see cref="Expense"/> objects.
        /// </summary>
        /// <param name="filepath">The filepath of the file to be read. Can be null.</param>
        /// <exception cref="FileNotFoundException">Thrown when the passed file is not found.</exception>
        /// <exception cref="Exception">Thrown when the passed file cannot be read correctly.</exception>
        /// <example>
        /// In this example, a blank list of Expense objects is created. The method then reads the specified file, populating the list with data which is then printed to the console.
        /// <code>
        ///     String expenseFile = "./test_expenses.exps";
        ///     Expenses expenses = new Expenses();
        ///     
        ///     expenses.ReadFromFile(expenseFile);
        ///     List&lt;Expense> list = expenses.List();
        ///     foreach(Expense exp in list)
        ///         Console.WriteLine(exp.Description);
        /// </code>
        /// </example>
        public void ReadFromFile(String filepath = null)
        {

            // ---------------------------------------------------------------
            // reading from file resets all the current expenses,
            // so clear out any old definitions
            // ---------------------------------------------------------------
            _Expenses.Clear();

            // ---------------------------------------------------------------
            // reset default dir/filename to null 
            // ... filepath may not be valid, 
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if it doesn't exist)
            // ---------------------------------------------------------------
            filepath = BudgetFiles.VerifyReadFromFileName(filepath/*, DefaultFileName*/);

            // ---------------------------------------------------------------
            // read the expenses from the db file
            // ---------------------------------------------------------------
            //_ReadXMLFile(filepath);
            _ReadDBFile();

            // ----------------------------------------------------------------
            // save filename info for later use?
            // ----------------------------------------------------------------
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);
        }


        // ====================================================================
        // save to a file
        // if filepath is not specified, read/save in AppData file
        // ====================================================================
        /// <summary>
        /// Saves the Expenses object to a specified file, in XML format. If no file is specified, saves the object to the current file.
        /// </summary>
        /// <param name="filepath">The filepath to which the Expenses will be saved.</param>
        /// <exception cref="Exception">Thrown when passed file does not exist or is read-only.</exception>
        /// <example>
        /// In this example, a blank list of Expenses is created. Two new Expense objects are created and added to the list. The list is then saved to a file.
        /// <code>
        /// Expenses expenses = new Expenses();
        /// String expenseFile = "./test_expenses.exps";
        /// 
        /// Expense expense1 = new Expense(1, DateTime.Now, 1, 450, "Electrician Consultation");
        /// expenses.Add(expense1);
        /// expenses.Add(DateTime.Now, 7, 60, "Root canal");
        /// 
        /// expenses.SaveToFile(expenseFile);
        /// </code>
        /// </example>
        public void SaveToFile(String filepath = null)
        {
            // ---------------------------------------------------------------
            // if file path not specified, set to last read file
            // ---------------------------------------------------------------
            if (filepath == null && DirName != null && FileName != null)
            {
                filepath = DirName + "\\" + FileName;
            }

            // ---------------------------------------------------------------
            // just in case filepath doesn't exist, reset path info
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if it doesn't exist)
            // ---------------------------------------------------------------
            filepath = BudgetFiles.VerifyWriteToFileName(filepath/*, DefaultFileName*/);

            // ---------------------------------------------------------------
            // save as XML
            // ---------------------------------------------------------------
            //_WriteXMLFile(filepath);
            _WriteDBFile();

            // ----------------------------------------------------------------
            // save filename info for later use
            // ----------------------------------------------------------------
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);
        }

        // ====================================================================
        // Add expense
        // ====================================================================
        /// <summary>
        /// Adds a passed <see cref="Expense"/> object to the list of expenses.
        /// </summary>
        /// <param name="exp">An expense to be added.</param>
        /// <example>
        /// In this example, a blank list of Expenses is created. A new Expense object is then created and added to the list.
        /// <code>
        ///     Expenses expenses = new Expenses();
        ///     Expense exp = new Expense(1, DateTime.Now, 1, 450, "Electrician Consultation");
        ///     expenses.Add(exp);
        /// </code>
        /// </example>
        private void Add(Expense exp)
        {
            _Expenses.Add(exp);
        }

        /// <summary>
        /// Creates a new <see cref="Expense"/> object based on passed values and adds it to the list of expenses. An ID number is assigned automatically.
        /// </summary>
        /// <param name="date">The date on which the expense occurred.</param>
        /// <param name="category">A number representing category of the expense.</param>
        /// <param name="amount">The cost or monetary amount of the expense.</param>
        /// <param name="description">A brief description of the expense.</param>
        /// <example>
        /// In this example, a blank list of Expenses is created. A new Expense object is then added directly to the list.
        /// <code>
        ///     Expenses expenses = new Expenses();
        ///     expenses.Add(DateTime.Now, 1, 450, "Electrician Consultation");
        /// </code>
        /// </example>
        public void Add(DateTime date, int category, Double amount, String description)
        {
            //int new_id = 1;

            //// if we already have expenses, set ID to max
            //if (_Expenses.Count > 0)
            //{
            //    new_id = (from e in _Expenses select e.Id).Max();
            //    new_id++;
            //}

            //set the new id to the most up to date number
            int new_id = (_Expenses[_Expenses.Count - 1].Id) + 1;

            _Expenses.Add(new Expense(new_id, date, category, -amount, description));

        }

        // ====================================================================
        // Delete expense
        // ====================================================================
        /// <summary>
        /// Deletes an <see cref="Expense"/> object which matches the passed ID.
        /// </summary>
        /// <param name="Id">The ID number of the expense to be deleted.</param>
        /// <example>
        /// In this example, an existing list of Expenses is loaded from a file. The expense with the ID of 3 is then deleted from the list.
        /// <code>
        ///     Expenses expenses = new Expenses();
        ///     String expenseFile = "./test_expenses.exps";
        ///     expenses.ReadFromFile(expenseFile);
        ///     List&lt;Expense> originalList = expenses.List();
        ///     
        ///     expenses.Delete(3);
        ///     List&lt;Expense> adjustedList = expenses.List();
        ///     
        ///     Console.WriteLine("List before deletion:");
        ///     foreach(Expense exp in originalList)
        ///         Console.WriteLine(exp.Description);
        ///     
        ///     Console.WriteLine("List after deletion:");
        ///     foreach(Expense exp in adjustedList)
        ///         Console.WriteLine(exp.Description);
        /// </code>
        /// </example>
        public void Delete(int Id)
        {
            int i = _Expenses.FindIndex(x => x.Id == Id);

            if (i > -1)
            {
                _Expenses.RemoveAt(i);
            }

        }

        // ====================================================================
        // Return list of expenses
        // Note:  make new copy of list, so user cannot modify what is part of
        //        this instance
        // ====================================================================
        /// <summary>
        /// Returns a copy of the list of expenses. Any changes made to this copy will not affect the original list.
        /// </summary>
        /// <returns>The copied list of expenses.</returns>
        /// <example>
        /// In this example [based on one written by Helen Katalifos and/or Sandy Bultena], a blank list of Expenses is created. An Expense object is then added to the list. The list is then returned, and its contents are printed to the console.
        /// <code>
        /// Expenses expenses = new Expenses();
        /// expenses.Add(DateTime.Now, 1, 450, "Electrician Consultation");
        /// List&lt;Expense> list = expenses.List();
        /// foreach(Expense exp in list)
        ///     Console.WriteLine(exp.Description);
        /// </code>
        /// </example>
        public List<Expense> List()
        {
            List<Expense> newList = new List<Expense>();
            foreach (Expense expense in _Expenses)
            {
                newList.Add(new Expense(expense));
            }
            return newList;
        }


        // ====================================================================
        // read from an XML file and add categories to our categories list
        // ====================================================================
        //private void _ReadXMLFile(String filepath)
        //{
        //    try
        //    {
        //        XmlDocument doc = new XmlDocument();
        //        doc.Load(filepath);

        //        // Loop over each Expense
        //        foreach (XmlNode expense in doc.DocumentElement.ChildNodes)
        //        {
        //            // set default expense parameters
        //            int id = int.Parse((((XmlElement)expense).GetAttributeNode("ID")).InnerText);
        //            String description = "";
        //            DateTime date = DateTime.Parse("2000-01-01");
        //            int category = 0;
        //            Double amount = 0.0;

        //            // get expense parameters
        //            foreach (XmlNode info in expense.ChildNodes)
        //            {
        //                switch (info.Name)
        //                {
        //                    case "Date":
        //                        date = DateTime.Parse(info.InnerText);
        //                        break;
        //                    case "Amount":
        //                        amount = Double.Parse(info.InnerText);
        //                        break;
        //                    case "Description":
        //                        description = info.InnerText;
        //                        break;
        //                    case "Category":
        //                        category = int.Parse(info.InnerText);
        //                        break;
        //                }
        //            }

        //            // have all info for expense, so create new one
        //            this.Add(new Expense(id, date, category, amount, description));

        //        }

        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("ReadFromFileException: Reading XML " + e.Message);
        //    }
        //}

        public void _ReadDBFile()
        {
            try
            {
                // Get the number of rows in the expense table
                using SQLiteCommand command = new SQLiteCommand(_connection);
                String stm = "SELECT COUNT(*) FROM expenses;";
                using var cm = new SQLiteCommand(stm, _connection);

                using SQLiteDataReader reader = cm.ExecuteReader();

                reader.Read();
                int numberOfRows = reader.GetInt32(0);

                // Loop through every row
                for (int i = 1; i < numberOfRows; i++)
                {
                    // Create an expense for every row
                    stm = "SELECT Id, CategoryId, Amount, Description, Date FROM categories WHERE Id = " + i;
                    using var cm2 = new SQLiteCommand(stm, _connection);

                    using SQLiteDataReader reader2 = cm2.ExecuteReader();

                    reader.Read();
                    int id = reader.GetInt32(0);
                    int categoryId = reader.GetInt32(1);
                    double amount = reader.GetDouble(2);
                    string description = reader.GetString(3);
                    DateTime date = reader.GetDateTime(4);

                    // have all info for expense, so create new one
                    this.Add(new Expense(id, date, categoryId, amount, description));
                }
            }
            catch (Exception e)
            {
                throw new Exception("ReadFromFileException: Reading Database " + e.Message);
            }

        }

        // ====================================================================
        // write to an XML file
        // if filepath is not specified, read/save in AppData file
        // ====================================================================
        //private void _WriteXMLFile(String filepath)
        //{
        //    // ---------------------------------------------------------------
        //    // loop over all categories and write them out as XML
        //    // ---------------------------------------------------------------
        //    try
        //    {
        //        // create top level element of expenses
        //        XmlDocument doc = new XmlDocument();
        //        doc.LoadXml("<Expenses></Expenses>");

        //        // foreach Category, create an new xml element
        //        foreach (Expense exp in _Expenses)
        //        {
        //            // main element 'Expense' with attribute ID
        //            XmlElement ele = doc.CreateElement("Expense");
        //            XmlAttribute attr = doc.CreateAttribute("ID");
        //            attr.Value = exp.Id.ToString();
        //            ele.SetAttributeNode(attr);
        //            doc.DocumentElement.AppendChild(ele);

        //            // child attributes (date, description, amount, category)
        //            XmlElement d = doc.CreateElement("Date");
        //            XmlText dText = doc.CreateTextNode(exp.Date.ToString("M'/'dd'/'yyyy hh:mm:ss tt")); // Need to fix this. It's making a test fail.
        //            ele.AppendChild(d);
        //            d.AppendChild(dText);

        //            XmlElement de = doc.CreateElement("Description");
        //            XmlText deText = doc.CreateTextNode(exp.Description);
        //            ele.AppendChild(de);
        //            de.AppendChild(deText);

        //            XmlElement a = doc.CreateElement("Amount");
        //            XmlText aText = doc.CreateTextNode(exp.Amount.ToString());
        //            ele.AppendChild(a);
        //            a.AppendChild(aText);

        //            XmlElement c = doc.CreateElement("Category");
        //            XmlText cText = doc.CreateTextNode(exp.Category.ToString());
        //            ele.AppendChild(c);
        //            c.AppendChild(cText);

        //        }

        //        // write the xml to FilePath
        //        doc.Save(filepath);

        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception("SaveToFileException: Reading XML " + e.Message);
        //    }
        //}

        private void _WriteDBFile()
        {
            try
            {

                ClearExpenses();
                foreach (Expense expense in _Expenses)
                {
                    using var cmd = new SQLiteCommand(_connection);
                    cmd.CommandText = "INSERT INTO expenses(Id, CategoryId, Amount, Description, Date) VALUES(@id, @categoryId, @amount, @description, @date)";
                    cmd.Parameters.AddWithValue("@id", expense.Id);
                    cmd.Parameters.AddWithValue("@categoryId", expense.Category);
                    cmd.Parameters.AddWithValue("@amount", expense.Amount);
                    cmd.Parameters.AddWithValue("@description", expense.Description);
                    cmd.Parameters.AddWithValue("@date", expense.Date);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw new Exception("SaveToFileException: Readin Database " + e.Message);
            }
        }


        private void ClearExpenses()
        {
            // clears the old table of expenses
            using SQLiteCommand cmd = new SQLiteCommand(_connection);
            cmd.CommandText = "DELETE FROM categoryTypes";
            cmd.ExecuteNonQuery();
        }
    }
}

