using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUSIPSCAN
{
    class Program
    {
        /// <summary>
        /// Main entry point for the CUSIPSCAN program.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("************************************************************");
            Console.WriteLine("*                                                          *");
            Console.WriteLine("*          FEDERATED MDT ADVISERS CUSIPSCAN VER 2          *");
            Console.WriteLine("*                                                          *");
            Console.WriteLine("************************************************************");
            Console.WriteLine();
            string scenario = "";

            //This loop encloses the user interaction code.
            //The user can enter only three valid values:
            //1.  APL
            //2.  Advent
            //3.  Exit
            //The code exits when the user enters invalid data for the 2nd time.

            do
            {
                //Ask user which system they want to scan.  This will be the processing
                //scenario.
                Console.WriteLine("Which Scenario? APL or ADVENT?  Type EXIT to quit.");
                scenario = Console.ReadLine().ToUpper();

                //Immediately exit if user types "Exit."
                if (scenario == "EXIT")
                {
                    return;
                }

                //Give user a second chance if they type an invalid value.
                if (scenario != "APL" && scenario != "ADVENT")
                {
                    Console.WriteLine("You must have made a typing error.  Try again.");
                    Console.WriteLine("Which Scenario? APL or ADVENT?  Type EXIT to quit.");
                    scenario = Console.ReadLine().ToUpper();
                    //Immediately exit if user types "Exit."
                    if (scenario == "EXIT")
                    {
                        return;
                    }
                }

                //Exit when the user types an invalid value on the second try.
                if (scenario != "APL" && scenario != "ADVENT")
                {
                    Console.WriteLine("You must have made another typing error.");
                    Console.WriteLine("Exiting.  Hit Enter");
                    Console.ReadLine();
                    return;
                }

                //Create the file names of the input and output files.
                GlobalVar.BuildFileNames(scenario);
                if (GlobalVar.SessionErrorExists) // error handling for missing files.
                {
                    GlobalVar.LogEntry(GlobalVar.SessionErrorDescription);
                    Console.WriteLine("Session error: " + GlobalVar.SessionErrorDescription);
                    Console.WriteLine("Program ended");
                    Console.ReadLine();
                    return;
                }

                //Query the operating system to determine the user.
                GlobalVar.BuildUser();
                GlobalVar.LogEntry("CUSIPScan session started for: " + scenario);

                //Create a ListFactory object to control the processes to read in
                //data, analyze it, and then create reports.
                ListFactory lf = new ListFactory(scenario);
                lf.MakeList();
                lf.MakeReport();

                //Scenario finished.
                GlobalVar.LogEntry("CUSIPScan session finished for: " + scenario);
                Console.Write(scenario + " finished.");
                Console.ReadLine();

            } while (scenario != "EXIT");  //End of loop

            Console.Write("Program ended");
            Console.ReadLine();
        }
    }
}
