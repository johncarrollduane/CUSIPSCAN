using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUSIPSCAN
{
    static class GlobalVar
    {
        /// <summary>
        /// Property holds the processing scenario (Advent, APL, or Exit)
        /// </summary>
        public static string Scenario { get; set; }

        /// <summary>
        /// Method converts a string value to a double.  If the method fails to
        /// resolve to a number, return a zero.
        /// </summary>
        /// <param name="strVal"></param>
        /// <returns>a value of type double</returns>
        public static double ConvertToDouble(string strVal)
        {
            double tmpDouble;
            if (double.TryParse(strVal, out tmpDouble))
            {
                return tmpDouble;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Access routine for global processing date.
        /// </summary>
        public static string conDate { get; set; }

        /// <summary>
        /// Access routine for global processing time.
        /// </summary>
        public static string conTime { get; set; }


        /// <summary>
        /// Global string variable holding program Title.
        /// </summary>
        public const string conTitle = "CUSIPSCAN";


        /// <summary>
        /// Boolean indicator that session error exists.
        /// </summary>
        public static bool SessionErrorExists { get; set; }


        /// <summary>
        /// Session error description
        /// </summary>
        public static string SessionErrorDescription { get; set; }

        /// <summary>
        /// Access routine for error checking boolean variable
        /// </summary>
        public static string UserName { get; set; }

        /// <summary>
        /// Access routine for the user settings file name.
        /// </summary>
        public static string UserSettingsFile { get; set; }

        /// <summary>
        /// Access routine for the holiday file name.
        /// </summary>
        public static string HolidayFile { get; set; }

        /// <summary>
        /// Access routine for the SPOT CUSIP file name.
        /// </summary>
        public static string InputFileCUSIPS { get; set; }

        /// <summary>
        /// Access routine for APL holdings file.
        /// </summary>
        public static string InputFileAPLHoldings { get; set; }


        /// <summary>
        /// Access routine for Advent holdings file.
        /// </summary>
        public static string InputFileAdventHoldings { get; set; }


        /// <summary>
        /// Access routine for prohibited issuers list file.
        /// </summary>
        public static string InputFileProhibited { get; set; }

        /// <summary>
        /// Holds file name for previous day's prohibited holdings
        /// </summary>
        public static string InputFilePrevProhibited { get; set; }

        /// <summary>
        /// Holds file name of APL header file from previous night.
        /// </summary>
        public static string InputFileAPLHDR { get; set; }

        /// <summary>
        /// Holds file name of Advent header file from previous night.
        /// </summary>
        public static string InputFileAdventHDR { get; set; }

        /// <summary>
        /// Holds Status code change file name..
        /// </summary>
        public static string FinalOutputReport { get; set; }

        /// <summary>
        /// Holds holdings that are on phohibited issuers list
        /// </summary>
        public static string ProhibitedHoldings { get; set; }

        /// <summary>
        /// Holds Process Log file name.
        /// </summary>
        public static string OutputProcessLog { get; set; }


        /// <summary>
        /// Method that writes to the log file.  Each entry to the log will contain the date and time.
        /// </summary>
        /// <param name="entryText">Entry text that will be written to the log.</param>
        public static void LogEntry(string entryText)
        {
            conDate = DateTime.Today.ToShortDateString();
            conTime = DateTime.Now.ToShortTimeString();

            //Open the file and write the log entry..
            using (StreamWriter sw = new StreamWriter(OutputProcessLog, true))
            {
                sw.WriteLine("{0} {1} {2} {3}", conDate, conTime, UserName, entryText);
            }
        }



        /// <summary>
        /// This method builds the user identity.
        /// </summary>
        public static void BuildUser()
        {
            //Determine the user name of the person running the application.

            //First load into memory from the UserSettings file a list of all
            //the allowable users.
            Users eodUsers = new Users(UserSettingsFile);

            //If the user settings file cannot be found, exit the procedure.
            if (eodUsers.ErrorMessage != null)
            {
                LogEntry(SessionErrorDescription);
                LogEntry("Program terminated");
                return;
            }

            //Lookup the network ID of the user from the operating system.
            //Use the network ID to get the user's full name from UserSettings.txt.
            //Assign the user name to a global variable for use later.
            UserName = eodUsers.GetFullName(Environment.UserName);
        }

        /// <summary>
        /// This method builds all the input and output file names using settings data 
        /// as well as the current system data and time.
        /// </summary>
        public static void BuildFileNames(string system)
        {
            //First load the settings data from the settings text file into AppSettings object.
            Settings AppSettings = new Settings(Properties.Settings.Default.FileSettings);
            //In case the settings file is corrupted, return to the caller and terminate.
            if (SessionErrorExists) { return; }

            //Create an instance of the DateTool object.  It loads the holidays file
            //in order to exclude them from business days.
            HolidayFile = AppSettings.GetSetting("DIR", "SYSTEMS", "GENERAL") +
                AppSettings.GetSetting("FIL", "SYSTEMS", "HOLIDAYS");
            if (!File.Exists(HolidayFile))
            {
                GlobalVar.SessionErrorExists = true;
                GlobalVar.SessionErrorDescription = "File not found: " + HolidayFile;
                return;
            }

            //Load the current system date and time into the DateTool object and 
            //into global variables.
            DateTool dt = new DateTool(HolidayFile);
            if (SessionErrorExists) { return; }

            dt.TheDate = DateTime.Today;
            conDate = dt.TheDate.ToShortDateString();
            conTime = DateTime.Now.ToShortTimeString();

            //Create file names for output files from settings.
            FinalOutputReport = String.Format("{0}{1}_{2}.txt", 
                AppSettings.GetSetting("DIR", "CSAPP", "OUTPUT"), system, dt.DateYYYYMMDD);
            ProhibitedHoldings = String.Format("{0}{1}_{2}.txt",
                AppSettings.GetSetting("DIR", "CSAPP", "OUTPUT"), "PRO",dt.DateYYYYMMDD);
            OutputProcessLog = String.Format("{0}{1}", 
                AppSettings.GetSetting("DIR", "CSAPP", "OUTPUT"),
                AppSettings.GetSetting("FIL", "CSAPP", "LOG"));

            //Create names for the input files from settings.
            UserSettingsFile = AppSettings.GetSetting("DIR", "SYSTEMS", "GENERAL") +
                AppSettings.GetSetting("FIL", "SYSTEMS", "USERS");
            if (!File.Exists(UserSettingsFile))
            {
                GlobalVar.SessionErrorExists = true;
                GlobalVar.SessionErrorDescription = "File not found: " + UserSettingsFile;
                return;
            }

            //CUSIP file
            InputFileCUSIPS = AppSettings.GetSetting("DIR", "SPOT", "TRADING") + 
                AppSettings.GetSetting("FIL", "SPOT", "CUSIP");
            if (!File.Exists(InputFileCUSIPS))
            {
                GlobalVar.SessionErrorExists = true;
                GlobalVar.SessionErrorDescription = "File not found: " + InputFileCUSIPS;
                return;
            }

            //Prohibited issuer list file.
            InputFileProhibited = AppSettings.GetSetting("DIR", "SPOT", "TRADING") +
                AppSettings.GetSetting("FIL", "SPOT", "PROHIB");
            if (!File.Exists(InputFileProhibited))
            {
                GlobalVar.SessionErrorExists = true;
                GlobalVar.SessionErrorDescription = "File not found: " + InputFileProhibited;
                return;
            }

            //Previous day's prohibited holdings
            InputFilePrevProhibited = String.Format("{0}{1}_{2}.txt",
                AppSettings.GetSetting("DIR", "CSAPP", "OUTPUT"), "PRO", dt.PrevDateYYYYMMDD);
            if (!File.Exists(InputFilePrevProhibited))
            {
                GlobalVar.SessionErrorExists = true;
                GlobalVar.SessionErrorDescription = "File not found: " + InputFilePrevProhibited;
                return;
            }

            //APL scenario only
            if (system == "APL")
            {
                InputFileAPLHoldings = String.Format("{0}{1}.cus",
                    AppSettings.GetSetting("DIR", "APL", "HOLDINGS"), dt.DateYYYYMMDD);
                if (!File.Exists(InputFileAPLHoldings))
                {
                    GlobalVar.SessionErrorExists = true;
                    GlobalVar.SessionErrorDescription = "File not found: " + InputFileAPLHoldings;
                    return;
                }

                //Previous APL header file
                InputFileAPLHDR = String.Format("{0}{1}.hdr", AppSettings.GetSetting("DIR", "APL", "DISK"),
                    dt.PrevDateYYYYMMDD);
                if (!File.Exists(InputFileAPLHDR))
                {
                    GlobalVar.SessionErrorExists = true;
                    GlobalVar.SessionErrorDescription = "File not found: " + InputFileAPLHDR;
                    return;
                }
            }

            //Advent scenario only
            if (system == "ADVENT")
            {
                InputFileAdventHoldings = String.Format("{0}{1}.hp", AppSettings.GetSetting("DIR", "ADVENT", "DISK"),
                    dt.PrevDateYYYYMMDD);
                if (!File.Exists(InputFileAdventHoldings))
                {
                    GlobalVar.SessionErrorExists = true;
                    GlobalVar.SessionErrorDescription = "File not found: " + InputFileAdventHoldings;
                    return;
                }

                //Previous Advent header file
                InputFileAdventHDR = String.Format("{0}{1}.hdr", AppSettings.GetSetting("DIR", "ADVENT", "DISK"),
                    dt.PrevDateYYYYMMDD);
                if (!File.Exists(InputFileAdventHDR))
                {
                    GlobalVar.SessionErrorExists = true;
                    GlobalVar.SessionErrorDescription = "File not found: " + InputFileAdventHDR;
                    return;
                }
            }

        }
    }
}
