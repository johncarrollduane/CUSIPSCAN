using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUSIPSCAN
{
    /// <summary>
    /// This class defines the investment security object.  It could be
    /// an investment position held by a client.  It could also be a
    /// security in a reference list, such as a list of prohibited securities.
    /// </summary>
    public class Security
    {
        /// <summary>
        /// Internal account number for Federated client.
        /// </summary>
        public string SNAM { get; set; }

        /// <summary>
        /// Status code for the account holding the security
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// Indicator of the source of the data for this security.
        /// </summary>
        public string System { get; set; }

        /// <summary>
        /// Security ticker symbol
        /// </summary>
        public string Ticker { get; set; }

        /// <summary>
        /// Name of security issuer
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 9 character CUSIP
        /// </summary>
        public string CUSIP { get; set; }

        /// <summary>
        /// Number of shares or cash balance
        /// </summary>
        public double Qty { get; set; }

        /// <summary>
        /// Price used for valuation
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// Indicates whether the item is a new exception
        /// </summary>
        public bool IsNew { get; set; }

        /// <summary>
        /// Indicates whether the item is a new exception
        /// </summary>
        public bool IsETF { get; set; }

        /// <summary>
        /// Helper member for SecurityType
        /// </summary>
        private string _secType;

        /// <summary>
        /// Translates the numerical APL security type code
        /// to a string value
        /// </summary>
        public string SecurityType
        {
            get
            {
                return _secType;
            }
            set
            {
                switch (value)
                {
                    case "28":
                        _secType = "Common Stock";
                        break;
                    case "30":
                        _secType = "Warrants/Rights";
                        break;
                    case "35":
                        _secType = "Mutual Funds";
                        break;
                    case "34":
                        _secType = "Corp Bonds";
                        break;
                    case "44":
                        _secType = "Govt Bonds";
                        break;
                    default:
                        _secType = value;
                        break;
                }
            }
        }
    }
}
