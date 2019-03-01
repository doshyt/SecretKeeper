using System;
using System.Text;
using System.Security.Cryptography;

namespace SecretKeeper.Engine
{
    public static class Hash
    {
        public static string GetSecureToken(RNGCryptoServiceProvider cryptoProvider)
        {
            using (var shaProvider = SHA256.Create())
            {
                var byteArray = new byte[4];
                cryptoProvider.GetBytes(byteArray);
                var randNumber = BitConverter.ToUInt32(byteArray, 0);

                var data = shaProvider.ComputeHash(Encoding.UTF8.GetBytes(randNumber.ToString()));
                var sBuilder = new StringBuilder();
                for (var i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }
    }
}
