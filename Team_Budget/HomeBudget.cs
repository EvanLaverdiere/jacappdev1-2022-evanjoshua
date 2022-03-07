using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Dynamic;
using System.Data.SQLite;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================


namespace Budget
{
    // ====================================================================
    // CLASS: HomeBudget
    //        - Combines categories Class and expenses Class
    //        - One File defines Category and Budget File
    //        - etc
    // ====================================================================
    /// <summary>
    /// Class representing the core of a budget-tracking project. Has functionality to produce a collection of <see cref="BudgetItem"/> objects by combining <see cref="Categories"/> and <see cref="Expenses"/> objects; to group these BudgetItems by common <see cref="Category"/>, by month, or by category and month; to save the budget to a series of files; to create this collection by reading from a file; and to save the budget to a series of files.
    /// </summary>
    /// <example>
    /// In this example, a HomeBudget object is created. Its Categories have a list of default values, while its Expenses are blank.
    /// 
    /// One new Category and one new Expense are then created. They are added to the HomeBudget's Categories and Expenses properties.
    /// 
    /// The modified budget is then saved to a file. In the process, files for categories and expenses are created in the same directory.
    /// <code>
    /// <![CDATA[
    /// try
    /// {
    ///     String filename = "./test.budget";
    ///     HomeBudget homeBudget = new HomeBudget();
    ///
    ///     // Initial list will be empty because there are no Expenses to combine with Categories.
    ///     List<BudgetItem> budgetItems = homeBudget.GetBudgetItems(null, null, false, 0);
    ///
    ///     homeBudget.categories.Add("Legal Fees", Category.CategoryType.Expense);
    ///     homeBudget.expenses.Add(DateTime.Now, 17, 100, "Lawyer consultation");
    ///
    ///     // The new list will include a new BudgetItem with an Amount of -100, a Balance of -100, a Category of "Legal Fees", a CategoryID of 17, an ExpenseID of 1, and a ShortDescription of "Lawyer consultation".
    ///     List<BudgetItem> revisedItems = homeBudget.GetBudgetItems(null, null, false, 0);
    ///
    ///     // Save the budget to a file. Files for expenses and categories will be created at the same time.
    ///     homeBudget.SaveToFile(filename);
    /// }
    /// catch (Exception e)
    /// {
    ///     Console.WriteLine("ERROR: " + e.Message);
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// <seealso cref="BudgetItem"/>
    /// <seealso cref="Expenses"/>
    /// <seealso cref="Categories"/>
    /// <seealso cref="BudgetItemsByCategory"/>
    /// <seealso cref="BudgetItemsByMonth"/>
    public class HomeBudget
    {
        private string _FileName;
        private string _DirName;
        private Categories _categories;
        private Expenses _expenses;
        private SQLiteConnection _connection;

        #region Properties
        /// <summary>
        /// Gets the name of the file containing the budget.
        /// </summary>
        public String FileName { get { return _FileName; } }
        /// <summary>
        /// Gets the directory where the file containing the budget is stored.
        /// </summary>
        public String DirName { get { return _DirName; } }
        /// <summary>
        /// Gets the full path to the location of the file which contains the budget. If directory or file name have no value, returns null.
        /// </summary>
        public String PathName
        {
            get
            {
                if (_FileName != null && _DirName != null)
                {
                    return Path.GetFullPath(_DirName + "\\" + _FileName);
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Gets a Categories object.
        /// </summary>
        public Categories categories { get { return _categories; } }
        /// <summary>
        /// Gets an Expenses object.
        /// </summary>
        public Expenses expenses { get { return _expenses; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a HomeBudget object with its categories list and expenses lists. 
        /// A new database is created if the database is to be created.
        /// </summary>
        /// <param name="HomeBudgetDBFile">the location of the database to use or create</param>
        /// <param name="newDatabase">the flag that indicates if a new database is to be created</param>
        public HomeBudget(String HomeBudgetDBFile, bool newDatabase = false)
        {
            // if database exists, and user doesn't want a new database, open existing DB
            if (!newDatabase && File.Exists(HomeBudgetDBFile))
            {
                Database.existingDatabase(HomeBudgetDBFile);
            }

            // file did not exist, or user wants a new database, so open NEW DB
            else
            {
                Database.newDatabase(HomeBudgetDBFile);
                newDatabase = true;
            }

            // create the categories object
            _categories = new Categories(Database.dbConnection, newDatabase);


            // create the expenses object
            _categories = new Categories(Database.dbConnection, newDatabase);


            // assign a value to the connection property so we can properly execute queries.
            _connection = Database.dbConnection;
        }
        #endregion

        #region Methods

        // ============================================================================
        // Get all expenses list
        // NOTE: VERY IMPORTANT... budget amount is the negative of the expense amount
        // Reasoning: an expense of $15 is -$15 from your bank account.
        // ============================================================================
        /// <summary>
        /// Gets a list of BudgetItems by combining the contents of the Expenses and Categories objects. List can be restricted to a specific time frame and to a specific category.
        /// </summary>
        /// <param name="Start">The beginning of the desired time frame. Can be null.</param>
        /// <param name="End">The end of the desired time frame. Can be null.</param>
        /// <param name="FilterFlag">True if the list will filter out unwanted categories; false otherwise.</param>
        /// <param name="CategoryID">The ID number of the desired category. Must be included even if the list will not be filtered.</param>
        /// <returns>The list of BudgetItems, organized by date in ascending order.</returns>
        /// <example>
        /// 
        /// For all examples below, assume the budget file contains the
        /// following elements:
        /// 
        /// <code>
        /// Cat_ID  Expense_ID  Date                    Description                    Cost     Balance
        ///    10       1       1/10/2018 12:00:00 AM   Clothes hat (on credit)         10      -10
        ///     9       2       1/11/2018 12:00:00 AM   Credit Card hat                -10       0
        ///    10       3       1/10/2019 12:00:00 AM   Clothes scarf(on credit)        15      -15
        ///     9       4       1/10/2020 12:00:00 AM   Credit Card scarf              -15       0
        ///    14       5       1/11/2020 12:00:00 AM   Eating Out McDonalds            45      -45
        ///    14       7       1/12/2020 12:00:00 AM   Eating Out Wendys               25      -70
        ///    14      10       2/1/2020 12:00:00 AM    Eating Out Pizza                33.33   -103.33
        ///     9      13       2/10/2020 12:00:00 AM   Credit Card mittens            -15      -88.33
        ///     9      12       2/25/2020 12:00:00 AM   Credit Card Hat                -25      -63.33
        ///    14      11       2/27/2020 12:00:00 AM   Eating Out Pizza                33.33   -96.66
        ///    14       9       7/11/2020 12:00:00 AM   Eating Out Cafeteria            11.11   -107.77
        /// </code>
        /// 
        /// <b>Getting a list of ALL budget items.</b>
        /// 
        /// <code>
        /// <![CDATA[
        ///  HomeBudget budget = new HomeBudget();
        ///  budget.ReadFromFile(filename);
        ///  
        ///  // Get a list of all budget items
        ///  var budgetItems = budget.GetBudgetItems(null, null, false, 0);
        ///            
        ///  // print important information
        ///  foreach (var bi in budgetItems)
        ///  {
        ///      Console.WRiteLine ( 
        ///          String.Format("{0} {1,-20}  {2,8:C} {3,12:C}", 
        ///             bi.Date.ToString("yyyy/MMM/dd"),
        ///             bi.ShortDescription,
        ///             bi.Amount, bi.Balance)
        ///       );
        ///  }
        ///
        /// ]]>
        /// </code>
        /// 
        /// Sample output (note that the list is sorted chronologically in ascending order):
        /// <code>
        /// 2018/Jan/10 hat (on credit)       ($10.00)     ($10.00)
        /// 2018/Jan/11 hat                     $10.00        $0.00
        /// 2019/Jan/10 scarf(on credit)      ($15.00)     ($15.00)
        /// 2020/Jan/10 scarf                   $15.00        $0.00
        /// 2020/Jan/11 McDonalds             ($45.00)     ($45.00)
        /// 2020/Jan/12 Wendys                ($25.00)     ($70.00)
        /// 2020/Feb/01 Pizza                 ($33.33)    ($103.33)
        /// 2020/Feb/10 mittens                 $15.00     ($88.33)
        /// 2020/Feb/25 Hat                     $25.00     ($63.33)
        /// 2020/Feb/27 Pizza                 ($33.33)     ($96.66)
        /// 2020/Jul/11 Cafeteria             ($11.11)    ($107.77)
        /// </code>
        /// 
        /// <b>Getting a FILTERED list of budget items</b>
        /// 
        /// For this example, the value of the FilterFlag is set to true, and the category ID is set to 9 ("Credit").
        /// 
        /// <code>
        /// <![CDATA[
        /// HomeBudget newBudget = new HomeBudget();
        /// newBudget.ReadFromFile(filename);
        /// int creditId = 9;
        /// 
        /// // Get a filtered list of budget items which only contains items belonging to the Credit category.
        /// var creditItems = newBudget.GetBudgetItems(null, null, true, creditId);
        /// 
        /// //print important information
        /// foreach(BudgetItem item in creditItems)
        /// {
        ///     Console.WRiteLine ( 
        ///          String.Format("{0} {1,-20}  {2,8:C} {3,12:C}", 
        ///             bi.Date.ToString("yyyy/MMM/dd"),
        ///             bi.ShortDescription,
        ///             bi.Amount, bi.Balance)
        ///       );
        /// }
        /// ]]>
        /// </code>
        /// 
        /// Sample output (note that Balance correctly tracks only the filtered items):
        /// <code>
        /// 2018/Jan/11 hat                     $10.00      $10.00
        /// 2020/Jan/10 scarf                   $15.00      $25.00
        /// 2020/Feb/10 mittens                 $15.00      $40.00
        /// 2020/Feb/25 Hat                     $25.00      $65.00
        /// </code>
        /// </example>
        public List<BudgetItem> GetBudgetItems(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // ------------------------------------------------------------------------
            // return joined list within time frame
            // ------------------------------------------------------------------------
            Start = Start ?? new DateTime(1900, 1, 1);
            string startString = Start.Value.ToString("yyyy-MM-dd");
            End = End ?? new DateTime(2500, 1, 1);
            string endString = End.Value.ToString("yyyy-MM-dd");


            using SQLiteCommand command = new SQLiteCommand(_connection);
            StringBuilder stm = new StringBuilder();
            stm.Append("Select e.Id as 'ExpenseId', c.Id as 'CategoryId', " +
                            "e.Date, e.Amount, e.Description as 'ExpenseDescription', " +
                            "c.Description as 'CategoryDescription' " +
                            "FROM expenses as e " +
                            "INNER JOIN categories as c " +
                            "ON e.CategoryId=c.Id " +
                            $"WHERE e.Date>={startString} AND e.Date<={endString} ");

            if (FilterFlag)
            {
                stm.Append($"AND c.Id={CategoryID}");
            }

            stm.Append(";");

            using var cmd = new SQLiteCommand(stm.ToString(), _connection);
            using SQLiteDataReader reader = cmd.ExecuteReader();

            List<BudgetItem> budgetItemList = new List<BudgetItem>();
            double balance = 0;
            while (reader.Read())
            {

                balance += reader.GetDouble(3);
                budgetItemList.Add(new BudgetItem
                {
                    ExpenseID = reader.GetInt32(0),
                    CategoryID = reader.GetInt32(1),
                    Date = reader.GetDateTime(2),
                    Amount = reader.GetDouble(3),
                    ShortDescription = reader.GetString(4),
                    Category = reader.GetString(5),
                    Balance = balance
                });
            }

            return budgetItemList;
        }

        // ============================================================================
        // Group all expenses month by month (sorted by year/month)
        // returns a list of BudgetItemsByMonth which is 
        // "year/month", list of budget items, and total for that month
        // ============================================================================
        /// <summary>
        /// Gets a list of BudgetItem lists. Each list contains BudgetItems which occurred during a specific month and year, along with the total for that month. The lists are sorted by order of year and month. The list can be restricted to a specific time frame, and filtered to exclude budget items which do not belong to a specific category.
        /// </summary>
        /// <param name="Start">The beginning of the desired time frame. Can be null.</param>
        /// <param name="End">The end of the desired time frame. Can be null.</param>
        /// <param name="FilterFlag">True if the list will filter out unwanted categories; false otherwise.</param>
        /// <param name="CategoryID">The ID number of the desired category. Must be included even if the list will not be filtered.</param>
        /// <returns>The list of BudgetItemsByMonth, in order of year and month.</returns>
        /// <seealso cref="BudgetItemsByMonth"/>
        /// <example>
        /// 
        /// For all examples below, assume the budget file contains the
        /// following elements:
        /// 
        /// <code>
        /// Cat_ID  Expense_ID  Date                    Description                    Cost     Balance
        ///    10       1       1/10/2018 12:00:00 AM   Clothes hat (on credit)         10      -10
        ///     9       2       1/11/2018 12:00:00 AM   Credit Card hat                -10      10
        ///    10       3       1/10/2019 12:00:00 AM   Clothes scarf(on credit)        15      -15
        ///     9       4       1/10/2020 12:00:00 AM   Credit Card scarf              -15      0
        ///    14       5       1/11/2020 12:00:00 AM   Eating Out McDonalds            45      -45
        ///    14       7       1/12/2020 12:00:00 AM   Eating Out Wendys               25      -70
        ///    14      10       2/1/2020 12:00:00 AM    Eating Out Pizza                33.33   -103.33
        ///     9      13       2/10/2020 12:00:00 AM   Credit Card mittens            -15      -88.33
        ///     9      12       2/25/2020 12:00:00 AM   Credit Card Hat                -25      -63.33
        ///    14      11       2/27/2020 12:00:00 AM   Eating Out Pizza                33.33   -96.66
        ///    14       9       7/11/2020 12:00:00 AM   Eating Out Cafeteria            11.11   -107.77
        /// </code>
        /// 
        /// <b>Getting the list of ALL budget items, subdivided by month.</b>
        /// 
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget();
        /// budget.ReadFromFile(filename);
        /// 
        /// // Get a list of BudgetItemsByMonth
        /// var monthlyBudgets = budget.GetBudgetItemsByMonth(null, null, false, 0);
        /// 
        /// //Print important information
        /// foreach(var month in monthlyBudgets)
        /// {
        ///     Console.WriteLine(String.Format("======{0,7}======", month.Month));
        ///     
        ///     foreach(BudgetItem item in month.Details){
        ///         Console.WriteLine(String.Format("{0,2}, {1,-20}, {2,8:C}",
        ///             item.Date.Day.ToString(),
        ///             item.ShortDescription,
        ///             item.Amount));
        ///     }
        ///     
        ///     // Prints the monthly sub-total
        ///     Console.WriteLine(String.Format("MONTHLY SUBTOTAL: {0,8:C}", month.Total));
        /// }
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// ======2018/01======
        /// 10 hat (on credit)      -$10.00
        /// 11 hat                   $10.00
        /// MONTHLY SUBTOTAL:         $0.00
        /// ======2019/01======
        /// 10 scarf (on credit)    -$15.00
        /// MONTHLY SUBTOTAL:       -$15.00
        /// ======2020/01======
        /// 10 scarf                 $15.00
        /// 11 McDonalds            -$45.00
        /// 12 Wendys               -$25.00
        /// MONTHLY SUBTOTAL:       -$55.00
        /// ======2020/02======
        ///  1 Pizza                -$33.33
        /// 10 mittens               $15.00
        /// 25 Hat                   $25.00
        /// 27 Pizza                -$33.33
        /// MONTHLY SUBTOTAL:       -$26.66
        /// ======2020/07======
        /// 11 Cafeteria            -$11.11
        /// MONTHLY SUBTOTAL:       -$11.11
        /// </code>
        /// 
        /// <b>Getting a FILTERED list of budget items, subdivided by month</b>
        /// 
        /// For this example, the value of FilterFlag is set to true, and the Category ID is set to 9 ("Credit").
        /// 
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget();
        /// budget.ReadFromFile(filename);
        /// 
        /// // Get a list of BudgetItemsByMonth
        /// var monthlyBudgets = budget.GetBudgetItemsByMonth(null, null, true, 9);
        /// 
        /// //Print important information
        /// foreach(var month in monthlyBudgets)
        /// {
        ///     Console.WriteLine(String.Format("======{0,7}======", month.Month));
        ///     
        ///     foreach(BudgetItem item in month.Details){
        ///         Console.WriteLine(String.Format("{0,2}, {1,-20}, {2,8:C}",
        ///             item.Date.Day.ToString(),
        ///             item.ShortDescription,
        ///             item.Amount));
        ///     }
        ///     
        ///     // Prints the monthly sub-total
        ///     Console.WriteLine(String.Format("MONTHLY SUBTOTAL: {0,8:C}", month.Total));
        /// }
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// ======2018/01======
        /// 11 hat                      $10.00
        /// MONTHLY SUBTOTAL:           $10.00
        /// ======2020/01======
        /// 10 scarf                    $15.00
        /// MONTHLY SUBTOTAL:           $15.00
        /// ======2020/02======
        /// 10 mittens                  $15.00
        /// 25 Hat                      $25.00
        /// </code>
        /// </example>
        public List<BudgetItemsByMonth> GetBudgetItemsByMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            Start = Start ?? new DateTime(1900, 1, 1);
            string startString = Start.Value.ToString("yyyy-MM-dd");
            End = End ?? new DateTime(2500, 1, 1);
            string endString = End.Value.ToString("yyyy-MM-dd");

            List<int> years = new List<int>();
            List<int> months = new List<int>();

            StringBuilder stm = new StringBuilder();

            stm.Append("SELECT DISTINCT strftime('%Y', expenses.Date) as 'Year', strftime('%m', expenses.Date) as 'Month' FROM expenses INNER JOIN categories on expenses.CategoryId = expenses.Id WHERE expenses.Date >= " + startString.ToString() + "AND expenses.Date <= " + endString.ToString());

            if (FilterFlag == true)
            {
                stm.Append(" AND categories.Id = " + CategoryID);
            }

            stm.Append(" GROUP BY 'Year', 'Month';");

            using var cmd = new SQLiteCommand(stm.ToString(), _connection);

            using SQLiteDataReader reader = cmd.ExecuteReader();

            int count = 0;

            while (reader.Read())
            {
                years.Add(reader.GetInt32(0));
                months.Add(reader.GetInt32(1));
                count++;
            }

            List<BudgetItemsByMonth> itemsByMonth = new List<BudgetItemsByMonth>();

            for (int i = 0; i < count; i++)
            {
                List<BudgetItem> details = new List<BudgetItem>();

                Double total = 0;
                int startDate = 1;
                int endDate = DateTime.DaysInMonth(years[i], months[i]);

                List<BudgetItem> items = GetBudgetItems(new DateTime(years[i], months[i], startDate), new DateTime(years[i], months[i], endDate), FilterFlag, CategoryID);

                foreach (BudgetItem item in items)
                {
                    total += item.Amount;
                    details.Add(item);
                }

                itemsByMonth.Add(new BudgetItemsByMonth
                {
                    Month = new DateTime(years[i], months[i], startDate).ToString("MMMM"),
                    Details = details,
                    Total = total
                });
            }

            return itemsByMonth;
        }

        // ============================================================================
        // Group all expenses by category (ordered by category name)
        // ============================================================================
        /// <summary>
        /// Gets a list of budget item lists. Each list contains budget items belonging to a single category. The list of lists is arranged alphabetically by category name. The list can be restricted to a specific time frame, and can be filtered to exclude budget items which do not belong to a specific category.
        /// </summary>
        /// <param name="Start">The beginning of the desired time frame. Can be null.</param>
        /// <param name="End">The end of the desired time frame. Can be null.</param>
        /// <param name="FilterFlag">True if the list will filter out unwanted categories; false otherwise.</param>
        /// <param name="CategoryID">The ID number of the desired category. Must be included even if the list will not be filtered.</param>
        /// <returns>The list of BudgetItemsByCategory, ordered alphabetically by category name.</returns>
        /// <seealso cref="BudgetItemsByCategory"/>
        /// <example>
        /// For all examples below, assume the budget file contains the
        /// following elements:
        /// 
        /// <code>
        /// Cat_ID  Expense_ID  Date                    Description                    Cost     Balance
        ///    10       1       1/10/2018 12:00:00 AM   Clothes hat (on credit)         10      -10
        ///     9       2       1/11/2018 12:00:00 AM   Credit Card hat                -10      10
        ///    10       3       1/10/2019 12:00:00 AM   Clothes scarf(on credit)        15      -15
        ///     9       4       1/10/2020 12:00:00 AM   Credit Card scarf              -15      0
        ///    14       5       1/11/2020 12:00:00 AM   Eating Out McDonalds            45      -45
        ///    14       7       1/12/2020 12:00:00 AM   Eating Out Wendys               25      -70
        ///    14      10       2/1/2020 12:00:00 AM    Eating Out Pizza                33.33   -103.33
        ///     9      13       2/10/2020 12:00:00 AM   Credit Card mittens            -15      -88.33
        ///     9      12       2/25/2020 12:00:00 AM   Credit Card Hat                -25      -63.33
        ///    14      11       2/27/2020 12:00:00 AM   Eating Out Pizza                33.33   -96.66
        ///    14       9       7/11/2020 12:00:00 AM   Eating Out Cafeteria            11.11   -107.77
        /// </code>
        /// 
        /// <b>Getting a list of ALL budget items, subdivided by Category.</b>
        /// 
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget();
        /// budget.ReadFromFile(filename);
        /// 
        /// // Get a list of all budget items, organized by category
        /// var budgetByCategories = budget.GetBudgetItemsByCategory(null, null, false, 0);
        /// 
        /// // print important information
        /// foreach (var catGroup in budgetByCategories)
        /// {
        ///     Console.WriteLine(String.Format("===BUDGET ITEMS FOR {0} CATEGORY===", catGroup.Category));
        ///     
        ///     foreach(BudgetItem item in catGroup.Details)
        ///     {
        ///         Console.WriteLine(String.Format("{0} {1,-20} {2,8:C}".
        ///             item.Date.ToString(),
        ///             item.ShortDescription,
        ///             item.Amount));
        ///     }
        ///     
        ///     // Print the category sub-total
        ///     Console.WriteLine(String.Format("CATEGORY SUBTOTAL: {0,8:C}", catGroup.Total));
        /// }
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// ===BUDGET ITEMS FOR Clothes CATEGORY===
        /// 2018-01-10 12:00:00 AM hat (on credit)       -$10.00
        /// 2019-01-10 12:00:00 AM scarf (on credit)     -$15.00
        /// CATEGORY SUBTOTAL:  -$25.00
        /// ===BUDGET ITEMS FOR Credit Card CATEGORY===
        /// 2018-01-11 12:00:00 AM hat                    $10.00
        /// 2020-01-10 12:00:00 AM scarf                  $15.00
        /// 2020-02-10 12:00:00 AM mittens                $15.00
        /// 2020-02-25 12:00:00 AM Hat                    $25.00
        /// CATEGORY SUBTOTAL:   $65.00
        /// ===BUDGET ITEMS FOR Eating Out CATEGORY===
        /// 2020-01-11 12:00:00 AM McDonalds             -$45.00
        /// 2020-01-12 12:00:00 AM Wendys                -$25.00
        /// 2020-02-01 12:00:00 AM Pizza                 -$33.33
        /// 2020-02-27 12:00:00 AM Pizza                 -$33.33
        /// 2020-07-11 12:00:00 AM Cafeteria             -$11.11
        /// CATEGORY SUBTOTAL: -$147.77
        /// </code>
        /// 
        /// <b>Getting a FILTERED list of budget items, subdivided by category</b>
        /// 
        /// In this example, the FilterFlag is set to true, and the Category ID is set to 9 ("Credit"). Note that this is functionally the same as a regular filtered GetBudgetItems.
        /// 
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget();
        /// budget.ReadFromFile(filename);
        /// 
        /// // Get a list of all budget items, organized by category
        /// var budgetByCategories = budget.GetBudgetItemsByCategory(null, null, true, 9);
        /// 
        /// // print important information
        /// foreach (var catGroup in budgetByCategories)
        /// {
        ///     Console.WriteLine(String.Format("===BUDGET ITEMS FOR {0} CATEGORY===", catGroup.Category));
        ///     
        ///     foreach(BudgetItem item in catGroup.Details)
        ///     {
        ///         Console.WriteLine(String.Format("{0} {1,-20} {2,8:C}".
        ///             item.Date.ToString(),
        ///             item.ShortDescription,
        ///             item.Amount));
        ///     }
        ///     
        ///     // Print the category sub-total
        ///     Console.WriteLine(String.Format("CATEGORY SUBTOTAL: {0,8:C}", catGroup.Total));
        /// }
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// <code>
        /// ===BUDGET ITEMS FOR Credit Card CATEGORY===
        /// 2018-01-11 12:00:00 AM hat                   $10.00
        /// 2020-01-10 12:00:00 AM scarf                 $15.00
        /// 2020-02-10 12:00:00 AM mittens               $15.00
        /// 2020-02-25 12:00:00 AM Hat                   $25.00
        /// CATEGORY SUBTOTAL:   $65.00
        /// </code>
        /// </example>
        public List<BudgetItemsByCategory> GetBudgetItemsByCategory(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // First, get all categories that have associated expenses within the budget.
            List<String> categoryNames = new List<String>();
            List<int> categoryIDs = new List<int>();

            // Ensure Start and End have a fixed value if null. convert them into strings to avoid exceptions.
            Start = Start ?? new DateTime(1900, 1, 1);
            string startString = Start.Value.ToString("yyyy-MM-dd");
            End = End ?? new DateTime(2500, 1, 1);
            string endString = End.Value.ToString("yyyy-MM-dd");

            //string stm = "SELECT DISTINCT c.Description, c.Id from categories c inner join expenses e on c.Id = e.CategoryId order by c.Description ASC";

            StringBuilder stm = new StringBuilder();

            stm.Append("SELECT DISTINCT c.Description, c.Id from categories c" +
                " INNER JOIN expenses e ON c.Id = e.CategoryId" +
                $" WHERE e.Date >= \'{startString}\' AND e.Date <= \'{endString}\'");
            // if the user wants to filter the results by category, tweak the command.
            if (FilterFlag)
            {
                stm.Append($" AND c.Id = {CategoryID}");
            }
            stm.Append(" order by c.Description ASC;");

            using var cmd = new SQLiteCommand(stm.ToString(), _connection);
            using SQLiteDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string cat = reader.GetString(0);
                categoryNames.Add(cat);

                int id = reader.GetInt32(1);
                categoryIDs.Add(id);
            }

            // create list of BudgetItemsByCategory.
            var summary = new List<BudgetItemsByCategory>();

            // Cycle through each category in the budget.
            for (int i = 0; i < categoryIDs.Count; i++)
            {
                // Get list of all BudgetItems belonging to this category.
                List<BudgetItem> details = GetBudgetItems(Start, End, true, categoryIDs[i]);

                // Calculate total balance for this category.
                double total = 0;
                foreach (BudgetItem item in details)
                {
                    total += item.Amount;
                }

                // Add new BudgetItemsByCategory to the list.
                summary.Add(new BudgetItemsByCategory
                {
                    Category = categoryNames[i],
                    Details = details,
                    Total = total
                });
            }

            return summary;
        }

        // ============================================================================
        // Group all expenses by category and Month
        // creates a list of ExpandoObjects... which are objects that can have
        //   properties added to it on the fly.
        // ... for each element of the list (expenses by month), the ExpandoObject will have a property
        //     Month = (year/month) (string)
        //     Total = Double total for that month
        //     and for each category that had an entry in that month...
        //     1) Name of category , 
        //     2) and a property called "details: <name of category>" 
        //  
        // ... the last element of the list will contain an ExpandoObject
        //     with the properties for each category, equal to the totals for that
        //     category, and the name of the "Month" property will be "Totals"
        // ============================================================================
        /// <summary>
        /// Gets a list of dictionaries containing detailed information about the budget. Each dictionary covers one month of the budget, and has both in-depth and summary information on each category represented within that month. The last dictionary in the collection instead contains sub-totals for each category represented in the budget as a whole. 
        /// </summary>
        /// <param name="Start">The beginning of the desired time frame. Can be null.</param>
        /// <param name="End">The end of the desired time frame. Can be null.</param>
        /// <param name="FilterFlag">True if the dictionary will filter out unwanted categories; false otherwise.</param>
        /// <param name="CategoryID">The ID number of the desired Category. Must be included even if the dictonary will not be filtered.</param>
        /// <returns>The list of dictionaries.</returns>
        /// <example>
        /// For the examples below, assume the budget file contains the
        /// following elements:
        /// 
        /// <code>
        /// Cat_ID  Expense_ID  Date                    Description                    Cost     Balance
        ///    10       1       1/10/2018 12:00:00 AM   Clothes hat (on credit)         10      -10
        ///     9       2       1/11/2018 12:00:00 AM   Credit Card hat                -10      10
        ///    10       3       1/10/2019 12:00:00 AM   Clothes scarf(on credit)        15      -15
        ///     9       4       1/10/2020 12:00:00 AM   Credit Card scarf              -15      0
        ///    14       5       1/11/2020 12:00:00 AM   Eating Out McDonalds            45      -45
        ///    14       7       1/12/2020 12:00:00 AM   Eating Out Wendys               25      -70
        ///    14      10       2/1/2020 12:00:00 AM    Eating Out Pizza                33.33   -103.33
        ///     9      13       2/10/2020 12:00:00 AM   Credit Card mittens            -15      -88.33
        ///     9      12       2/25/2020 12:00:00 AM   Credit Card Hat                -25      -63.33
        ///    14      11       2/27/2020 12:00:00 AM   Eating Out Pizza                33.33   -96.66
        ///    14       9       7/11/2020 12:00:00 AM   Eating Out Cafeteria            11.11   -107.77
        /// </code>
        /// 
        /// <b>Getting a dictionary of ALL budget items, organized by month and category.</b>
        /// 
        /// Note that the dictionary's key-value pairs can have the following keys, in descending order:
        /// - "Month": Top-level. Value is a specific month of a specific year, or the word "Totals".
        /// - "Total": Value is the total earnings for the specified month. Equivalent to <see cref="Budget.BudgetItemsByMonth.Total"/>.
        /// - "details:[Category name]": Value is a list of BudgetItems which fall under the named category for this month. Repeats for each category which is represented in the current month.
        /// - "[Category name]": Value is the sum of the amounts of the above list of BudgetItems. Always comes after the corresponding "details:[Category name]" key-value pair.
        ///         - If the top-level "Month" key's value is "Totals", the value of "[Category name]" is instead the sum of all BudgetItems which match the corresponding category. Its value is equivalent to <see cref="Budget.BudgetItemsByCategory.Total"/>.
        /// 
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget();
        /// budget.ReadFromFile(filename);
        /// 
        /// // Get a list of dictionaries
        /// var budgetDictionaries = budget.GetBudgetDictionaryByCategoryAndMonth(null, null, false, 0);
        /// 
        /// // Print important information
        /// foreach(var dictionary in budgetDictionaries)
        /// {
        ///     // Pass over each key-value pair in the current dictionary.
        ///     foreach(KeyValuePair<string,object> valuePair in dictionary)
        ///     {
        ///         // If the key is "Month", treat it as a subheader.
        ///         if (valuePair.Key.Contains("Month"))
        ///         {
        ///             Console.WriteLine("====" + valuePair.Value + "====");
        ///         }
        ///         else if (valuePair.Key.Contains("details"))
        ///         {
        ///             // If the key contains any variation of the word "details", its value is a list of BudgetItems. Cast it as such.
        ///             Console.WriteLine(valuePair.Key);
        ///             List<BudgetItem> budgets = (List<BudgetItem>)valuePair.Value;
        ///             
        ///             // Loop over each BudgetItem in the list and print out its relevant information.
        ///             foreach(BudgetItem budgetItem in budgets)
        ///             {
        ///                 Console.WriteLine(String.Format("\t{0,-12} {1,10:C}", budgetItem.ShortDescription, budgetItem.Amount));
        ///             }
        ///         }
        ///         else if (valuePair.Key.Contains("Total"))
        ///         {
        ///             // If the key is "Total", its value is a monetary sub-total for the current month.
        ///             Console.WriteLine(string.Format("TOTAL: {0,10:C}", valuePair.Value));
        ///         }
        ///         else
        ///         {
        ///             // Treat any other key as having a sub-total for a specific category as a value. Said sub-total will either be for the current month or for the entire budget.
        ///             Console.WriteLine(string.Format("{0,-20}:{1,10:C}", valuePair.Key, valuePair.Value));
        ///         }
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// 
        /// Sample output: 
        /// 
        /// <code>
        /// ====2018/01====
        /// TOTAL:      $0.00
        /// details:Clothes
        ///        hat(on credit)    -$10.00
        /// Clothes             :   -$10.00
        /// details:Credit Card
        ///         hat              $10.00
        /// Credit Card         :    $10.00
        /// ====2019/01====
        /// TOTAL:    -$15.00
        /// details:Clothes
        ///        scarf(on credit)    -$15.00
        /// Clothes             :   -$15.00
        /// ====2020/01====
        /// TOTAL:    -$55.00
        /// details:Credit Card
        ///         scarf            $15.00
        /// Credit Card         :    $15.00
        /// details:Eating Out
        ///         McDonalds       -$45.00
        ///         Wendys          -$25.00
        /// Eating Out          :   -$70.00
        /// ====2020/02====
        /// TOTAL:    -$26.66
        /// details:Credit Card
        ///         mittens          $15.00
        ///         Hat              $25.00
        /// Credit Card         :    $40.00
        /// details:Eating Out
        ///         Pizza           -$33.33
        ///         Pizza           -$33.33
        /// Eating Out          :   -$66.66
        /// ====2020/07====
        /// TOTAL:    -$11.11
        /// details:Eating Out
        ///         Cafeteria       -$11.11
        /// Eating Out          :   -$11.11
        /// ====TOTALS====
        /// Credit Card         :    $65.00
        /// Clothes             :   -$25.00
        /// Eating Out          :  -$147.77
        /// </code>
        /// 
        /// <b>Getting a FILTERED list of dictionaries.</b>
        /// 
        /// For this example, the value of FilterFlag is set to true, and the category ID is set to 9 ("Credit").
        /// <code>
        /// <![CDATA[
        /// HomeBudget budget = new HomeBudget();
        /// budget.ReadFromFile(filename);
        /// 
        /// // Get a list of dictionaries
        /// var budgetDictionaries = budget.GetBudgetDictionaryByCategoryAndMonth(null, null, true, 9);
        /// 
        /// // Print important information
        /// foreach(var dictionary in budgetDictionaries)
        /// {
        ///     // Pass over each key-value pair in the current dictionary.
        ///     foreach(KeyValuePair<string,object> valuePair in dictionary)
        ///     {
        ///         // If the key is "Month", treat it as a subheader.
        ///         if (valuePair.Key.Contains("Month"))
        ///         {
        ///             Console.WriteLine("====" + valuePair.Value + "====");
        ///         }
        ///         else if (valuePair.Key.Contains("details"))
        ///         {
        ///             // If the key contains any variation of the word "details", its value is a list of BudgetItems. Cast it as such.
        ///             Console.WriteLine(valuePair.Key);
        ///             List<BudgetItem> budgets = (List<BudgetItem>)valuePair.Value;
        ///             
        ///             // Loop over each BudgetItem in the list and print out its relevant information.
        ///             foreach(BudgetItem budgetItem in budgets)
        ///             {
        ///                 Console.WriteLine(String.Format("\t{0,-12} {1,10:C}", budgetItem.ShortDescription, budgetItem.Amount));
        ///             }
        ///         }
        ///         else if (valuePair.Key.Contains("Total"))
        ///         {
        ///             // If the key is "Total", its value is a monetary sub-total for the current month.
        ///             Console.WriteLine(string.Format("TOTAL: {0,10:C}", valuePair.Value));
        ///         }
        ///         else
        ///         {
        ///             // Treat any other key as having a sub-total for a specific category as a value. Said sub-total will either be for the current month or for the entire budget.
        ///             Console.WriteLine(string.Format("{0,-20}:{1,10:C}", valuePair.Key, valuePair.Value));
        ///         }
        ///     }
        /// }
        /// ]]>
        /// </code>
        /// 
        /// Sample output:
        /// 
        /// <code>
        /// ====2018/01====
        /// TOTAL:     $10.00
        /// details:Credit Card
        ///         hat              $10.00
        /// Credit Card         :    $10.00
        /// ====2020/01====
        /// TOTAL:     $15.00
        /// details:Credit Card
        ///         scarf            $15.00
        /// Credit Card         :    $15.00
        /// ====2020/02====
        /// TOTAL:     $40.00
        /// details:Credit Card
        ///         mittens          $15.00
        ///         Hat              $25.00
        /// Credit Card         :    $40.00
        /// ====TOTALS====
        /// Credit Card         :    $65.00
        /// </code>
        /// </example>
        public List<Dictionary<string, object>> GetBudgetDictionaryByCategoryAndMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items by month 
            // -----------------------------------------------------------------------
            List<BudgetItemsByMonth> GroupedByMonth = GetBudgetItemsByMonth(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // loop over each month
            // -----------------------------------------------------------------------
            var summary = new List<Dictionary<string, object>>();
            var totalsPerCategory = new Dictionary<String, Double>();

            foreach (var MonthGroup in GroupedByMonth)
            {
                // create record object for this month
                Dictionary<string, object> record = new Dictionary<string, object>();
                record["Month"] = MonthGroup.Month;
                record["Total"] = MonthGroup.Total;

                // break up the month details into categories
                var GroupedByCategory = MonthGroup.Details.GroupBy(c => c.Category);

                // -----------------------------------------------------------------------
                // loop over each category
                // -----------------------------------------------------------------------
                foreach (var CategoryGroup in GroupedByCategory.OrderBy(g => g.Key))
                {

                    // calculate totals for the cat/month, and create list of details
                    double total = 0;
                    var details = new List<BudgetItem>();

                    foreach (var item in CategoryGroup)
                    {
                        total = total + item.Amount;
                        details.Add(item);
                    }

                    // add new properties and values to our record object
                    record["details:" + CategoryGroup.Key] = details;
                    record[CategoryGroup.Key] = total;

                    // keep track of totals for each category
                    if (totalsPerCategory.TryGetValue(CategoryGroup.Key, out Double CurrentCatTotal))
                    {
                        totalsPerCategory[CategoryGroup.Key] = CurrentCatTotal + total;
                    }
                    else
                    {
                        totalsPerCategory[CategoryGroup.Key] = total;
                    }
                }

                // add record to collection
                summary.Add(record);
            }
            // ---------------------------------------------------------------------------
            // add final record which is the totals for each category
            // ---------------------------------------------------------------------------
            Dictionary<string, object> totalsRecord = new Dictionary<string, object>();
            totalsRecord["Month"] = "TOTALS";

            foreach (var cat in categories.List())
            {
                try
                {
                    totalsRecord.Add(cat.Description, totalsPerCategory[cat.Description]);
                }
                catch { }
            }
            summary.Add(totalsRecord);


            return summary;
        }
        #endregion
    }
}
