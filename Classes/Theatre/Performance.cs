using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheatreManagerApplication.Classes
{
    public class Performance
    {
        private int performanceID;
        private DateTime performanceDate;
        private int playID;
        public Performance(int inPerformanceID, DateTime inPerformanceDate, int inPlayid)
        {
            performanceID = inPerformanceID;
            performanceDate = inPerformanceDate;
            playID = inPlayid;
        }

        public int GetPerformanceID()
        {
            return performanceID;
        }

        public void SetNewPerformanceDate(DateTime newPerformanceDate)
        {
            performanceDate = newPerformanceDate;
        }

        public DateTime GetPerformanceDate()
        {
            return performanceDate;
        }

        public int GetPlayIDFromPerformance()
        {
            return playID;
        }

        public void SetPlayIdInPerformance(int playIDs)
        {
             playID = playIDs;
        }
    }
}
