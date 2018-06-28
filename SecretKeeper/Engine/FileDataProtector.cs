using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.DataProtection;

namespace SecretKeeper.Engine
{
    public class FileDataProtector
    {
        ITimeLimitedDataProtector _protector;

        public FileDataProtector()
        {
            _protector = DataProtectionProvider.Create(new DirectoryInfo(@"c:\app\Certificates\")).CreateProtector("Files.TimeLimited").ToTimeLimitedDataProtector();
        }

        public byte[] EncryptStream(Stream DataStream)
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                // TODO: Check if we can run this with one stream only
                DataStream.Position = 0;
                DataStream.CopyTo(mStream);
                byte[] BytesData = mStream.ToArray();

                // TODO: Make time configurable parameter or at least const
                return _protector.Protect(BytesData, lifetime: TimeSpan.FromMinutes(5));
            }

        }

        public byte[] DecryptStream(MemoryStream DataStream)
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                // TODO: Check if we can run this with one stream only
                DataStream.Position = 0;
                DataStream.CopyTo(mStream);
                byte[] BytesData = mStream.ToArray();
                return _protector.Unprotect(BytesData);
            }

        }

        public string GetContentType(string path)
        {
            var types = new Dictionary<string, string>
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

            var ext = Path.GetExtension(path).ToLowerInvariant();

            if (types.ContainsKey(ext))
            {
                return types[ext];
            }
            else
            {
                return "application/octet-stream";
            }
        }

    }
}
