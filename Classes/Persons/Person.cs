using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheatreManagerApplication.Classes
{
    public abstract class Person
    {
        private string name;
        private string contactEmail;

        /// <summary>
        /// Abstract Person class constructor. Called only from children classes.
        /// </summary>
        /// <param name="inName"></param>
        /// <param name="inContactEmail"></param>
        public Person(string inName, string inContactEmail)
        {
            name = inName;
            contactEmail = inContactEmail;
        }

        //Get and Set methods for Person class values.

        /// <summary>
        /// Sets name to a value specified.
        /// </summary>
        /// <param name="inName">Value to set the name to.</param>
        public void SetName(string inName)
        {
            name = inName;
        }

        /// <summary>
        /// Method to read the name variable.
        /// </summary>
        /// <returns>Returns name string value.</returns>
        public string GetName()
        {
            return name;
        }

        /// <summary>
        /// Sets email to a value specified.
        /// </summary>
        /// <param name="inContactEmail">Value to set the email to.</param>
        public void SetEmail(string inContactEmail)
        {
            contactEmail = inContactEmail;
        }

        /// <summary>
        /// Method to read the email variable.
        /// </summary>
        /// <returns>Returns email string value.</returns>
        public string GetEmail()
        {
            return contactEmail;
        }
    }
}
