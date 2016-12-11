using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheatreManagerApplication.Classes
{
    public enum PlayType
    {
        Major,
        Minor,
        OneTime
    }

    public class Play
    {
        private int playID;
        private string playName;
        private PlayType playType;
        private string author;
        private float basePriceForSeat;
        private List<Performance> listOfPerformances;

        public Play(
            int inPlayID,
            string inPlayName,
            PlayType inPlayType,
            string inAuthor,
            float inBasePriceForSeat,
            List<Performance> inListOfPerformances)
        {
            playID = inPlayID;
            playName = inPlayName;
            playType = inPlayType;
            author = inAuthor;
            basePriceForSeat = inBasePriceForSeat;
            listOfPerformances = inListOfPerformances;
        }

        public int GetPlayID()
        {
            return playID;
        }

        /// <summary>
        /// Method that changes play name if string that is not empty was passed in.
        /// </summary>
        /// <param name="newName">String containing new name value.</param>
        /// <returns>Boolean indicator. If false, name was not 
        /// changed and the parameter passed in is empty string.</returns>
        public bool ChangePlayName(string newName)
        {
            if (newName == "")
            {
                return false;
            }

            playName = newName;
            return true;
        }

        public string GetPlayName()
        {
            return playName;
        }

        public PlayType GetPlayType()
        {
            return playType;
        }

        public void SetPlayType(PlayType newPlayType)
        {
            playType = newPlayType;
        }

        /// <summary>
        /// Method that changes author if string that is not empty was passed in.
        /// </summary>
        /// <param name="newName">String containing new author value.</param>
        /// <returns>Boolean indicator. If false, name was not 
        /// changed and the parameter passed in is empty string.</returns>
        public bool ChangeAuthor(string newAuthor)
        {
            if (newAuthor == "")
            {
                return false;
            }

            author = newAuthor;
            return true;
        }

        public string GetAuthor()
        {
            return author;
        }

        /// <summary>
        /// MEthod that changes base price for seat for the given play.
        /// </summary>
        /// <param name="newBasePrice">New basePrice value for seat.</param>
        /// <returns>Returns bool indicator of the change executing successfully.</returns>
        public bool ChangeBasePriceForSeat(float newBasePrice)
        {
            try
            {
                basePriceForSeat = newBasePrice;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public float GetBasePriceForSeat()
        {
            return basePriceForSeat;
        }

        /// <summary>
        /// IF for any reason new performance could not be added, bool indicator is returned.
        /// </summary>
        /// <param name="performanceToAdd">Performance to add to the list.</param>
        /// <returns>Boolean indicator. False mean that Performance could not be added.</returns>
        public bool AddPerformanceToTheListOfPerformances(Performance performanceToAdd)
        {
            try
            {
                listOfPerformances.Add(performanceToAdd);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Method that removes the specified Performance from the booking. 
        /// </summary>
        /// <param name="performanceToRemoveID">Integer parameter specifying which Performance to remove.</param>
        /// <returns>Returns boolean indicator of Performance being removed or not. 
        /// Could return false if eg. wrong parameter was passed in.</returns>
        public bool RemovePerformanceFromTheListOfPerformances(int performanceToRemoveID)
        {
            Performance performanceToRemove = null;
            bool hasRemovedPerformance = false;

            foreach (Performance performance in listOfPerformances)
            {
                if (performance.GetPerformanceID() == performanceToRemoveID)
                {
                    performanceToRemove = performance;
                }
            }

            if (performanceToRemove != null)
            {
                listOfPerformances.Remove(performanceToRemove);
                hasRemovedPerformance = true;
            }

            return hasRemovedPerformance;
        }


        public List<Performance> GetListOfPerformancesOfThisPlay()
        {
            return listOfPerformances;
        }

        public static PlayType PlayTypeIdToPlayTypeConverter(int playTypeID)
        {
            switch (playTypeID)
            {
                case 1:
                    return PlayType.Major;
                case 2:
                    return PlayType.Minor;
                case 3:
                    return PlayType.OneTime;
                default:
                    throw new Exception("Wrong convertion method in PlayTypeIdToPlayTypeConverter.");
            }
        }
    }
}
