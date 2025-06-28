using System;
using System.IO;
using System.Text.RegularExpressions;
using Wfm.Core;

namespace Wfm.Services.Helpers
{
    /// <summary>
    /// Represents a Generic Helper
    /// </summary>
    public partial class GenericHelper : IGenericHelper
    {
        public virtual bool IsSearchableDigits(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;

            bool isNum = true;

            // ************************************************
            // Analyze the search key
            // ************************************************
            string[] separators = { " ", ",", ".", "!", "?", ";", ":", "-", "_", "~", "#", "[", "]", "(", ")", "/", "\\", "*", "=", "+", "<", ">", "{", "}" };
            string[] keywords = str.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            // Check if the searchKey is digits
            for (int i = 0; i < keywords.Length; i++)
            {
                if (!CommonHelper.IsDigitsOnly(keywords[i]))
                {
                    isNum = false;
                    break;
                }
            }

            return isNum;
        }

        public virtual string ToSearchableString(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            bool isNum = IsSearchableDigits(str);

            str = str.Trim();
            string searchableString = string.Empty;
            if (isNum)
            {
                // Extract digits only
                searchableString = Regex.Replace(str, @"[^0-9]", "");
            }
            else
            {
                // Remove unprintable characters
                searchableString = Regex.Replace(str, @"[^\x20-\x7E\n\p{Sc}]", "");
                // Remove multiple spaces
                searchableString = Regex.Replace(searchableString, @"\s+", " ");
            }

            return searchableString;
        }


        // This method accepts two strings the represent two files to 
        // compare. A return value of 0 indicates that the contents of the files
        // are the same. A return value of any other value indicates that the 
        // files are not the same.
        public bool FileCompare(string file1, string file2)
        {
             // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            using (FileStream fs1 = new FileStream(file1, FileMode.Open),
                              fs2 = new FileStream(file2, FileMode.Open))
            {
                int file1byte;
                int file2byte;

                // Check the file sizes. If they are not the same, the files 
                // are not the same.
                if (fs1.Length != fs2.Length)
                {
                    // Close the file
                    fs1.Close();
                    fs2.Close();

                    // Return false to indicate files are different
                    return false;
                }

                // Read and compare a byte from each file until either a
                // non-matching set of bytes is found or until the end of
                // file1 is reached.
                do
                {
                    // Read one byte from each file.
                    file1byte = fs1.ReadByte();
                    file2byte = fs2.ReadByte();
                }
                while ((file1byte == file2byte) && (file1byte != -1));

                // Close the files.
                fs1.Close();
                fs2.Close();

                // Return the success of the comparison. "file1byte" is 
                // equal to "file2byte" at this point only if the files are 
                // the same.
                return ((file1byte - file2byte) == 0);

            }
        }

    }
}