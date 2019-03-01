using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecretKeeper.Models
{
    public class UploadItem
    {
        public long Id { get; set; }
        public string Token { get; set; } = "";
        [NotMapped]
        public IFormFile FileToUpload { set; get; }
        public string OriginalName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpiredBy { get; set; }
    }
}
