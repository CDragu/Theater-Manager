using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheatreManagerApplication.Classes
{
    public enum SeatType
    {
        Upper,
        Dress,
        Stall,
        Balcony
    }

    public class Seat
    {
        private int seatID;
        private char seatRow;
        private int seatNumber;
        private bool isUsable;

        private SeatType seatType;
        private float seatPrice = 0;

        public Seat(int inSeatID, char inSeatRow, int inSeatNumber,
            SeatType inSeatType, bool inIsUsable)
        {
            seatID = inSeatID;
            seatRow = inSeatRow;
            seatNumber = inSeatNumber;
            seatType = inSeatType;
            isUsable = inIsUsable;
        }

        public int GetSeatID()
        {
            return seatID;
        }

        public int GetSeatNumber()
        {
            return seatNumber;
        }

        public SeatType GetSeatType()
        {
            return seatType;
        }

        public char GetSeatRow()
        {
            return seatRow;
        }

        /// <summary>
        /// Sets seat price to the value specified.
        /// </summary>
        /// <param name="inSeatPrice">New seat price.</param>
        /// <returns>Returns bool indicator of success or failure</returns>
        public bool SetSeatPrice(float inSeatPrice)
        {
            if (inSeatPrice != 0)
            {
                seatPrice = inSeatPrice;
                return true;
            }
            return false;
        }

        public float GetSeatPrice()
        {
            return seatPrice;
        }

        public bool IsUsable()
        {
            return isUsable;
        }

        /// <summary>
        /// This is a variation of the CreateSeatRowColStringFromThisSeat method which is static and can therefore be used without an instance of Seat,
        /// simply by providing SeatType and Row parameters.
        /// </summary>
        /// <returns>String carrying information about seat type, seat row and seat number.</returns>
        public static string CreateSeatRowColString(SeatType inSeatType, char inSeatRow, int inSeatNumber)
        {
            string seatRowColString = "";

            //this sets the first char representing seat type
            switch (inSeatType)
            {
                case SeatType.Upper:
                    seatRowColString = "U";
                    break;
                case SeatType.Dress:
                    seatRowColString = "D";
                    break;
                case SeatType.Stall:
                    seatRowColString = "S";
                    break;
                default:
                    throw new Exception("Problem in static Seat.CreateSeatRowColString method!");
            }

            //this sets the second char representing row letter
            seatRowColString += inSeatRow.ToString();
            
            //this sets the third and fourth chars which together represent seat number
            if (inSeatNumber < 10)
            {
                seatRowColString += "0" + inSeatNumber.ToString();
            }
            else
            {
                seatRowColString += inSeatNumber.ToString();
            }

            return seatRowColString;
        }

        /// <summary>
        /// This is a variation of the CreateSeatRowColString method which makes sure that a 
        /// string representing seatRowCol of the calling Seat instance will be returned.
        /// </summary>
        /// <returns>String carrying information about seat type, seat row and seat number.</returns>
        public string CreateSeatRowColStringFromThisSeat()
        {
            string seatRowColString = "";

            //this sets the first char representing seat type
            switch (this.seatType)
            {
                case SeatType.Upper:
                    seatRowColString = "U";
                    break;
                case SeatType.Dress:
                    seatRowColString = "D";
                    break;
                case SeatType.Stall:
                    seatRowColString = "S";
                    break;
                default:
                    throw new Exception("Problem in static Seat.CreateSeatRowColString method!");
            }

            //this sets the second char representing row letter
            seatRowColString += this.seatRow.ToString();

            //this sets the third and fourth chars which together represent seat number
            if (this.seatNumber < 10)
            {
                seatRowColString += "0" + this.seatNumber.ToString();
            }
            else
            {
                seatRowColString += this.seatNumber.ToString();
            }

            return seatRowColString;
        }

        public static SeatType GetSeatTypeFromString (string inSeatRowCol)
        {
            if (inSeatRowCol.Length == 4)
            {
                char[] seatRowColCharArray = new char[inSeatRowCol.Length];
                inSeatRowCol = inSeatRowCol.ToUpper();
                seatRowColCharArray = inSeatRowCol.ToCharArray();

                switch (seatRowColCharArray[0])
                {
                    case 'U':
                        return SeatType.Upper;
                    case 'D':
                        return SeatType.Dress;
                    case 'S':
                        return SeatType.Stall;
                    default:
                        return SeatType.Balcony;

                        //throw new Exception("Problem in static Seat.GetSeatTypeFromString method!");
                }
            }
            else
            {
                throw new Exception("Length of the string is not correct.");
            }
        }

        public static char GetSeatRowFromString(string inSeatRowCol)
        {
            try
            {
                char[] seatRowColCharArray;
                if (inSeatRowCol.Length == 4)
                {
                    seatRowColCharArray = new char[inSeatRowCol.Length];
                    inSeatRowCol = inSeatRowCol.ToUpper();
                    seatRowColCharArray = inSeatRowCol.ToCharArray();
                }
                else
                {
                    throw new Exception("Length of the string is not correct.");
                }
                return seatRowColCharArray[1];
            }
            catch
            {
                throw new Exception("Problem in Seat.SetSeatType method!");
            }
        }

        public static int GetSeatNumberFromString(string inSeatRowCol)
        {
            try
            {
                if(inSeatRowCol.Length != 4)
                {
                    throw new Exception(@"The string passed in to static Seat.GetSeatNumberFromString as a seatRowCol is not of correct length. 
                        The correct length is 4 and of form ABNN where A is the char defining SeatType, 
                        B char defining Row letter and NN the number of the seat with leading zero if lesser than 10.");
                }
                string numberSubstring = inSeatRowCol.Substring(2);
                int seatNumber = int.Parse(numberSubstring);

                return seatNumber;
            }
            catch
            {
                throw new Exception("Problem in static Seat.GetSeatNumberFromString method!");
            }
        }

        /// <summary>
        /// Calculating seat price of the object calling the method.
        /// </summary>
        /// <param name="baseSeatPrice">Base seat price - corresponds to SeatType.Upper</param>
        /// <returns>Seat price</returns>
        public float CalculateThisSeatPrice(float baseSeatPrice)
        {
            float seatPrice;
            try
            {
                //Convert.ToDouble(baseSeatPrice); IF DOESNT WORK, TRY THIS
                switch (this.seatType)
                {
                    case SeatType.Dress:
                        seatPrice = (float)Math.Round((baseSeatPrice * 1.2f), 2);
                        return seatPrice;
                    case SeatType.Stall:
                        seatPrice = (float)Math.Round((baseSeatPrice * 0.8f), 2);
                        return seatPrice;
                    case SeatType.Upper:
                        seatPrice = (float)baseSeatPrice;
                        return seatPrice;
                    default:
                        throw new Exception("seatType was not set when used in CalculateSeatPrice method in Seat class.");
                }
            }
            catch
            {
                throw new Exception("Could not CalculateSeatPrice in Seat class. Probably float-related error.");
            }
        }

        /// <summary>
        /// Static seat price calculator.
        /// </summary>
        /// <param name="baseSeatPrice">Base seat price - corresponds to SeatType.Upper</param>
        /// <param name="inSeatType">Type of the seat.</param>
        /// <returns>Seat price</returns>
        public static float CalculateSeatPrice(float baseSeatPrice, SeatType inSeatType)
        {
            float seatPrice;
            try
            {
                //Convert.ToDouble(baseSeatPrice); IF DOESNT WORK, TRY THIS
                switch (inSeatType)
                {
                    case SeatType.Dress:
                        seatPrice = (float)Math.Round((baseSeatPrice * 1.2f), 2);
                        return seatPrice;
                    case SeatType.Stall:
                        seatPrice = (float)Math.Round((baseSeatPrice * 0.8f), 2);
                        return seatPrice;
                    case SeatType.Upper:
                        seatPrice = (float)baseSeatPrice;
                        return seatPrice;
                    default:
                        throw new Exception("seatType was not set when used in static CalculateSeatPrice method in Seat class.");
                }
            }
            catch
            {
                throw new Exception("Could not CalculateSeatPrice in Seat class. Probably float-related error.");
            }
        }

        public override string ToString()
        {
            return "Seat type: " + this.seatType + " | Seat row: " + this.seatRow 
                + " | Seat number: " + this.seatNumber + " | Seat ID: " + this.seatID 
                + " | Seat price : " + this. seatPrice + "£";
        }
    }
}
