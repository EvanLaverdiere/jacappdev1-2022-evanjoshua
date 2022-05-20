using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget;

namespace EnterpriseBudget.ChairpersonControl
{
    public class Presenter
    {
        private IDisplayable viewable, displayable;
        private HomeBudget budget;
        private bool modified = false;
        private int selectCount = 0;
        private string filePath = Environment.ExpandEnvironmentVariables(@"%APPDATA%\\EnterpriseBudget\\deptBudget.db");

        public Presenter(IDisplayable view, IDisplayable display)
        {
            viewable = view;
            displayable = display;
        }

        public void CreateBudget(string dbName, bool newDatabase)
        {
            budget = new HomeBudget(dbName, newDatabase);
        }

        /// <summary>
        /// Retrieves a list of categories in the Home Budget.
        /// </summary>
        /// <returns>The list of categories.</returns>
        public List<Category> GetCategories()
        {
            return budget.categories.List();
        }

        /// <summary>
        /// Tells the HomeBudget model to create a new Expense object.
        /// </summary>
        /// <remarks>
        /// Database is updated only if the passed data is valid. 
        /// If any data is invalid, the View will display an error message.
        /// If creating this Expense would put the user over-budget, the View will display an error message.
        /// </remarks>
        /// <param name="date">The date of the Expense.</param>
        /// <param name="category">The ID of the Expense's corresponding Category.</param>
        /// <param name="amount">The monetary amount of the Expense. Must be a number that can be parsed as a double.</param>
        /// <param name="description">A brief description of the Expense.</param>
        /// <param name="limit">The budget limit for the selected Category. Used to verify that an Expense won't go over-budget.</param>
        public void CreateNewExpense(DateTime? date, int category, string amount, string description, double limit)
        {
            if (ValidateExpenseInput(date, category, amount, description))
            {
                try
                {
                    // Will we go overbudget if we add this expense?
                    if(isWithinBudget(limit, double.Parse(amount), category)){
                        // If not, the expense can safely be added to the database.
                        budget.expenses.Add(date.Value, category, double.Parse(amount), description);

                        // Display a message indicating the Expense was successfully added.
                        viewable.ShowSuccess($"Successfully added \'{description}\' expense to the database.");
                    }
                    else
                    {
                        // Otherwise, display an error message stating that this Expense cannot be added.
                        viewable.ShowError("Cannot add this Expense! It will exceed the budget limit for this category.");
                    }
                }
                catch (Exception e)
                {
                    viewable.ShowError("Error adding expense: " + e.ToString());
                }
            }
        }


        /// <summary>
        /// Validates data that is meant to be used to create an Expense object.
        /// If any of the data is invalid, method tells the View to display an appropriate error message.
        /// </summary>
        /// <param name="date">The date of the Expense.</param>
        /// <param name="category">The ID of the desired Category.</param>
        /// <param name="amount">The monetary amount of the expense. Must be in a numerical format that can be parsed as a double.</param>
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

            // If no messages were appended, everything is good. Return true.
            return true;
        }

        /// <summary>
        /// Retrieves a list of category types for classifying expenses.
        /// </summary>
        /// <returns>The list of category types.</returns>
        public List<string> GetCategoryTypes()
        {
            List<string> categoryTypes = new List<string>();

            foreach (string type in Enum.GetNames(typeof(Category.CategoryType)))
            {
                categoryTypes.Add(type);
            }

            return categoryTypes;
        }


        public bool Modified()
        {
            return modified;
        }

        /// <summary>
        /// Retrieves a list of <see cref="BudgetItem"/>s from the HomeBudget and tells the View to display said list.
        /// </summary>
        /// <remarks>
        /// The list can be filtered to display budget items from within a specific time frame,
        /// budget items belonging to a specific category, or both.
        /// </remarks>
        /// <param name="start">The beginning of the desired time frame. Can be null.</param>
        /// <param name="end">The end of the desired time frame. Can be null.</param>
        /// <param name="filterFlag">True if the results are to be filtered by category, false otherwise.</param>
        /// <param name="categoryId">The ID of the desired category.</param>
        public void GetBudgetItems(DateTime? start, DateTime? end, bool filterFlag, int categoryId)
        {
            List<BudgetItem> budgetItems = budget.GetBudgetItems(start, end, filterFlag, categoryId);
            string displayType = displayable.GetDisplayType();
            if (displayType == "Pie Chart")
            {
                displayable.DisplayToChart(budgetItems.Cast<object>().ToList());
            }
            else
            {
                displayable.DisplayToGrid(budgetItems);
            }
        }

        /// <summary>
        /// Retrieves a list of <see cref="Budget.BudgetItemsByMonth"/> and tells the view to display said list.
        /// </summary>
        /// <remarks>
        /// The list can be filtered to display BudgetItemsByMonth from within a specific time frame,
        /// BudgetItemsByMonth belonging to a specific category, or both.
        /// </remarks>
        /// <param name="start">The beginning of the desired time frame. Can be null.</param>
        /// <param name="end">The end of the desired time frame. Can be null.</param>
        /// <param name="filterFlag">True if the results are to be filtered by category, false otherwise.</param>
        /// <param name="categoryId">The ID of the desired category.</param>
        public void GetBudgetItemsByMonth(DateTime? start, DateTime? end, bool filterFlag, int categoryId)
        {
            List<BudgetItemsByMonth> budgetItems = budget.GetBudgetItemsByMonth(start, end, filterFlag, categoryId);
            string displayType = displayable.GetDisplayType();
            if (displayType == "Pie Chart")
            {
                displayable.DisplayToChart(budgetItems.Cast<object>().ToList());
            }
            else
            {
                displayable.DisplayToGrid(budgetItems);
            }
        }

        /// <summary>
        /// Retrieves a list of <see cref="Budget.BudgetItemsByCategory"/> and tells the view to display said list.
        /// </summary>
        /// <remarks>
        /// The list can be filtered to display BudgetItemsByCategory from within a specific time frame,
        /// BudgetItemsByCategory belonging to a specific category, or both.
        /// </remarks>
        /// <param name="start">The beginning of the desired time frame. Can be null.</param>
        /// <param name="end">The end of the desired time frame. Can be null.</param>
        /// <param name="filterFlag">True if the results are to be filtered by category, false otherwise.</param>
        /// <param name="categoryId">The ID of the desired category.</param>
        public void GetBudgetItemsByCategory(DateTime? start, DateTime? end, bool filterFlag, int categoryId)
        {
            List<BudgetItemsByCategory> budgetItems = budget.GetBudgetItemsByCategory(start, end, filterFlag, categoryId);
            string displayType = displayable.GetDisplayType();
            if (displayType == "Pie Chart")
            {
                displayable.DisplayToChart(budgetItems.Cast<object>().ToList());
            }
            else
            {
                displayable.DisplayToGrid(budgetItems);
            }
        }

        /// <summary>
        /// Retrieves a list of dictionaries representing a breakdown of the budget by category and month, and tells the view to display said list.
        /// </summary>
        /// <remarks>
        /// The list can be filtered to display budget dictionaries belonging to a specific time frame,
        /// budget dictionaries belonging to a specific category, or both.
        /// </remarks>
        /// <param name="start">The beginning of the desired time frame. Can be null.</param>
        /// <param name="end">The end of the desired time frame. Can be null.</param>
        /// <param name="filterFlag">True if the results are to be filtered by category, false otherwise.</param>
        /// <param name="categoryId">The ID of the desired category.</param>
        public void GetBudgetDictionaryByCategoryAndMonth(DateTime? start, DateTime? end, bool filterFlag, int categoryId)
        {
            List<Dictionary<string, object>> budgetItems = budget.GetBudgetDictionaryByCategoryAndMonth(start, end, filterFlag, categoryId);
            string displayType = displayable.GetDisplayType();
            if (displayType == "Pie Chart")
            {
                List<Category> categories = GetCategories();
                List<string> categoryNames = new List<string>();
                foreach (Category category in categories)
                {
                    categoryNames.Add(category.Description);
                }
                displayable.InitializeByCategoryAndMonthDisplay(categoryNames);
                displayable.DisplayToChart(budgetItems.Cast<object>().ToList());
            }
            else if (displayable.isOrderedByMonthAndCategory())
            {
                displayable.DisplayToGrid(budgetItems);
            }


        }

        /// <summary>
        /// Updates the View's display with a list of budget items.
        /// </summary>
        /// <remarks>
        /// Does so in one of four ways, based on the combination of the values of the boolean parameters "orderByCategory" and "orderByMonth".
        /// 
        /// Other parameters are used to filter the list of budget items by time frame, by category, or both.
        /// </remarks>
        /// <param name="start">The beginning of the desired time frame. Can be null.</param>
        /// <param name="end">The end of the desired time frame. Can be null.</param>
        /// <param name="filterFlag">True if the results are to be filtered by category, false otherwise.</param>
        /// <param name="categoryId">The ID of the desired category.</param>
        /// <param name="orderByCategory">True if the budget is to be summarized by Category, false otherwise.</param>
        /// <param name="orderByMonth">True if the budget is to be summarized by Month, false otherwise.</param>
        public void UpdateDisplay(DateTime? start, DateTime? end, bool filterFlag, int categoryId, bool orderByCategory, bool orderByMonth)
        {
            // If both variables are true, send back a list of Budget dictionaries by Category and Month.
            if (orderByCategory && orderByMonth)
                GetBudgetDictionaryByCategoryAndMonth(start, end, filterFlag, categoryId);

            // If only orderByCategory is true, send back a list of BudgetItemsByCategory.
            else if (orderByCategory && !orderByMonth)
                GetBudgetItemsByCategory(start, end, filterFlag, categoryId);

            // If only orderByMonth is true, send back a list of BudgetItemsByMonth.
            else if (!orderByCategory && orderByMonth)
                GetBudgetItemsByMonth(start, end, filterFlag, categoryId);

            // If both variables are false, send back a list of regular BudgetItems.
            else
                GetBudgetItems(start, end, filterFlag, categoryId);
        }

        public void Search(string text, List<int> indexes, List<string> items, List<string> amounts)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Contains(text))
                {
                    indexes.Add(i);
                }
                else if (amounts[i].Contains(text))
                {
                    indexes.Add(i);
                }
            }

            if (selectCount == indexes.Count)
            {
                selectCount = 0;
            }

            if (indexes.Count == 0)
            {
                viewable.ShowError("No matching items were found");
            }
            else if (indexes.Count == 1)
            {
                viewable.Select(indexes[0]);
            }
            else
            {
                viewable.Select(indexes[selectCount++]);
            }
        }

        /// <summary>
        /// Retrieves the total amounts for all Expenses belonging to a specified Category.
        /// </summary>
        /// <param name="categoryId">The ID of the desired Category.</param>
        /// <returns></returns>
        public double getTotalForCategory(int categoryId)
        {
            double total = 0;
            //SqlDataReader rdr;

            //try
            //{
            //    SqlCommand getCatTotal = Model.Connection.cnn.CreateCommand();

            //    getCatTotal.CommandText = "SELECT SUM(Amount) " +
            //        "FROM expenses " +
            //        "WHERE CategoryId = @categoryId";

            //    getCatTotal.Parameters.AddWithValue("@categoryId", categoryId);

            //    rdr = getCatTotal.ExecuteReader();

            //    if (rdr.HasRows)
            //    {
            //        rdr.Read();

            //        total = rdr.GetDouble(0);
            //    }
            //}
            //catch (Exception)
            //{

            //    throw;
            //}

            // Retrieve a list of Expenses from the budget, since we can't directly query the database without revamping the model.
            List<Expense> expenses = budget.expenses.List();

            // Cycle through these Expenses.
            foreach(Expense expense in expenses)
            {
                // Add the Amount of each Expense that matches the passed Category to the total.
                if (expense.Category == categoryId)
                    total += expense.Amount;
            }

            // Return the total.
            return total;
        }

        /// <summary>
        /// Verifies that a passed amount will not exceed the budget for a specified Category.
        /// </summary>
        /// <param name="limit">The budget limit for the specified Category.</param>
        /// <param name="amount">The amount of an Expense to be created.</param>
        /// <param name="categoryId">The ID of the desired Category.</param>
        /// <returns>True if adding this amount will keep the total expenses within the budget, or false otherwise.</returns>
        private bool isWithinBudget(double limit, double amount, int categoryId)
        {
            // Get the current total of all expenses belonging to this Category.
            double currentTotal = getTotalForCategory(categoryId);

            // Add the passed amount to this total.
            double projectedTotal = amount + currentTotal;

            // Return true if the projected total is within the limit, or false otherwise.
            return projectedTotal <= limit;
        }

        public void SaveBudget(List<BudgetItem> items)
        {
            using var con = new SQLiteConnection("Data source=" + filePath);
            con.Open();
            using var cmd = new SQLiteCommand(con);

            cmd.CommandText = "DELETE FROM expenses;";
            cmd.ExecuteNonQuery();

            for (int i = 0; i < items.Count; i++)
            {
                cmd.CommandText = "INSERT INTO expenses(Id, Date, Amount, Description, CategoryId) VALUES(@Id, @Date, @Amount, @Description, @CategoryId);";
                cmd.Parameters.AddWithValue("@Id", items[i].ExpenseID);
                cmd.Parameters.AddWithValue("@Date", items[i].Date.ToString());
                cmd.Parameters.AddWithValue("@Amount", items[i].Amount);
                cmd.Parameters.AddWithValue("@Description", items[i].ShortDescription);
                cmd.Parameters.AddWithValue("@CategoryId", items[i].CategoryID);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }

            con.Close();

            SqlCommand saveToSqlServer = Model.Connection.cnn.CreateCommand();
            saveToSqlServer.CommandText = "UPDATE deptBudgets SET sqlitefile = @sqlitefile WHERE deptId = @deptId";
            saveToSqlServer.Parameters.Add("@deptId", SqlDbType.Int).Value = 1;
            saveToSqlServer.Parameters.Add("@sqlitefile", SqlDbType.Binary).Value = File.ReadAllBytes(filePath);
            saveToSqlServer.ExecuteNonQuery();
        }
    }
}
