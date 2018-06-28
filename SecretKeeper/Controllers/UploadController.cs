using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecretKeeper.Models;
using SecretKeeper.Engine;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;

namespace SecretKeeper.Controllers
{
    [Route("[controller]")]
    public class UploadController : Controller
    {
        
        private readonly FileDataProtector _protector = new FileDataProtector();
        // TODO: Implement secure random number generation
        private readonly Random _rndController = new Random();
        private readonly UploadContext _context;

        public UploadController(UploadContext context)
        {
            _context = context;
        }

        public IActionResult Index(UploadItem model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Post(UploadItem model, IFormFile FileToUpload)
        {
            // TODO: Limit upload file size
            // TODO: Secure upload according to best practices

            if (FileToUpload != null)
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
                _context.UploadItems.Add(new UploadItem { Token = privateFileName, OriginalName = FileToUpload.FileName, CreatedDate = DateTime.Now });
                _context.SaveChanges();
            }

            return View("Index", model);
        }

        [HttpGet("{token}")]
        public IActionResult GetFile(string token)
        {
            // TODO: Make it one-time link
            // TODO: Cleanup after access
            // TODO: Add proper error handling for invalid token

            var id = _context.UploadItems
           .Where(b => b.Token == token)
            .FirstOrDefault();

            UploadItem item = null;
            string originalName = ""; 
            try
            {
                item = _context.UploadItems.Find(id.Id);
                originalName = item.OriginalName;
            }

            catch (NullReferenceException)
            {
                return RedirectToAction("Index", "StaticFile");
                //return Ok("The requested file has not been found");
            }

            catch (CryptographicException)
            {
                return Ok("The requested file has expired");
            }

            string filename = token.Split("/").Last();

            var path = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot\\Uploads", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                stream.CopyTo(memory);
            }

            _context.UploadItems.Remove(item);
            _context.SaveChanges();

            String contentType = _protector.GetContentType(path);
            return File(_protector.DecryptStream(memory), contentType, originalName);
        }
    }
}