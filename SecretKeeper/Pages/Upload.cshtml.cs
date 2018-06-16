using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Security.Authentication;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;

namespace SecretKeeper.Pages
{
    public class UploadModel : PageModel
    {
        [BindProperty]
        public string Token { get; set; } = "";

        [BindProperty]
        public IFormFile FileToUpload { set; get; }

        public async Task<IActionResult> OnPostAsync()
        {

            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; },
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11
            };

            HttpClient client = new HttpClient(handler);

            var requestContent = new MultipartFormDataContent();
            var fileContent = new StreamContent(FileToUpload.OpenReadStream());
            fileContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue(FileToUpload.ContentType);
            requestContent.Add(fileContent, FileToUpload.FileName, FileToUpload.FileName);

            var response = await client.PostAsync($"https://{this.Request.Host}/api/secret/UploadFile", requestContent);

            Token = await response.Content.ReadAsStringAsync();

            return Page();
        }
    }
}