using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;

namespace SecretKeeper.Engine
{
    public class FileDataProtector
    {
        IDataProtector _protector;

        public FileDataProtector()
        {
            _protector = DataProtectionProvider.Create(new DirectoryInfo(@"c:\app\Certificates\")).CreateProtector("Files.TimeLimited").ToTimeLimitedDataProtector();
        }

        public byte[] EncryptStream(Stream DataStream)
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                DataStream.Position = 0;
                DataStream.CopyTo(mStream);
                byte[] BytesData = mStream.ToArray();
                return _protector.Protect(BytesData);
            }

        }

        public string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }

    }
}
