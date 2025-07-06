using System.Security.Cryptography;
using System.Text;

namespace TP4SCS.Library.Utils.Helpers
{
    public class MoMoHelpers
    {
        public string HmacSHA256(string rawData, string secretKey)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
