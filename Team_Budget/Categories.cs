using System;
using System.Collections.Generic;
using System.Data.SQLite;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{
    // ====================================================================
    // CLASS: categories
    //        - A collection of category items,
    //        - Read / write to file
    //        - etc
    // ====================================================================
    /// <summary>
    /// Class which represents a collection of Categories within a budget.
    /// Has functionality to create the collection by reading from a file, save the collection to a file, create or set the collection to default values, and add or delete individual <see cref="Category"/> objects.
    /// </summary>
    /// <seealso cref="Category"/>
    public class Categories
    {
        private SQLiteConnection _connection;

        #region Constructors
        /// <summary>
        /// Connects to the categories table of an SQLite database. 
        /// If database is new, creates a collection <see cref="Category"/> objects with default values.
        /// </summary>
        /// <param name="conn">A connection to an SQLite database.</param>
        /// <param name="newDB">True if a new database is to be created, false otherwise.</param>
        public Categories(SQLiteConnection conn, bool newDB = false)
        {
            _connection = conn;

            // if user specified they want a new database, set the list of categories to defaults.
            if (newDB)
            {
                // fill the categoryTypes table.
                SetCategoryTypes();

                SetCategoriesToDefaults();
            }
        }
        #endregion

        #region Method
        /// <summary>
        /// Gets a <see cref="Category"/> object corresponding to the passed ID number.
        /// </summary>
        /// <param name="i">The ID number of the desired category.</param>
        /// <returns>The desired Category object.</returns>
        /// <exception cref="Exception">Thrown when no category matches the passed ID number.</exception>
        /// <example>
        /// In this example, a list of default Categories is created. GetCategoryFromId is then called to retrieve the Category with the ID of 5. The string representation of said Category is then printed to the console.
        /// <code>
        ///     Categories categories = new Categories();
        ///     int targetId = 5;
        ///     
        ///     Category targetCat = categories.GetCategoryFromId(targetId);
        ///     Console.WriteLine(String.Format("The category with the ID number of {0} is {1}.", targetId, targetCat.ToString()));
        /// </code>
        /// </example>
        public Category GetCategoryFromId(int i)
        {
            using SQLiteCommand command = new SQLiteCommand(_connection);
            String stm = "SELECT Id, TypeId, Description FROM categories WHERE Id = " + i;

            using var cm = new SQLiteCommand(stm, _connection);
            using SQLiteDataReader reader = cm.ExecuteReader();

            reader.Read();

            int id = reader.GetInt32(0);
            int typeId = reader.GetInt32(1) - 1;
            string description = reader.GetString(2);

            Category category = new Category(id, description, (Category.CategoryType)typeId);

            return category;
        }

        // ====================================================================
        // set categories to default
        // ====================================================================
        /// <summary>
        /// Sets the list of categories to default values, overwriting whatever was previously there.
        /// </summary>
        /// <example>
        /// In this example, a Categories object is created and populated with data read from a file. Two new Category objects are added to the list. The Categories object is then set to a list of default values.
        /// <code>
        ///     Categories categories = new Categories();
        ///     String categoriesFile = "./test_categories.cats";
        ///     
        ///     categories.ReadFromFile(categoriesFile);
        ///     categories.Add(new Category("Legal Fees", Category.CategoryType.Expense));
        ///     categories.Add(new Category("Bail", Category.CategoryType.Expense));
        ///     
        ///     categories.SetCategoriesToDefaults();
        /// </code>
        /// </example>
        public void SetCategoriesToDefaults()
        {
            // ---------------------------------------------------------------
            // reset any current categories,
            // ---------------------------------------------------------------
            //_Cats.Clear();
            using SQLiteCommand cmd = new SQLiteCommand(_connection);
            cmd.CommandText = "DELETE FROM categories";
            cmd.ExecuteNonQuery();

            // ---------------------------------------------------------------
            // Add Defaults
            // ---------------------------------------------------------------
            Add("Utilities", Category.CategoryType.Expense);
            Add("Rent", Category.CategoryType.Expense);
            Add("Food", Category.CategoryType.Expense);
            Add("Entertainment", Category.CategoryType.Expense);
            Add("Education", Category.CategoryType.Expense);
            Add("Miscellaneous", Category.CategoryType.Expense);
            Add("Medical Expenses", Category.CategoryType.Expense);
            Add("Vacation", Category.CategoryType.Expense);
            Add("Credit Card", Category.CategoryType.Credit);
            Add("Clothes", Category.CategoryType.Expense);
            Add("Gifts", Category.CategoryType.Expense);
            Add("Insurance", Category.CategoryType.Expense);
            Add("Transportation", Category.CategoryType.Expense);
            Add("Eating Out", Category.CategoryType.Expense);
            Add("Savings", Category.CategoryType.Savings);
            Add("Income", Category.CategoryType.Income);

        }

        // ====================================================================
        // Add category
        // ====================================================================

        /// <summary>
        /// Creates a new <see cref="Category"/> object based on passed values and adds it to the list of categories. An ID number is assigned automatically.
        /// </summary>
        /// <param name="desc">A brief description of the category.</param>
        /// <param name="type">The category's type.</param>
        /// <example>
        /// In this example, a list of Categories is created with default values. A new Category object is then added to the list. The revised list is then printed to the console.
        /// <code>
        ///     Categories categories = new Categories();
        ///     categories.Add("Legal Fees", Category.CategoryType.Expense);
        ///     
        ///     foreach(Category cat in categories.List())
        ///         Console.WriteLine(cat.ToString());
        /// </code>
        /// </example>
        public void Add(String desc, Category.CategoryType type)
        {
            using var cmd = new SQLiteCommand(_connection);
            cmd.CommandText = "INSERT INTO categories(Description, TypeId) VALUES(@description, @type)";
            cmd.Parameters.AddWithValue("@description", desc);
            cmd.Parameters.AddWithValue("@type", (int)type + 1);
            cmd.Prepare();
            cmd.ExecuteNonQuery();

        }


        // ====================================================================
        // Delete category
        // ====================================================================
        /// <summary>
        /// Deletes a <see cref="Category"/> object which matches the passed ID number.
        /// </summary>
        /// <param name="Id">The ID number of the category to be deleted.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the passed ID number is not found within the categories list.</exception>
        /// <example>
        /// In this example, a Categories list is created with default values. The number of the last Category in the list is then identified, and the Category corresponding to that number is deleted.
        /// <code>
        ///     Categories categories = new Categories();
        ///     int totalCats = categories.List().Count;
        ///     categories.Delete(totalCats);
        /// </code>
        /// </example>
        public void Delete(int Id)
        {
            using var cmd = new SQLiteCommand(_connection);
            cmd.CommandText = "DELETE from categories where Id=@Id";
            cmd.Parameters.AddWithValue("@Id", Id);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        // ====================================================================
        // Return list of categories
        // Note:  make new copy of list, so user cannot modify what is part of
        //        this instance
        // ====================================================================
        /// <summary>
        /// Returns a copy of the list of categories. Any changes made to this copy will not affect the original list.
        /// </summary>
        /// <returns>The copied list of categories.</returns>
        /// <example>
        /// In this example, a list of Categories is created with default values. A new Category object is then added to the list. The List() method is then called, and a string representation of each Category in that list is then printed to the console.
        /// <code>
        ///     Categories categories = new Categories();
        ///     categories.Add("Legal Fees", Category.CategoryType.Expense);
        ///     List&lt;Category> list = categories.List();
        ///     
        ///     foreach(Category cat in list)
        ///         Console.WriteLine(cat.ToString());
        /// </code>
        /// </example>
        public List<Category> List()
        {
            List<Category> newList = new List<Category>();

            using SQLiteCommand command = new SQLiteCommand(_connection);
            string stm = "SELECT Id, TypeId, Description FROM categories ORDER BY Id ASC";

            using var cmd = new SQLiteCommand(stm, _connection);
            using SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int typeId = reader.GetInt32(1) - 1;
                string description = reader.GetString(2);
                newList.Add(new Category(id, description, (Category.CategoryType)typeId));
            }

            return newList;
        }


        // ====================================================================
        // write all categories in our list to XML file
        // ====================================================================

        public void UpdateProperties(int id, string newDescription, Category.CategoryType newType)
        {
            // To be filled
            using var cmd = new SQLiteCommand(_connection);

            cmd.CommandText = "UPDATE categories SET Description=@newDescription, TypeId=@newType WHERE Id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@newDescription", newDescription);
            cmd.Parameters.AddWithValue("@newType", (int)newType + 1);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        private void SetCategoryTypes()
        {
            // Remove values from table.
            using SQLiteCommand cmd = new SQLiteCommand(_connection);
            cmd.CommandText = "DELETE FROM categoryTypes";
            cmd.ExecuteNonQuery();

            // Insert default category types.
            string[] typeNames = Enum.GetNames(typeof(Category.CategoryType));

            for(int i = 0; i < typeNames.Length; i++)
            {
                cmd.CommandText = "INSERT INTO categoryTypes (Id, Type) VALUES (@id, \"@type\")";
                cmd.Parameters.AddWithValue("@id", i + 1);
                cmd.Parameters.AddWithValue("@type", typeNames[i]);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }

        }
        #endregion
    }
}

