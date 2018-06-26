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
using SecretKeeper.Engine;
using System.IO;

namespace SecretKeeper.Pages
{
    public class UploadModel : PageModel
    {
        [BindProperty]
        public string Token { get; set; } = "";

        [BindProperty]
        public IFormFile FileToUpload { set; get; }

//        private readonly Random _rndController = new Random();

  //      private FileDataProtector Protector = new FileDataProtector();

        /*
        public async Task<IActionResult> OnPostAsync()
        {
            
            /*
            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; },
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11
            };

            HttpClient client = new HttpClient(handler);

            var requestContent = new MultipartFormDataContent();



                var result = new StringBuilder();
                using (var reader = new StreamReader(FileToUpload.OpenReadStream()))
                {
                    while (reader.Peek() >= 0)
                        result.AppendLine(await reader.ReadLineAsync());
                }
                
            
                */

            /*
            var fileContent = new StreamContent();

            fileContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue(FileToUpload.ContentType);
            //requestContent.Headers.ContentType = new MediaTypeWithQualityHeaderValue(FileToUpload.ContentType);
            requestContent.Add(fileContent, FileToUpload.FileName, FileToUpload.FileName);
            requestContent.Add(CreateFileContent(file.InputStream, file.FileName, "text/plain"));

            var response = await client.PostAsync($"https://{this.Request.Host}/api/secret/UploadFile", requestContent);

            Token = await response.Content.ReadAsStringAsync();

            return Page();

            */
            





            /*
            string privateFileName = Hash.GetToken(_rndController);
            privateFileName += "." + FileToUpload.FileName.Split(".").Last();
            // privateFileName += Path.GetExtension(file.FileName);

            var basePath = Path.Combine("wwwroot", "Uploads");
            var filePath = Path.Combine(basePath, privateFileName);

            using (var MemStream = new MemoryStream())
            {
                await FileToUpload.CopyToAsync(MemStream);
                byte[] ByteData = Protector.EncryptStream(MemStream);
                Stream EncryptedStream = new MemoryStream(ByteData);

                using (var FileStream = new FileStream(filePath, FileMode.Create))
                {
                    await EncryptedStream.CopyToAsync(FileStream);
                }

            }
            
            Token = $"https://{this.Request.Host}/Upload/" + privateFileName;

            return Page();
            
        }

            */

        /*
        public IActionResult OnGet(string token)
        {
            if (token != null)
            {
                string filename = token.Split("/").Last();

                var path = Path.Combine(
                         Directory.GetCurrentDirectory(),
                         "wwwroot\\Uploads", filename);

                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                String contentType = Protector.GetContentType(path);
                return File(memory, contentType, Path.GetFileName(path));

                //return RedirectToPage("./Uploads/" + token);
            }
            else
            {
                return Page();
            }
        }

    */

    }
}