using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace CUSIPSCAN
{
    /// <summary>
    /// Abstract class to define the characteristics of a dictionary collection of securities.
    /// </summary>
    abstract class Securities
    {
        /// <summary>
        /// Dictionary collection of objects of type Security.
        /// </summary>
        public Dictionary<string, Security> DictSec = new Dictionary<string, Security>();

        /// <summary>
        /// List holding prohibited holdings data from previous day
        /// </summary>
        public List<Securities> ProHoldingsList = new List<Securities>();

        /// <summary>
        /// Signature of a required method for each child class that will create the collection.
        /// </summary>
        /// <param name="fi">This parameter is a FileInfo object holding the file name of the
        /// data source</param>
        public abstract void LoadDict(FileInfo fi);
    }
}
