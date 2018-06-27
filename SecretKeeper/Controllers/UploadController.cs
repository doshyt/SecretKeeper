using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecretKeeper.Models;
using SecretKeeper.Engine;
using Microsoft.AspNetCore.Http;

namespace SecretKeeper.Controllers
{
    [Route("[controller]")]
    public class UploadController : Controller
    {

        private FileDataProtector _protector = new FileDataProtector();
        private Random _rndController = new Random();
        
        public IActionResult Index(UploadItem model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Post(UploadItem model, IFormFile FileToUpload)
        {
            string privateFileName = Hash.GetToken(_rndController);
            privateFileName += "." + FileToUpload.FileName.Split(".").Last();

            var basePath = Path.Combine("wwwroot", "Uploads");
            var filePath = Path.Combine(basePath, privateFileName);

            using (var MemStream = new MemoryStream())
            {
                await FileToUpload.CopyToAsync(MemStream);
                byte[] ByteData = _protector.EncryptStream(MemStream);
                Stream EncryptedStream = new MemoryStream(ByteData);

                using (var FileStream = new FileStream(filePath, FileMode.Create))
                {
                    await EncryptedStream.CopyToAsync(FileStream);
                }

            }

            model.Token = $"https://{this.Request.Host}/upload/" + privateFileName;
            return View("Index", model);
        }

        [HttpGet("{token}")]
        public IActionResult GetFile(string token)
        {

            string filename = token.Split("/").Last();

            var path = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot\\Uploads", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                stream.CopyTo(memory);
            }

            String contentType = _protector.GetContentType(path);
            return File(_protector.DecryptStream(memory), contentType, Path.GetFileName(path));
        }
    }
}