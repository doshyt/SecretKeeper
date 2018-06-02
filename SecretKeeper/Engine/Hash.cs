using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SecretKeeper.Engine
{
    public static class Hash
    {
        public static string GetToken(Random rndController)
        {
            using (SHA256 ShaProvider = SHA256.Create())
            {
                int randValue = rndController.Next(1, 10000000);
                byte[] data = ShaProvider.ComputeHash(Encoding.UTF8.GetBytes(randValue.ToString()));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }
    }
}
