using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SecretKeeper.Engine
{
    public class Hash
    {
        public static string GetToken()
        {
            using (SHA256 ShaProvider = SHA256.Create())
            {
                Random rnd = new Random();
                int randValue = rnd.Next(1, 100000);


                byte[] data = ShaProvider.ComputeHash(Encoding.UTF8.GetBytes(randValue.ToString()));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }
    }
}
