using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.DataProtection;

namespace SecretKeeper.Engine
{
    public class FileDataProtector
    {
        readonly ITimeLimitedDataProtector _protector;

        public FileDataProtector()
        {
            _protector = DataProtectionProvider.Create(new DirectoryInfo("Certificates"))
                .CreateProtector("Files.TimeLimited").ToTimeLimitedDataProtector();
        }

        public byte[] EncryptStream(Stream dataStream)
        {
            using (var mStream = new MemoryStream())
            {
                // TODO: Check if we can run this with one stream only
                dataStream.Position = 0;
                dataStream.CopyTo(mStream);
                var bytesData = mStream.ToArray();

                // TODO: Make time configurable parameter or at least const
                return _protector.Protect(bytesData, lifetime: TimeSpan.FromMinutes(5));
            }

        }

        public byte[] DecryptStream(MemoryStream dataStream)
        {
            using (var mStream = new MemoryStream())
            {
                // TODO: Check if we can run this with one stream only
                dataStream.Position = 0;
                dataStream.CopyTo(mStream);
                var bytesData = mStream.ToArray();
                return _protector.Unprotect(bytesData);
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

            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }

    }
}
