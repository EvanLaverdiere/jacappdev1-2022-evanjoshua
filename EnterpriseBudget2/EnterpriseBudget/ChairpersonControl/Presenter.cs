using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Budget;

namespace EnterpriseBudget.ChairpersonControl
{
    internal class Presenter
    {
        private IDisplayable viewable, displayable;
        private HomeBudget budget;


        public Presenter(IDisplayable view, IDisplayable display)
        {
            viewable = view;
            displayable = display;
        }

        public void CreateBudget(string dbName, bool newDatabase)
        {
            budget = new HomeBudget(dbName, newDatabase);
        }
    }

}
