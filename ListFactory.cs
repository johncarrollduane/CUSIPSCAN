using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUSIPSCAN
{
    /// <summary>
    /// This class defines the ListFactory object.  This object serves as a process
    /// control mechanism for the program.  The process applies to the APL
    /// scenario and to the Advent scenario.
    /// Overview of process:
    /// A.  INCLUDED IN MAKELIST METHOD
    ///     1.  Create list of holdings
    ///     2.  Create list of securities in the EMS universe
    ///     3.  Create list of prohibited issuers list securities
    ///     4.  Create a list of client accounts for this scenario
    ///     5.  Compare holdings to EMS universe and create list of holdings NOT in 
    ///     the universe.
    ///     6.  Compare holdings to prohibited securities and create list of 
    ///     prohibited holdings.
    ///     7.  Compare holdings to EMS universe and create list of ETF holdings.  Then
    ///         compare that list to the list of client accounts and create a list of
    ///         accounts with an ETF and a status code that is not FROZEN.
    /// B.  INCLUDED IN MAKEREPORT METHOD
    ///     1.  Create text file report showing
    ///         a.  Holdings not in EMS universe
    ///         b.  Holdings on prohibited issuer list.
    /// </summary>
    class ListFactory
    {
        /// <summary>
        /// This property holds the processing scenario indicator.
        /// Possible values:
        /// 1.  APL
        /// 2.  Advent
        /// </summary>
        private string _scenario;

        /// <summary>
        /// This list holds CUSIPSCAN exceptions.  It serves as a mechanism to pass
        /// the exceptions from one method to another.
        /// </summary>
        private List<Security> _excepts = new List<Security>();

        /// <summary>
        /// This list holds prohibited issuer list exceptions.  It serves as a 
        /// mechanism to pass the exceptions from one method to another.
        /// </summary>
        private List<Security> _pros = new List<Security>();

        /// <summary>
        /// This list holds accounts with ETF's where the status code is not FROZEN.
        /// </summary>
        private List<Security> _etfs = new List<Security>();

        /// <summary>
        /// Constructor for the object.
        /// </summary>
        /// <param name="scenario">Processing scenario for the program</param>
        public ListFactory(string scenario)
        {
            _scenario = scenario;
        }

        /// <summary>
        /// Process control method for creation of the lists.  The process differs
        /// slightly between the APL and Advent scenarios.
        /// </summary>
        public void MakeList()
        {
            //Create FileInfo objects to represent the input files used in both scenarios.
            //Then create objects to hold that data.
            //EMS Universe

            FileInfo fi1 = new FileInfo(GlobalVar.InputFileCUSIPS);
            Securities cusips = new CUSIPs(fi1);

            //Prohibited Issuer List
            FileInfo fi4 = new FileInfo(GlobalVar.InputFileProhibited);
            Securities pros = new Prohibiteds(fi4);

            //Scenario specific processes.
            // 1.  Create FileInfo objects for input files.
            // 2.  Create objects to hold the data.
            // 3.  Create lists of differences
            switch (_scenario)
            {
                case "APL":
                    //APL holdings
                    FileInfo fi2 = new FileInfo(GlobalVar.InputFileAPLHoldings);
                    Securities aplsCurr = new APL_Holdings(fi2);

                    //APL accounts
                    FileInfo fi7 = new FileInfo(GlobalVar.InputFileAPLHDR);
                    Accounts aplAccts = new APLAccts(fi7);

                    //Holdings not in EMS universe
                    ExceptionsList apl = new APLExceptions(aplsCurr.DictSec,cusips.DictSec);
                    _excepts = apl.ExceptList;

                    //Holdings on prohibited issuer list
                    FileInfo fi5 = new FileInfo(GlobalVar.InputFilePrevProhibited);
                    ExceptionsList proAPL = new ProhibitedExceptions(aplsCurr.DictSec, pros.DictSec, fi5, _scenario);
                    _pros = proAPL.ExceptList;

                    //Accounts holding ETF's where status code is not FROZEN
                    ETFExceptions etfAPL = new ETFExceptions(aplsCurr.DictSec, cusips.DictSec, aplAccts.DictAccts,_scenario);
                    _etfs = etfAPL.ETF_Exceptions;

                    break;

                case "ADVENT":
                    //Advent holdings
                    FileInfo fi3 = new FileInfo(GlobalVar.InputFileAdventHoldings);
                    Securities advs = new Advent_Holdings(fi3);

                    //Advent accounts
                    FileInfo fi8 = new FileInfo(GlobalVar.InputFileAdventHDR);
                    Accounts adventAccts = new AdventAccts(fi8);

                    //Holdings not in EMS universe
                    ExceptionsList adv = new AdventExceptions(advs.DictSec, cusips.DictSec);
                    _excepts = adv.ExceptList;

                    //Holdings on prohibited issuer list
                    FileInfo fi6 = new FileInfo(GlobalVar.InputFilePrevProhibited);
                    ExceptionsList proADV = new ProhibitedExceptions(advs.DictSec, pros.DictSec, fi6, _scenario);
                    _pros = proADV.ExceptList;

                    //Accounts holding ETF's where status code is not FROZEN
                    ETFExceptions etfADV = new ETFExceptions(advs.DictSec, cusips.DictSec, adventAccts.DictAccts, _scenario);
                    _etfs = etfADV.ETF_Exceptions;

                    break;
            }
        }

        /// <summary>
        /// Process control method for the creation of the output report.
        /// The method uses lists of exceptions created in MAKELIST method.
        /// </summary>
        public void MakeReport()
        {
            switch (_scenario)
            {
                case "APL":
                    ExceptionReport apl = new ExceptionReport();
                    apl.CreateReport(_excepts, _pros, _etfs, "APL");
                    break;
                case "ADVENT":
                    ExceptionReport advent = new ExceptionReport();
                    advent.CreateReport(_excepts, _pros, _etfs, "ADVENT");
                    break;
            }
        }

    }
}
