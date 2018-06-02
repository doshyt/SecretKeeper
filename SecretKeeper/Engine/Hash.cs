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
        public static string GetToken()
        {
            using (SHA256 ShaProvider = SHA256.Create())
            {
                Random rnd = new Random();
                int randValue = rnd.Next(1, 100000);


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
