﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget;

namespace WpfHomeBudget
{
    public class Presenter
    {
        // backing fields
        private IViewable viewable;
        HomeBudget budget;

        // constructor
        public Presenter(IViewable view)
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

        /// <summary>
        /// Tells the HomeBudget model to create a new Expense object.
        /// </summary>
        /// <remarks>
        /// Database is updated only if the passed data is valid. 
        /// If any data is invalid, the View will display an error message.
        /// </remarks>
        /// <param name="date">The date of the Expense.</param>
        /// <param name="category">The ID of the Expense's corresponding Category.</param>
        /// <param name="amount">The monetary amount of the Expense. Must be a number that can be parsed as a double.</param>
        /// <param name="description">A brief description of the Expense.</param>
        public void CreateNewExpense(DateTime? date, int category, string amount, string description)
        {
            if(ValidateExpenseInput(date, category, amount, description))
                budget.expenses.Add(date.Value, category, double.Parse(amount), description);
        }

        /// <summary>
        /// Validates data that is meant to be used to create an Expense object.
        /// If any of the data is invalid, method tells the View to display an appropriate error message.
        /// </summary>
        /// <param name="category">The ID of the desired Category.</param>
        /// <param name="description">A brief description of the Expense.</param>
        /// <returns>True if the data is valid, false otherwise.</returns>
        private bool ValidateExpenseInput(DateTime? date, int category, string amount, string description)
        {
            StringBuilder errorBuilder = new StringBuilder();

            // Did the user enter a date?
            if (!date.HasValue)
                errorBuilder.AppendLine("\'Date\' is a required field.");

            // Does the database include a category corresponding to this ID number?
            try
            {
                budget.categories.GetCategoryFromId(category);
            }
            catch (Exception e)
            {
                // GetCategoryFromId will throw an exception if no matching ID is found in the Categories table.
                errorBuilder.AppendLine(e.Message);
            }

            // Did the amount field contain anything?
            if (string.IsNullOrEmpty(amount))
                errorBuilder.AppendLine("\'Amount\' is a required field.");
            // If so, does it contain an actual number?
            else if (!double.TryParse(amount, out _))
                errorBuilder.AppendLine("\'Amount\' must contain a number. Do not enter letters or special characters.");

            // Is the description blank?
            if (string.IsNullOrEmpty(description))
                errorBuilder.AppendLine("\'Description\' is a required field.");

            //If any error messages were appended to the error builder, call the view's ShowError() method and return false.
            if (!string.IsNullOrEmpty(errorBuilder.ToString()))
            {
                viewable.ShowError(errorBuilder.ToString());
                return false;
            }

            return true;
        }

        public List<Category> GetCategories()
        {
            return budget.categories.List();
        }
    }
}