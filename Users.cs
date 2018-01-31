using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CUSIPSCAN
{
    /// <summary>
    /// This class defines the person using an application.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Property that holds a persons 5-1-1 network ID
        /// </summary>
        public string UserID;

        /// <summary>
        /// Property that holds the user's initials (3 characters)
        /// </summary>
        public string UserInitials;

        /// <summary>
        /// Property that holds a users full name.
        /// </summary>
        public string UserName;

        /// <summary>
        /// Property that holds a user's telephone extension (4 characters)
        /// </summary>
        public string UserPhoneExt;
    }

    /// <summary>
    /// This class provides a list of users and related data about them.
    /// </summary>
    class Users
    {
        /// <summary>
        /// List that holds User data elements/
        /// </summary>
        public List<User> UserList = new List<User>();

        /// <summary>
        /// Helper member for the FullName property
        /// </summary>
        private string _fullName;

        /// <summary>
        /// Helper member for user initials property.
        /// </summary>
        private string _initials;

        /// <summary>
        /// Constructor for this class.
        /// Loads data from the UserSettings.txt file into the list
        /// called UserList.
        /// <param name="fileName">Name of User Settings file</param>
        /// </summary>
        public Users(string fileName)
        {
            FileInfo fi = new FileInfo(fileName);

            //Make sure the file exists.
            //Catch "file not found" error here before trying to open file.
            if (!fi.Exists)
            {
                ErrorMessage = "User settings file not found.";
                return;
            }

            //Read fixed column width file line by line and load into the list.
            try
            {
                using (StreamReader sr = new StreamReader(fi.FullName))
                {
                    while (!sr.EndOfStream)
                    {
                        string strData = sr.ReadLine();
                        User myUser = new User();

                        myUser.UserID = strData.Substring(0, 7).TrimEnd();
                        myUser.UserInitials = strData.Substring(8, 3);
                        myUser.UserPhoneExt = strData.Substring(12, 4);
                        myUser.UserName = strData.Substring(17, 20).TrimEnd();
                        UserList.Add(myUser);
                    }
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }

        }

        /// <summary>
        /// Full name of the user based on the network ID
        /// </summary>
        /// <param name="id">5-1-1 network ID</param>
        /// <returns>String containing the users first and last name</returns>
        public string GetFullName(string id)
        {
            _fullName = "NOT FOUND";
            foreach (User myUser in UserList)
            {
                if (myUser.UserID == id.ToUpper())
                {
                    _fullName = myUser.UserName;
                }
            }
            return _fullName;
        }

        /// <summary>
        /// Initials of the user based on the network ID.
        /// The initials are the name of the user's Advent directory.
        /// </summary>
        /// <param name="id">5-1-1 network ID</param>
        /// <returns>String containing the users 3-letter initials</returns>
        public string GetInitials(string id)
        {
            _initials = "NA";
            foreach (User myUser in UserList)
            {
                if (myUser.UserID == id.ToUpper())
                {
                    _initials = myUser.UserInitials;
                }
            }
            return _initials;
        }

        /// <summary>
        /// Property designed to hold error messages delivered back to the caller.
        /// </summary>
        public string ErrorMessage { get; set; }


    }
}
