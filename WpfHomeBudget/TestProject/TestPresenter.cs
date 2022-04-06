using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfHomeBudget;
using Xunit;

namespace TestProject
{
    public class TestView : IViewable
    {
        public bool calledClearError;
        public bool calledClearForm;
        public bool calledClearSelection;
        public bool calledRefresh;
        public bool calledShowBudgetItems;
        public bool calledShowError;
        public bool calledShowSuccess;
        public void ClearError()
        {
            throw new NotImplementedException();
        }

        public void ClearForm()
        {
            throw new NotImplementedException();
        }

        public void ClearSelection()
        {
            throw new NotImplementedException();
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public void ShowBudgetItems()
        {
            throw new NotImplementedException();
        }

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
    }
    public class TestPresenter
    {
        private const string TEST_DB_INPUT_FILE = "testDBInput.db";



        [Fact]
        public void TestConstructor()
        {
            TestView testView = new TestView();
            Presenter presenter = new Presenter(testView);
            Assert.IsType<Presenter>(presenter);
        }

        [Fact]
        public void Test_CreateNewExpenseCallsShowErrorForBadInput()
        {
            // Arrange
            TestView testView = new TestView();
            Presenter presenter = new Presenter(testView);
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
            Presenter presenter = new Presenter(testView);
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
            Presenter presenter = new Presenter(testView);

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
            Presenter presenter = new Presenter(testView);

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
    }
}
