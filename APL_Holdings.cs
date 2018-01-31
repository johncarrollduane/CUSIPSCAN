using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUSIPSCAN
{
    /// <summary>
    /// Class that defines an object that will load data from the APL
    /// holdings file into a dictionary collection.
    /// </summary>
    class APL_Holdings : Securities
    {
        /// <summary>
        /// Constructor for this object.  It calls the method that loads the 
        /// dictionary collection.
        /// </summary>
        /// <param name="fi">APL holdings file</param>
        public APL_Holdings(FileInfo fi)
        {
            LoadDict(fi);
        }

        /// <summary>
        /// Method that loads the APL holdings file data into a dictionary
        /// collection.
        /// </summary>
        /// <param name="fi">APL holdings file</param>
        public override void LoadDict(FileInfo fi)
        {
            Console.WriteLine("Loading apl holdings file");

            using (StreamReader sr = new StreamReader(fi.FullName))
            {
                string row;
                string[] fields;
                while ((row = sr.ReadLine()) != null)
                {
                    Security mySecurity = new Security();
                    row = row.Replace("\"", "");
                    fields = row.Split(',');
                    mySecurity.System = "APL";
                    mySecurity.SNAM = fields[0];
                    mySecurity.SecurityType = fields[1].Trim();
                    mySecurity.Ticker = fields[2];
                    mySecurity.Qty = GlobalVar.ConvertToDouble(fields[3]);
                    mySecurity.CUSIP = fields[4].Substring(0,8);
                    DictSec.Add(mySecurity.SNAM + mySecurity.CUSIP, mySecurity);
                    mySecurity = null;
                }
            }
            Console.WriteLine("Positions in apl holdings file: " + DictSec.Count);
        }
    }
}
