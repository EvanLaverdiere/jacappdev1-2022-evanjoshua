using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfHomeBudget;

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
            throw new NotImplementedException();
        }
    }
    class TestPresenter
    {
    }
}
