using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretKeeper.Models
{
    public class UploadItem
    {
        public string Token { get; set; } = "";

        public IFormFile FileToUpload { set; get; }
    }
}
