using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUSIPSCAN
{
    /// <summary>
    /// Class that defines the date functionality of this tool.
    /// </summary>
    public class DateTool
    {
        /// <summary>
        /// List collection to hold the holidays.  Holds only DateTime data types.
        /// </summary>
        private List<DateTime> _holidays = new List<DateTime>();

        /// <summary>
        /// Helper member for ErrorMessage public propoerty.
        /// </summary>
        private string _errorMessage;

        /// <summary>
        /// Constructor for the DateTool class.  It loads the holidays from the
        /// holiday file into a list called _holidays.
        /// </summary>
        public DateTool(string holidayFileName)
        {

            FileInfo holidayFile = new FileInfo(holidayFileName);

            //Make sure the file exists.
            //Catch "file not found" error here before trying to open file.
            if (!holidayFile.Exists)
            {
                _errorMessage = "Holidays file not found.";
                GlobalVar.SessionErrorExists = true;
                GlobalVar.SessionErrorDescription = "File not found: " + holidayFile.FullName;
                return;
            }

            //Read fixed column width file line by line and load into the list.
            try
            {
                //Load arrayList
                using (StreamReader sr = holidayFile.OpenText())
                {
                    string s = "";
                    DateTime d;
                    while ((s = sr.ReadLine()) != null)
                    {
                        d = DateTime.Parse(s);
                        _holidays.Add(d);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }

        }

        /// <summary>
        /// Access routine for current date constant.
        /// </summary>
        public DateTime TheDate { get; set; }


        /// <summary>
        /// Read only property that returns the current date in YYYYMMDD format.
        /// </summary>
        public string DateYYYYMMDD
        {
            get
            {
                return TheDate.ToString("yyyyMMdd");
            }
        }

        /// <summary>
        /// Read only property that returns the previous date in YYYYMMDD format.
        /// </summary>
        public string PrevDateYYYYMMDD
        {
            get
            {
                return PrevDate.ToString("yyyyMMdd");
            }
        }

        /// <summary>
        /// Read only property that returns the date 2 business days ago in YYYYMMDD format.
        /// </summary>
        public string Prev2DateYYYYMMDD
        {
            get
            {
                return Prev2Date.ToString("yyyyMMdd");
            }
        }

        /// <summary>
        /// Read-only property that returns the business day that is
        /// one day before the given date.  The given date is stored
        /// in theDate.  The procedure subtracts a day from theDate
        /// and then compares it to the holiday list.  It continues
        /// to subtract a day until the result is not a Sat or Sun or
        /// a holiday.
        /// </summary>
        public DateTime PrevDate
        {
            get
            {
                //subtract one day from the given date.
                DateTime _prev = TheDate.AddDays(-1);

                //Continue subtracting one day from the date as long as the day is a Sat, Sun, or holiday.
                while (_prev.DayOfWeek == DayOfWeek.Saturday || _prev.DayOfWeek == DayOfWeek.Sunday ||
                    IsHoliday(_prev))
                {
                    _prev = _prev.AddDays(-1);
                }
                return _prev;
            }
        }

        /// <summary>
        /// Read-only property that returns the business day that is
        /// two days before the given date.  The given date is stored
        /// in theDate.  The procedure subtracts 2 days from theDate
        /// and then compares it to the holiday list.  It continues
        /// to subtract a day until the result is not a Sat or Sun or
        /// a holiday.
        /// </summary>
        public DateTime Prev2Date
        {
            get
            {
                //subtract one day from the given date.
                DateTime _prev2 = TheDate.AddDays(-2);

                //Continue subtracting one day from the date as long as the day is a Sat, Sun, or holiday.
                while (_prev2.DayOfWeek == DayOfWeek.Saturday || _prev2.DayOfWeek == DayOfWeek.Sunday ||
                    IsHoliday(_prev2))
                {
                    _prev2 = _prev2.AddDays(-1);
                }
                return _prev2;
            }
        }


        /// <summary>
        /// Checks to see if value is a holiday.
        /// </summary>
        /// <param name="dt">DateTime value that will be evaluated</param>
        /// <returns>true or false</returns>
        public bool IsHoliday(DateTime dt)
        {
            bool isHoliday = true;
            foreach (DateTime holiday in _holidays)
            {
                if (dt == holiday)
                {
                    //If there is a match, exit the loop and return True.
                    isHoliday = true;
                    return isHoliday;
                }
                else
                {
                    isHoliday = false;
                }
            }
            return isHoliday;
        }

        /// <summary>
        /// Property designed to hold error messages delivered back to the caller.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return this._errorMessage;
            }
            set
            {
                this._errorMessage = value;
                this._errorMessage += " DateTool Error";
            }
        }
    }
}
