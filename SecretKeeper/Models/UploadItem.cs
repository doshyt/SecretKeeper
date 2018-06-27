using Microsoft.AspNetCore.Http;

namespace SecretKeeper.Models
{
    public class UploadItem
    {
        public string Token { get; set; } = "";

        public IFormFile FileToUpload { set; get; }
    }
}
