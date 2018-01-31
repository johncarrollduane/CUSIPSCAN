using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUSIPSCAN
{
    /// <summary>
    /// Class that defines an object that will load data from the
    /// CUSIP file into a dictionary collection.
    /// </summary>
    class CUSIPs : Securities
    {
        /// <summary>
        /// Constructor for this object.  It calls the method that loads the 
        /// dictionary collection.
        /// </summary>
        /// <param name="fi">CUSIP file</param>
        public CUSIPs(FileInfo fi)
        {
            LoadDict(fi);
        }

        /// <summary>
        /// Method that loads the CUSIP file data into a dictionary
        /// collection.
        /// </summary>
        /// <param name="fi">CUSIP file</param>
        public override void LoadDict(FileInfo fi)
        {
            Console.WriteLine("Loading cusip file");
            
            using (StreamReader sr = new StreamReader(fi.FullName))
            {
                string row;
                while ((row = sr.ReadLine()) != null)
                {
                    if (row.Substring(71, 3).Trim() == "yes")
                    {
                        Security mySecurity = new Security();
                        mySecurity.System = "SPOT";
                        mySecurity.CUSIP = row.Substring(0, 8);
                        mySecurity.Ticker = row.Substring(10, 5).Trim();
                        mySecurity.Name = row.Substring(17, 25).Trim();

                        //The last field in the row contains "yes" or "no."
                        //Yes indicates that the security is an ETF.
                        if (row.Substring(75).Trim() == "yes")
                        {
                            mySecurity.IsETF = true;
                        }
                        else { mySecurity.IsETF = false; }
                        

                        string tempPrice = row.Substring(47, 7).Trim();
                        mySecurity.Price = GlobalVar.ConvertToDouble(tempPrice);
                        DictSec.Add(mySecurity.CUSIP, mySecurity);
                        mySecurity = null;
                    }

                }
            }
            Console.WriteLine("Number of items in CUSIP list: " + DictSec.Count.ToString());
        }
    }
}
