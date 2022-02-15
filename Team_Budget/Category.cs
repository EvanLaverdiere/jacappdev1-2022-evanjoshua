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
    // CLASS: Category
    //        - An individual category for budget program
    //        - Valid category types: Income, Expense, Credit, Saving
    // ====================================================================
    /// <summary>
    /// Class representing a category within a budget. Categories are broadly grouped into four types: Income, Expense, Credit, and Savings. Categories can be collected within the <see cref="Categories"/> class.
    /// </summary>
    /// <seealso cref="CategoryType"/>
    /// <seealso cref="Categories"/>
    public class Category
    {
        // ====================================================================
        // Properties
        // ====================================================================
        /// <summary>
        /// Gets or sets the ID number of the category.
        /// </summary>
        public int Id { get; private set; }
        /// <summary>
        /// Gets or sets the description of the category.
        /// </summary>
        public String Description { get; private set; }
        /// <summary>
        /// Gets or sets the type of the category.
        /// </summary>
        public CategoryType Type { get; set; }
        /// <summary>
        /// A list of valid options for the category's type. Available options are Income (0), Expense (1), Credit (2), and Savings (3).
        /// </summary>
        public enum CategoryType
        {
            /// <summary>
            /// Category represents income.
            /// </summary>
            Income,
            /// <summary>
            /// Category represents an expense.
            /// </summary>
            Expense,
            /// <summary>
            /// Category represents a credit.
            /// </summary>
            Credit,
            /// <summary>
            /// Category represents savings.
            /// </summary>
            Savings
        };

        // ====================================================================
        // Constructor
        // ====================================================================
        /// <summary>
        /// Creates a new Category object based on passed values.
        /// </summary>
        /// <param name="id">The ID number of the category.</param>
        /// <param name="description">A brief description of the category.</param>
        /// <param name="type">The type of category. If no value is assigned, defaults to Expense.</param>
        /// <example>
        /// In this example, a Category object representing legal fees is created.
        /// <code>
        /// int id = 1;
        /// String description = "Legal Fees";
        /// CategoryType type = CategoryType.Expense;
        /// 
        /// Category legal = new Category(id, description, type);
        /// </code>
        /// </example>
        public Category(int id, String description, CategoryType type = CategoryType.Expense)
        {
            this.Id = id;
            this.Description = description;
            this.Type = type;
        }

        // ====================================================================
        // Copy Constructor
        // ====================================================================
        /// <summary>
        /// Creates a new Category object by copying an existing one. The duplicated Category can then be manipulated without affecting the original object.
        /// </summary>
        /// <param name="category">The Category object to be copied.</param>
        /// <example>
        /// In this example, a Category object representing legal fees is created. It is then copied into a new Category using this constructor.
        /// <code>
        /// int id = 1;
        /// String description = "Legal Fees";
        /// CategoryType type = CategoryType.Expense;
        /// 
        /// Category legal = new Category(id, description, type);
        /// Category copiedLegal = new Category(legal);
        /// </code>
        /// </example>
        public Category(Category category)
        {
            this.Id = category.Id;;
            this.Description = category.Description;
            this.Type = category.Type;
        }
        // ====================================================================
        // String version of object
        // ====================================================================
        /// <summary>
        /// Returns a brief description of the category.
        /// </summary>
        /// <returns>The description of the category.</returns>
        /// <example>
        /// In this example, a Category object is created. Its string representation is then printed to the console.
        /// <code>
        /// Category category = new Category(18, "Legal Fees", Category.CategoryType.Expense);
        /// Console.WriteLine(category.ToString());
        /// </code>
        /// </example>
        public override string ToString()
        {
            return Description;
        }

    }
}

