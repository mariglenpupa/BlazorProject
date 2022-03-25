using System.Security.Cryptography;
using System.Text;

namespace Panel.Shared
{
    public static class UserHelper
    {
        public static string HashPass(string userName, string pass)
        {
            string us = Convert.ToBase64String(Encoding.UTF8.GetBytes(userName));
            var data = Encoding.UTF8.GetBytes(pass + us);
            using SHA512 shaM = SHA512.Create();
            try
            {
                return Encoding.UTF8.GetString(shaM.ComputeHash(data));
            }
            catch
            {
                return "ERR:" + Convert.ToBase64String(shaM.ComputeHash(data));
            }
        }
    }
}
