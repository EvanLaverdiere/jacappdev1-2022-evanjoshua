﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfHomeBudget;
using Xunit;
using WpfHomeBudget.Interfaces;

namespace TestProject
{
    /// <summary>
    /// Mock class representing a View object which implements the IViewable interface.
    /// </summary>
    public class TestView : IViewable, IDisplayable
    {
        public bool calledClearError = false;
        public bool calledClearForm = false;
        public bool calledClearSelection = false;
        public bool calledRefresh = false;
        public bool calledShowError = false;
        public bool calledShowSuccess = false;
        public bool calledSelect = false;
        public bool calledGetDisplayType = false;
        public bool calledIsOrderedByMonthAndCategory = false;
        public bool calledDisplayToGrid = false;
        public bool calledDisplayToChart = false;
        public bool calledInitializeByCategoryAndMonthDisplay = false;

        public void ShowError(string error)
        {
            //throw new NotImplementedException();
            calledShowError = true;
        }

        public void ShowSuccess(string message)
        {
            //throw new NotImplementedException();
            calledShowSuccess = true;
        }

        public void turnDark()
        {
            throw new NotImplementedException();
        }

        public void turnLight()
        {
            throw new NotImplementedException();
        }

        public void Select(int index)
        {
            calledSelect = true;
        }

        public string GetDisplayType()
        {
            calledGetDisplayType = true;
            return " ";
        }

        public bool isOrderedByMonthAndCategory()
        {
            calledIsOrderedByMonthAndCategory = true;
            return true;
        }

        public void DisplayToGrid<T>(List<T> budgetItems)
        {
            calledDisplayToGrid = true;
        }

        public void DisplayToChart(List<object> budgetItems)
        {
            calledDisplayToChart = true;
        }

        public void InitializeByCategoryAndMonthDisplay(List<string> categoryNames)
        {
            calledInitializeByCategoryAndMonthDisplay = true;
        }
    }
    /// <summary>
    /// Class for testing the UI functionality of the Presenter object.
    /// </summary>
    public class TestPresenter
    {
        /// <summary>
        /// The filename to be used as a test database. 
        /// </summary>
        /// <remarks>Must be manually added to the TestProject folder, as it is not tracked in GitHub.</remarks>
        private const string TEST_DB_INPUT_FILE = "testDBInput.db";



        [Fact]
        public void TestConstructor()
        {
            TestView testView = new TestView();
            Presenter presenter = new Presenter(testView, testView);
            Assert.IsType<Presenter>(presenter);
        }

        [Fact]
        public void Test_CreateNewExpenseCallsShowErrorForBadInput()
        {
            // Arrange
            TestView testView = new TestView();
            Presenter presenter = new Presenter(testView, testView);
            DateTime? badDate = null;
            int badCategory = -1;
            string noAmount = "";
            string noDescription = "";

            // Act
            presenter.CreateNewExpense(badDate, badCategory, noAmount, noDescription);

            // Assert
            Assert.True(testView.calledShowError);
        }

        [Fact]
        public void Test_CreateNewCategoryCallsShowErrorForBadInput()
        {
            // Arrange
            TestView testView = new TestView();
            Presenter presenter = new Presenter(testView, testView);
            string badDescription = "";
            int badCategoryType = -1;

            // Act
            presenter.CreateNewCategory(badDescription, badCategoryType);

            // Assert
            Assert.True(testView.calledShowError);
        }

        [Fact]
        public void Test_CreateNewExpenseCallsShowSuccessForSuccessfulOperation()
        {
            // Arrange
            // Initialize the view & presenter.
            TestView testView = new TestView();
            Presenter presenter = new Presenter(testView, testView);

            // manually create a database. Copied from one of Sandy's HomeBudget_GetBudgetItemsByMonth tests.
            string folder = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            String goodDB = $"{folder}\\{TEST_DB_INPUT_FILE}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            presenter.CreateBudget(messyDB, false);


            DateTime? date = DateTime.Today; // get the current date
            int categoryId = 1; // In default Categories table, this value corresponds to the Utilities category.
            string amount = "49.99"; // The CreateNewExpense() method will try to parse this as a double.
            string description = "Hydro Bill";

            // Act
            presenter.CreateNewExpense(date, categoryId, amount, description);

            // Assert
            Assert.True(testView.calledShowSuccess);
        }

        [Fact]
        public void Test_CreateNewCategoryCallsShowSuccessForSuccessfulOperation()
        {
            // Arrange
            // Initialize the view & presenter.
            TestView testView = new TestView();
            Presenter presenter = new Presenter(testView, testView);

            // manually create a database. Copied from one of Sandy's HomeBudget_GetBudgetItemsByMonth tests.
            string folder = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            String goodDB = $"{folder}\\{TEST_DB_INPUT_FILE}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            presenter.CreateBudget(messyDB, false);

            string description = "Garply Category";
            int categoryType = 2;

            // Act
            presenter.CreateNewCategory(description, categoryType);

            // Assert
            Assert.True(testView.calledShowSuccess);
        }

        [Fact]
        public void Test_GetBudgetItemsCallsShowBudgetItems()
        {
            // Arrange
            TestView testView = new TestView();
            Presenter presenter = new Presenter(testView, testView);

            // Manually create a database so the Presenter can actually retrieve something.
            string folder = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            String goodDB = $"{folder}\\{TEST_DB_INPUT_FILE}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            presenter.CreateBudget(messyDB, false);

            // Act
            presenter.GetBudgetItems(null, null, false, 0);

            // Assert
            Assert.True(testView.calledGetDisplayType);
        }

        [Fact]
        public void Test_GetBudgetItemsByCategoryCallGetDisplayType()
        {
            // Arrange
            TestView testView = new TestView();
            Presenter presenter = new Presenter(testView, testView);

            // Manually create a database so the Presenter can actually retrieve something.
            string folder = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            String goodDB = $"{folder}\\{TEST_DB_INPUT_FILE}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            presenter.CreateBudget(messyDB, false);

            // Assert
            presenter.GetBudgetItemsByCategory(null, null, false, 0);

            // Act
            Assert.True(testView.calledGetDisplayType);
        }

        [Fact]
        public void Test_GetBudgetItemsByMonthCallscGetDisplayType()
        {
            // Arrange
            TestView testView = new TestView();
            Presenter presenter = new Presenter(testView, testView);

            // Manually create a database so the Presenter can actually retrieve something.
            string folder = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            String goodDB = $"{folder}\\{TEST_DB_INPUT_FILE}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            presenter.CreateBudget(messyDB, false);

            // Assert
            presenter.GetBudgetItemsByMonth(null, null, false, 0);

            // Act
            Assert.True(testView.calledGetDisplayType);

        }

        [Fact]
        public void Test_GetBudgetDictionaryByCategoryAndMonthCallsGetDisplayType()
        {
            // Arrange
            TestView testView = new TestView();
            Presenter presenter = new Presenter(testView, testView);

            // Manually create a database so the Presenter can actually retrieve something.
            string folder = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            String goodDB = $"{folder}\\{TEST_DB_INPUT_FILE}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            presenter.CreateBudget(messyDB, false);

            // Assert
            presenter.GetBudgetDictionaryByCategoryAndMonth(null, null, false, 0);

            // Act
            Assert.True(testView.calledGetDisplayType);

        }

        [Fact]
        public void Test_UpdateDisplayCallsGetDisplayType()
        {
            // Arrange
            TestView testView = new TestView();
            Presenter presenter = new Presenter(testView, testView);

            // Manually create a database so the Presenter can actually retrieve something.
            string folder = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\"));
            String goodDB = $"{folder}\\{TEST_DB_INPUT_FILE}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            presenter.CreateBudget(messyDB, false);

            // Assert
            presenter.UpdateDisplay(null, null, false, 0, false, false);

            // Act
            Assert.True(testView.calledGetDisplayType);
        }

        [Fact]
        public void Test_EditExpenseSuccess()
        {
            // Arrange
            TestView testView = new TestView();
            Presenter presenter = new Presenter(testView, testView);
            string budgetName = "TestDB";
            DateTime date = DateTime.Now;
            int category = 1;
            string amount = "50";
            string description = "test item";

            // Act
            presenter.CreateBudget(budgetName, false);
            presenter.CreateNewExpense(date, category, amount, description);
            presenter.EditExpense(1, date, category, amount, description);

            // Assert
            Assert.True(testView.calledShowSuccess);
            Assert.False(testView.calledShowError);
        }

        [Fact]
        public void Test_EditExpenseFailure()
        {
            // Arrange
            TestView testView = new TestView();
            Presenter presenter = new Presenter(testView, testView);
            string budgetName = "TestDB";
            DateTime date = DateTime.Now;
            int category = 1;
            string amount = "50";
            string description = "test item";

            // Act
            presenter.CreateBudget(budgetName, false);
            presenter.CreateNewExpense(date, category, amount, description);
            presenter.EditExpense(1, date, category, "notanumber", description); // Amount must be a number

            // Assert
            Assert.True(testView.calledShowError);
        }

        [Fact]
        public void Test_DeleteExpenseSuccess()
        {
            // Arrange
            TestView testView = new TestView();
            Presenter presenter = new Presenter(testView, testView);
            string budgetName = "TestDB2";
            DateTime date = DateTime.Now;
            int category = 1;
            string amount = "50";
            string description = "test item";

            // Act
            presenter.CreateBudget(budgetName, false);
            presenter.CreateNewExpense(date, category, amount, description);
            presenter.DeleteExpense(1);

            // Assert
            Assert.True(testView.calledShowSuccess);
            Assert.False(testView.calledShowError);
        }

        [Fact]
        public void Test_DeleteExpenseFailure()
        {
            // Arrange
            TestView testView = new TestView();
            Presenter presenter = new Presenter(testView, testView);

            // Act
            presenter.DeleteExpense(1); // Deleting when no budget was specified

            // Assert
            Assert.True(testView.calledShowError);
        }

        [Fact]
        public void Test_SelectItemSuccess()
        {
            // Arrange
            TestView testView = new TestView();
            Presenter presenter = new Presenter(testView, testView);
            string budgetName = "TestDB3";
            DateTime date = DateTime.Now;
            int category = 1;
            string amount = "10";
            string description = "McDonald's";
            List<int> indexes = new List<int>();
            List<string> items = new List<string>();
            List<string> amounts = new List<string>();

            // Act
            presenter.CreateBudget(budgetName, false);
            presenter.CreateNewExpense(date, category, amount, description);
            items.Add("McDonald's");
            amounts.Add("10");
            presenter.Search("McDonald's", indexes, items, amounts);

            // Assert
            Assert.True(testView.calledSelect);
        }

        [Fact]
        public void Test_SelectItemFailure()
        {
            // Arrange
            TestView testView = new TestView();
            Presenter presenter = new Presenter(testView, testView);
            string budgetName = "TestDB4";
            DateTime date = DateTime.Now;
            int category = 1;
            string amount = "10";
            string description = "McDonald's";
            List<int> indexes = new List<int>();
            List<string> items = new List<string>();
            List<string> amounts = new List<string>();

            // Act
            presenter.CreateBudget(budgetName, false);
            presenter.CreateNewExpense(date, category, amount, description);
            items.Add("Wendy's"); // has not been added to the budget
            amounts.Add("10");
            presenter.Search("McDonald's", indexes, items, amounts);

            // Assert
            Assert.False(testView.calledSelect);
        }
    }
}