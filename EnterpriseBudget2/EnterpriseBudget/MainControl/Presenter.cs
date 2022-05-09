
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnterpriseBudget.MainControl
{
    /// <summary>
    /// Presenter for the Main Control (Entry point of the App)
    /// </summary>
    class Presenter
    {
        private InterfaceView mainControl;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mc">Main Control view</param>
        public Presenter(InterfaceView mc)
        {
            mainControl = mc;
        }

        /// <summary>
        /// Given username and password, get the employee object
        /// </summary>
        /// <param name="user">username of employee</param>
        /// <param name="pass">password of employee</param>
        /// <param name="view">what type of view are they interested in?</param>
        /// <returns>an Model.Employee object if username and password are correct AND if user has the rights to see the view,
        ///          otherwise returns null </returns>        
        public Model.Employee GetEmployeeForSpecifiedView(string user, string pass, ViewType view)
        {
            // Get the employee object from the model
            Model.Employee employee = Model.Employee.validateUser(user, pass);

            // if there is such an employee with username/password, check their job type to
            // see if they have the privilege to see the view
            if (employee != null)
            {
                switch (view) {
                    case ViewType.ReadOnly:
                        if (employee.jobType == Model.JobTypes.Admin || employee.jobType == Model.JobTypes.Chair
                            || employee.jobType == Model.JobTypes.Faculty)
                        {
                            return employee;
                        }
                        break;
                    case ViewType.ReadWrite:
                        if (employee.jobType == Model.JobTypes.Admin || employee.jobType == Model.JobTypes.Chair)
                        {
                            return employee;
                        }
                        break;
                    case ViewType.Manage:
                        if (employee.jobType == Model.JobTypes.Admin)
                        {
                            return employee;
                        }
                        break;
                }
            }

            // employee cannot see this view (either bad username/password or privilege is not sufficient)
            return null;
        }
    }
}
