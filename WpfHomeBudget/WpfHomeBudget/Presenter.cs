using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget;

namespace WpfHomeBudget
{
    class Presenter
    {
        // backing fields
        private ViewInterface viewable;
        HomeBudget budget;

        // constructor
        public Presenter(ViewInterface view)
        {
            viewable = view;
            //budget = new HomeBudget();
        }
        // methods
        /// <summary>
        /// Initializes a budget database.
        /// </summary>
        /// <remarks>Creates a new database or opens an existing database based on boolean value passed in from the view.</remarks>
        /// <param name="dbName">The database filename.</param>
        /// <param name="newDatabase">True if this is a new database, false if this is an existing database.</param>
        public void CreateBudget(string dbName, bool newDatabase)
        {
            budget = new HomeBudget(dbName, newDatabase);
        }

        public void CreateNewCategory(string categoryName, int categoryType)
        {
            budget.categories.Add(categoryName, (Category.CategoryType)categoryType);
        }

        public void CreateNewExpense(DateTime date, int category, Double amount, string description)
        {
            if(ValidateExpenseInput(date, category, amount, description))
                budget.expenses.Add(date, category, amount, description);
        }

        private bool ValidateExpenseInput(DateTime date, int category, double amount, string description)
        {
            StringBuilder errorBuilder = new StringBuilder();
            // Does the database include a category corresponding to this ID number?
            try
            {
                budget.categories.GetCategoryFromId(category);
            }
            catch (Exception e)
            {
                errorBuilder.AppendLine(e.Message);
            }

            // Is the description blank?
            if (String.IsNullOrEmpty(description))
                errorBuilder.AppendLine("\'Description\' is a required field.");

            //If any error messages were appended to the error builder, call the ShowError() method and return false.
            if (!String.IsNullOrEmpty(errorBuilder.ToString()))
            {
                viewable.ShowError(errorBuilder.ToString());
                return false;
            }

            return true;
        }
    }
}
