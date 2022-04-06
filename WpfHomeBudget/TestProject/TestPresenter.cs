using System;
using System.Collections.Generic;
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
    }
    public class TestPresenter
    {
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
    }
}
