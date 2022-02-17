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
    // CLASS: Expense
    //        - An individual expens for budget program
    // ====================================================================
    /// <summary>
    /// Class representing a single expense within a budget. Expenses can be grouped within the <see cref="Expenses"/> class.
    /// </summary>
    /// <seealso cref="Expenses"/>
    /// 
    public class Expense
    {
        // ====================================================================
        // Properties
        // ====================================================================
        /// <summary>
        /// Gets the ID number of the Expense.
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// Gets the date on which the Expense was incurred.
        /// </summary>
        public DateTime Date { get; }
        /// <summary>
        /// Gets or sets the amount of money spent on the Expense.
        /// </summary>
        public Double Amount { get; }
        /// <summary>
        /// Gets or sets the description of the Expense.
        /// </summary>
        public String Description { get; }
        /// <summary>
        /// Gets or sets the category of the Expense.
        /// </summary>
        public int Category { get; }

        // ====================================================================
        // Constructor
        //    NB: there is no verification the expense category exists in the
        //        categories object
        // ====================================================================
        /// <summary>
        /// Creates an Expense object using passed values.
        /// </summary>
        /// <param name="id">The ID of the expense.</param>
        /// <param name="date">The date on which the expense occurred.</param>
        /// <param name="category">A number representing category of the expense.</param>
        /// <param name="amount">The cost or monetary amount of the expense.</param>
        /// <param name="description">A brief description of the expense.</param>
        /// <example>
        /// In this example, an expense representing a one-hour consultation with an electrician is created.
        /// <code>
        /// int id = 1;
        /// DateTime time = new DateTime(2021, 10, 31); // initializes a DateTime object with a date of October 31st, 2021.
        /// int category = 1; // In a default list of categories, this value corresponds to the Utilities category.
        /// Double amount = 100; // The cost of this hour-long consultation was $100.
        /// String description = "Electrician Consultation"; 
        /// 
        /// Expense consultation = new Expense(id, time, category, amount, description);
        /// </code>
        /// </example>
        public Expense(int id, DateTime date, int category, Double amount, String description)
        {
            this.Id = id;
            this.Date = date;
            this.Category = category;
            this.Amount = amount;
            this.Description = description;
        }

        // ====================================================================
        // Copy constructor - does a deep copy
        // ====================================================================
        /// <summary>
        /// Creates a new Expense object by copying an existing one. The duplicated Expense can then be manipulated without affecting the original object.
        /// </summary>
        /// <param name="obj">An Expense object to be copied.</param>
        /// <example>
        /// In this example, an Expense object representing a consultation with an electrician is created. It is then cloned into a new Expense using this constructor.
        /// <code>
        /// int id = 1;
        /// DateTime time = new DateTime(2021, 10, 31); // initializes a DateTime object with a date of October 31st, 2021.
        /// int category = 1; // In a default list of categories, this value corresponds to the Utilities category.
        /// Double amount = 100; // The cost of this hour-long consultation was $100.
        /// String description = "Electrician Consultation"; 
        /// 
        /// Expense consultation = new Expense(id, time, category, amount, description);
        /// Expense copiedConsultation = new Expense(consultation); 
        /// </code>
        /// </example>
        public Expense (Expense obj)
        {
            this.Id = obj.Id;
            this.Date = obj.Date;
            this.Category = obj.Category;
            this.Amount = obj.Amount;
            this.Description = obj.Description;
           
        }
    }
}
