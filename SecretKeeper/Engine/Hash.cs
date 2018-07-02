using System;
using System.Text;
using System.Security.Cryptography;

namespace SecretKeeper.Engine
{
    public static class Hash
    {
        public static string GetToken(Random rndController)
        {
            using (var shaProvider = SHA256.Create())
            {
                var randValue = rndController.Next(1, 10000000);
                var data = shaProvider.ComputeHash(Encoding.UTF8.GetBytes(randValue.ToString()));
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
