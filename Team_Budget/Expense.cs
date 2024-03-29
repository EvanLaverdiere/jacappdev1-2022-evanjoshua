﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{
    // ====================================================================
    // CLASS: Expense
    //        - An individual expens for budget program
    // ====================================================================
    /// <summary>
    /// Class representing a single expense within a budget. Expenses can be grouped within the <see cref="Expenses"/> class.
    /// </summary>
    /// <seealso cref="Expenses"/>
    /// 
    public class Expense
    {
        private int _id;
        private DateTime _date;
        private Double _amount;
        private String _description;
        private int _category;

        // ====================================================================
        // Properties
        // ====================================================================
        /// <summary>
        /// Gets the ID number of the Expense.
        /// </summary>
        public int Id { get { return _id; } }
        /// <summary>
        /// Gets the date on which the Expense was incurred.
        /// </summary>
        public DateTime Date { get { return _date; } }
        /// <summary>
        /// Gets or sets the amount of money spent on the Expense.
        /// </summary>
        public Double Amount { get { return _amount; } }
        /// <summary>
        /// Gets or sets the description of the Expense.
        /// </summary>
        public String Description { get { return _description; } }
        /// <summary>
        /// Gets or sets the category of the Expense.
        /// </summary>
        public int Category { get { return _category; } }

        // ====================================================================
        // Constructor
        //    NB: there is no verification the expense category exists in the
        //        categories object
        // ====================================================================
        /// <summary>
        /// Creates an Expense object using passed values.
        /// </summary>
        /// <param name="id">The ID of the expense.</param>
        /// <param name="date">The date on which the expense occurred.</param>
        /// <param name="category">A number representing category of the expense.</param>
        /// <param name="amount">The cost or monetary amount of the expense.</param>
        /// <param name="description">A brief description of the expense.</param>
        /// <example>
        /// In this example, an expense representing a one-hour consultation with an electrician is created.
        /// <code>
        /// <![CDATA[
        /// int id = 1;
        /// DateTime time = new DateTime(2021, 10, 31); // initializes a DateTime object with a date of October 31st, 2021.
        /// int category = 1; // In a default list of categories, this value corresponds to the Utilities category.
        /// Double amount = 100; // The cost of this hour-long consultation was $100.
        /// String description = "Electrician Consultation"; 
        /// 
        /// Expense consultation = new Expense(id, time, category, amount, description);
        /// ]]>
        /// </code>
        /// </example>
        public Expense(DateTime date, int category, Double amount, String description)
        {
            this._date = date;
            this._category = category;
            this._amount = amount;
            this._description = description;
        }

        public Expense(int id, DateTime date, int category, Double amount, String description)
        {
            this._id = id;
            this._date = date;
            this._category = category;
            this._amount = amount;
            this._description = description;
        }

        // ====================================================================
        // Copy constructor - does a deep copy
        // ====================================================================
        /// <summary>
        /// Creates a new Expense object by copying an existing one. The duplicated Expense can then be manipulated without affecting the original object.
        /// </summary>
        /// <param name="obj">An Expense object to be copied.</param>
        /// <example>
        /// In this example, an Expense object representing a consultation with an electrician is created. It is then cloned into a new Expense using this constructor.
        /// <code>
        /// <![CDATA[
        /// int id = 1;
        /// DateTime time = new DateTime(2021, 10, 31); // initializes a DateTime object with a date of October 31st, 2021.
        /// int category = 1; // In a default list of categories, this value corresponds to the Utilities category.
        /// Double amount = 100; // The cost of this hour-long consultation was $100.
        /// String description = "Electrician Consultation"; 
        /// 
        /// Expense consultation = new Expense(id, time, category, amount, description);
        /// Expense copiedConsultation = new Expense(consultation); 
        /// ]]>
        /// </code>
        /// </example>
        public Expense(Expense obj)
        {
            this._id = obj.Id;
            this._date = obj.Date;
            this._category = obj.Category;
            this._amount = obj.Amount;
            this._description = obj.Description;
        }
    }
}
