using System;
using System.Collections.Generic;
using System.Text;

namespace WpfHomeBudget.Interfaces
{
    public interface IDisplayable
    {
        string GetDisplayType();
        bool isOrderedByMonthAndCategory();
        void DisplayToGrid<T>(List<T> budgetItems);
        void DisplayToChart(List<object> budgetItems);
        void InitializeByCategoryAndMonthDisplay(List<string> categoryNames);
    }
}
