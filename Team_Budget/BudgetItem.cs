using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{
    // ====================================================================
    // CLASS: BudgetItem
    //        A single budget item, includes Category and Expense
    // ====================================================================

    /// <summary>
    /// Class representing a single item within a budget. Each BudgetItem is a combination of a <see cref="Budget.Category"/> and an <see cref="Expense"/>. 
    /// They are typically created by the <see cref="HomeBudget"/> class.
    /// </summary>
    /// <seealso cref="Budget.Category"/>
    /// <seealso cref="Budget.Expense"/>
    /// <seealso cref="BudgetItemsByMonth"/>
    /// <seealso cref="BudgetItemsByCategory"/>
    public class BudgetItem
    {
        /// <summary>
        /// Gets or sets the ID number of the budget item's category. See <see cref="Budget.Category.Id"/>.
        /// </summary>
        public int CategoryID { get; set; }
        /// <summary>
        /// Gets or sets the ID of the budget item's <see cref="Expense">expense</see>. See <see cref="Budget.Expense.Id"/>.
        /// </summary>
        public int ExpenseID { get; set; }
        /// <summary>
        /// Gets or sets the date on which the expense occurred. See <see cref="Expense.Date"/>
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// Gets or sets a brief description of the budget item's <see cref="Budget.Category">category</see>. See <see cref="Budget.Category.Description"/>
        /// </summary>
        public String Category { get; set; }
        /// <summary>
        /// Gets or sets a brief description of the expense. See <see cref="Expense.Description"/>.
        /// </summary>
        public String ShortDescription { get; set; }
        /// <summary>
        /// Gets or sets the monetary amount of the budget item. Amount is the inverse of the Expense's amount: i.e., an amount of -15 in Expense becomes 15 in BudgetItem.
        /// </summary>
        public Double Amount { get; set; }
        /// <summary>
        /// Gets or sets the balance remaining on the budget after the item's Amount is deducted from the total.
        /// </summary>
        public Double Balance { get; set; }

    }

    /// <summary>
    /// Class representing a collection of <see cref="BudgetItem"/>s that occurred in a specific month of a specific year. They are typically created by the <see cref="HomeBudget"/> class.
    /// </summary>    
    /// <seealso cref="HomeBudget"/>
    /// <see cref="BudgetItem"/>
    public class BudgetItemsByMonth
    {
        /// <summary>
        /// Gets or sets the year and month in which the collected BudgetItems occurred. Recommended format is "YYYY/MM".
        /// </summary>
        public String Month { get; set; }
        /// <summary>
        /// Gets or sets the list of BudgetItems for the specified month.
        /// </summary>
        public List<BudgetItem> Details { get; set; }
        /// <summary>
        /// Gets or sets the total budget for the specified month.
        /// </summary>
        public Double Total { get; set; }
    }


    /// <summary>
    /// Class representing a collection of budget items which belong to a common <see cref="Category"/>. They are typically created by the <see cref="HomeBudget"/> class.
    /// </summary>
    /// <seealso cref="HomeBudget"/>
    /// <see cref="BudgetItem"/>
    public class BudgetItemsByCategory
    {
        /// <summary>
        /// Gets or sets the <see cref="Category"/> to which the collected BudgetItems belong.
        /// </summary>
        public String Category { get; set; }
        /// <summary>
        /// Gets or sets the list of BudgetItems belonging to the specified category.
        /// </summary>
        public List<BudgetItem> Details { get; set; }
        /// <summary>
        /// Gets or sets the total budget for the specified category.
        /// </summary>
        public Double Total { get; set; }

    }


}
