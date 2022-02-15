using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

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
        private static String DefaultFileName = "budgetCategories.txt";
        private List<Category> _Cats = new List<Category>();
        private string _FileName;
        private string _DirName;

        // ====================================================================
        // Properties
        // ====================================================================
        /// <summary>
        /// Gets the name of the file where the list of categories is saved.
        /// </summary>
        public String FileName { get { return _FileName; } }
        /// <summary>
        /// Gets the directory containing the categories file.
        /// </summary>
        public String DirName { get { return _DirName; } }

        // ====================================================================
        // Constructor
        // ====================================================================
        /// <summary>
        /// Creates a collection of <see cref="Category"/> objects with default values.
        /// </summary>
        /// <example>
        /// <code>
        /// Categories categories = new Categories();
        /// </code>
        /// </example>
        /// <seealso cref="SetCategoriesToDefaults"/>
        public Categories()
        {
            SetCategoriesToDefaults();
        }

        // ====================================================================
        // get a specific category from the list where the id is the one specified
        // ====================================================================
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
            Category c = _Cats.Find(x => x.Id == i);
            if (c == null)
            {
                throw new Exception("Cannot find category with id " + i.ToString());
            }
            return c;
        }

        // ====================================================================
        // populate categories from a file
        // if filepath is not specified, read/save in AppData file
        // Throws System.IO.FileNotFoundException if file does not exist
        // Throws System.Exception if cannot read the file correctly (parsing XML)
        // ====================================================================
        /// <summary>
        /// Reads a passed XML file to create a list of <see cref="Category"/> objects. If file not specified, reads from last opened file.
        /// </summary>
        /// <param name="filepath">The filepath of the file to be read. Can be null.</param>
        /// <exception cref="FileNotFoundException">Thrown if specified file does not exist.</exception>
        /// <exception cref="Exception">Thrown when the passed file cannot be read properly.</exception>
        /// <example>
        /// In this example, a list of default Categories is created. The Categories object is then overwritten with categories read from a specified file.
        /// <code>
        ///     string categoriesFile = "./test_categories.cats";
        ///     Categories categories = new Categories();
        ///     categories.ReadFromFile(categoriesFile);
        /// </code>
        /// </example>
        public void ReadFromFile(String filepath = null)
        {

            // ---------------------------------------------------------------
            // reading from file resets all the current categories,
            // ---------------------------------------------------------------
            _Cats.Clear();

            // ---------------------------------------------------------------
            // reset default dir/filename to null 
            // ... filepath may not be valid, 
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if it doesn't exist)
            // ---------------------------------------------------------------
            filepath = BudgetFiles.VerifyReadFromFileName(filepath, DefaultFileName);

            // ---------------------------------------------------------------
            // If file exists, read it
            // ---------------------------------------------------------------
            _ReadXMLFile(filepath);
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);
        }

        // ====================================================================
        // save to a file
        // if filepath is not specified, read/save in AppData file
        // ====================================================================
        /// <summary>
        /// Saves the Categories object to the specified file, in XML format. If no file is specified, saves the object to the current file.
        /// </summary>
        /// <param name="filepath">The filepath to which the Categories object will be saved.</param>
        /// <exception cref="Exception">Thrown when the passed filepath does not exist or is read-only.</exception>
        /// <example>
        /// In this example, a default list of Categories is created. After adding a new Category object to the list, the modified Categories object is saved to a file.
        /// <code>
        ///     Categories categories = new Categories();
        ///     String categoriesFile = "./test_categories.cats"'
        ///     
        ///     categories.Add("Legal Fees", Category.CategoryType.Expense);
        ///     categories.SaveToFile(categoriesFile);
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
            filepath = BudgetFiles.VerifyWriteToFileName(filepath, DefaultFileName);

            // ---------------------------------------------------------------
            // save as XML
            // ---------------------------------------------------------------
            _WriteXMLFile(filepath);

            // ----------------------------------------------------------------
            // save filename info for later use
            // ----------------------------------------------------------------
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);
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
            _Cats.Clear();

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
        /// Adds a passed <see cref="Category"/> object to the list of categories.
        /// </summary>
        /// <param name="cat">A category to be added.</param>
        /// <example>
        /// In this example, a list of Categories is created with default values. A new Category object is then created (its type has the default value of "Expense". This new object is then added to the list.
        /// <code>
        ///     Categories categories = new Categories();
        ///     Category newCat = new Category(18, "Legal Fees");
        ///     categories.Add(newCat);
        /// </code>
        /// </example>
        private void Add(Category cat)
        {
            _Cats.Add(cat);
        }

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
            int new_num = 1;
            if (_Cats.Count > 0)
            {
                new_num = (from c in _Cats select c.Id).Max();
                new_num++;
            }
            _Cats.Add(new Category(new_num, desc, type));
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
            int i = _Cats.FindIndex(x => x.Id == Id);

            if(i != -1)
            {
                _Cats.RemoveAt(i);
            }
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
            foreach (Category category in _Cats)
            {
                newList.Add(new Category(category));
            }
            return newList;
        }

        // ====================================================================
        // read from an XML file and add categories to our categories list
        // ====================================================================
        private void _ReadXMLFile(String filepath)
        {

            // ---------------------------------------------------------------
            // read the categories from the xml file, and add to this instance
            // ---------------------------------------------------------------
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filepath);

                foreach (XmlNode category in doc.DocumentElement.ChildNodes)
                {
                    String id = (((XmlElement)category).GetAttributeNode("ID")).InnerText;
                    String typestring = (((XmlElement)category).GetAttributeNode("type")).InnerText;
                    String desc = ((XmlElement)category).InnerText;

                    Category.CategoryType type;
                    switch (typestring.ToLower())
                    {
                        case "income":
                            type = Category.CategoryType.Income;
                            break;
                        case "expense":
                            type = Category.CategoryType.Expense;
                            break;
                        case "credit":
                            type = Category.CategoryType.Credit;
                            break;
                        case "savings":
                            type = Category.CategoryType.Savings;
                            break;
                        default:
                            type = Category.CategoryType.Expense;
                            break;
                    }
                    this.Add(new Category(int.Parse(id), desc, type));
                }

            }
            catch (Exception e)
            {
                throw new Exception("ReadXMLFile: Reading XML " + e.Message);
            }

        }


        // ====================================================================
        // write all categories in our list to XML file
        // ====================================================================
        private void _WriteXMLFile(String filepath)
        {
            try
            {
                // create top level element of categories
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<Categories></Categories>");

                // foreach Category, create an new xml element
                foreach (Category cat in _Cats)
                {
                    XmlElement ele = doc.CreateElement("Category");
                    XmlAttribute attr = doc.CreateAttribute("ID");
                    attr.Value = cat.Id.ToString();
                    ele.SetAttributeNode(attr);
                    XmlAttribute type = doc.CreateAttribute("type");
                    type.Value = cat.Type.ToString();
                    ele.SetAttributeNode(type);

                    XmlText text = doc.CreateTextNode(cat.Description);
                    doc.DocumentElement.AppendChild(ele);
                    doc.DocumentElement.LastChild.AppendChild(text);

                }

                // write the xml to FilePath
                doc.Save(filepath);

            }
            catch (Exception e)
            {
                throw new Exception("_WriteXMLFile: Reading XML " + e.Message);
            }

        }

    }
}

