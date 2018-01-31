using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUSIPSCAN
{
    /// <summary>
    /// This class defines a Setting object.  It is designed to
    /// be one of many in a collection of Setting objects.
    /// Each setting is described with Type, Category, and Sub-Category.
    /// Type is the broadest description.  Category and Sub-Category
    /// narrow the description to a specific item.
    /// 
    /// Generally these settings are used to store directory and file names.
    /// Type: Indicates directory or file
    /// Category: Indicates the source or destination of the data, such as 
    ///           APL, Advent, or SPOT.
    /// Sub-Category: Indicates the specific nature of the data (such as 
    ///           holdings, users, or log.
    /// </summary>
    class Setting
    {
        /// <summary>
        /// 
        /// </summary>
        public string Type;
        public string Category;
        public string SubCategory;
        public string Data;
    }

    /// <summary>
    /// This class defines the object that holds a collection of objects
    /// of type Setting.
    /// </summary>
    public class Settings
    {
        /// <summary>
        /// List collection holding Setting objects.
        /// </summary>
        private List<Setting> _settingsList = new List<Setting>();

        /// <summary>
        /// Constructor for the object.  This loads the list with the
        /// settings data from the settings text file.
        /// </summary>
        /// <param name="settingsFile">The full path and file name of the
        /// settings file.</param>
        public Settings(string settingsFile)
        {
            FileInfo fi = new FileInfo(settingsFile);
            if (!fi.Exists)
            {
                Console.WriteLine("File not found; " + fi.FullName);
                GlobalVar.SessionErrorExists = true;
                GlobalVar.SessionErrorDescription = "File not found; " + fi.FullName;
                return;
            }
            using (StreamReader sr = new StreamReader(fi.FullName))
            {
                string row;
                while ((row = sr.ReadLine())!= null)
                {
                    Setting mySetting = new Setting();
                    mySetting.Type = row.Substring(0, 3);
                    mySetting.Category = row.Substring(4, 7).Trim();
                    mySetting.SubCategory = row.Substring(14, 10).Trim();
                    mySetting.Data = row.Substring(25).Trim();
                    _settingsList.Add(mySetting);
                    mySetting = null;
                }
            }
        }

        /// <summary>
        /// This method returns a settings item based on the three supplied
        /// parameters. The property returned is Settings.Data.
        /// </summary>
        /// <param name="type">Broadest descriptor</param>
        /// <param name="category">Intermediate descriptor</param>
        /// <param name="subcategory">Narrowest descriptor</param>
        /// <returns></returns>
        public string GetSetting(string type, string category, string subcategory)
        {
            //query to return a single settings item.
            var querySetting = from mySetting in _settingsList
                               where mySetting.Type == type
                               where mySetting.Category == category
                               where mySetting.SubCategory == subcategory
                               select mySetting;
            try
            {
                return querySetting.SingleOrDefault().Data;
            }
            catch (Exception e)
            {
                Console.WriteLine( e.Message);
                return e.Message;
            }
        }

    }
}
