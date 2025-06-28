using System;

namespace Wfm.Services.Security
{
    public class PasswordHelper
    {

        public static string EncryptDecrypt(string pString)
        {
            string encryptedString = string.Empty;
            foreach (char c in pString)
            {
                encryptedString = encryptedString + Convert.ToChar(256 - (int)c);
            }
            return encryptedString;
        }

    }
}
