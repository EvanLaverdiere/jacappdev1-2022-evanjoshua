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
        private IViewable viewable;
        HomeBudget budget;

        // constructor
        public Presenter(IViewable view)
        {
            viewable = view;
            //budget = new HomeBudget();
        }
        // methods
        public void CreateBudget(string dbName, bool newDatabase)
        {
            budget = new HomeBudget(dbName, newDatabase);
        }
    }
}
