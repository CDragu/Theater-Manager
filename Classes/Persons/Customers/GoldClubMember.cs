using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheatreManagerApplication.Classes
{
    public class GoldClubMember : Customer
    {
        private List<Play> listOfPlaysSeen;
        private DateTime membershipStartDate;
        private DateTime membershipLastRenewed;

        public GoldClubMember(string inName, string inContactEmail,
            int inCustomerID, List<Play> inListOfPlaysSeen, DateTime inMembershipStartDate, DateTime inMembershipLastRenewed)
            : base(inName, inContactEmail, inCustomerID, true)
        //true sets the isGoldClubMember in parent to true by calling GoldClubMember constructor
        {
            listOfPlaysSeen = inListOfPlaysSeen;
            membershipStartDate = inMembershipStartDate;
            membershipLastRenewed = inMembershipLastRenewed;
        }

        //Get and Set methods for GoldClubMember class values.
        public void SetListOfPlaysSeen(List<Play> inListOfPlaysSeen)
        {
            listOfPlaysSeen = inListOfPlaysSeen;
        }

        public List<Play> GetListOfPlaysSeen()
        {
            return listOfPlaysSeen;
        }

        public void AddPlayToListOfPlaysSeen(Play play)
        {
            listOfPlaysSeen.Add(play);
        }

        public DateTime GetMembershipStartDate()
        {
            return membershipStartDate;
        }

        public void SetMembershipLastRenewed(DateTime inMembershipLastRenewed)
        {
            membershipLastRenewed = inMembershipLastRenewed;
        }

        public DateTime GetMembershipLastRenewed()
        {
            return membershipLastRenewed;
        }

        public override string ToString()
        {
            string goldClubMemberString;
            goldClubMemberString = base.ToString();
            goldClubMemberString += "\nMembership start: " + this.GetMembershipStartDate().ToShortDateString();
            goldClubMemberString += "\nLast renewed: " + this.GetMembershipLastRenewed().ToShortDateString();
            return goldClubMemberString;
        }
    }
}
