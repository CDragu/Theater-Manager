using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheatreManagerApplication.Classes
{
    public class Customer : Person
    {
        private int customerID;
        private bool isGoldClubMember;


        public Customer(string inName, string inContactEmail, int inCustomerID, bool inIsGoldClubMember)
            : base(inName, inContactEmail)
        {
            customerID = inCustomerID;
            isGoldClubMember = inIsGoldClubMember;
        }

        public int GetCustomerID()
        {
            return customerID;
        }
        public bool IsGoldClubMember()
        {
            return isGoldClubMember;
        }

        public void MakeGoldMemeber()
        {
            isGoldClubMember = true;
        }

        public override string ToString()
        {
            string customerString;
            customerString = "Name " + this.GetName();
            customerString += "\nEmail: " + this.GetEmail();
            return customerString;
        }
    }
}
