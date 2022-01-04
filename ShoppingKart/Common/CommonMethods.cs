using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingKart.Common
{
    public static class CommonMethods
    {
        public static string SecretKey = "s@w@ecr@e@t12311@b@";
        public static string ConvertToEncrypt(string password)
        {
            if(string.IsNullOrEmpty(password))
            {
                return "";
            }
            password = password + SecretKey;
            var encryptedPassword = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(encryptedPassword);

        }

        public static string ConvertToDecrypt(string base64EncodedData)
        {
            if (string.IsNullOrEmpty(base64EncodedData))
            {
                return "";
            }
            var base64EncryptedBytes = Convert.FromBase64String(base64EncodedData);
            var result = Encoding.UTF8.GetString(base64EncryptedBytes);
            result = result.Substring(0, result.Length - SecretKey.Length);
            return result;

        }
    }
}
