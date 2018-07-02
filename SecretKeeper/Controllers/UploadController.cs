using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecretKeeper.Models;
using SecretKeeper.Engine;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Net;

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
        public async Task<IActionResult> Post(UploadItem model, IFormFile fileToUpload)
        {

            if (fileToUpload != null)
            {
                // limit size to 100MB
                if (fileToUpload.Length > 104857600)
                {
                    return BadRequest("File is to large. Allowed size < 100MB");
                }

                var privateFileName = Hash.GetToken(_rndController);
                var safeFileName = WebUtility.HtmlEncode(Path.GetFileName(fileToUpload.FileName));
                var fileExtension = Path.GetExtension(safeFileName);
                
                if (!string.IsNullOrEmpty(fileExtension))
                {
                    privateFileName += "." + fileExtension;
                }
                
                var basePath = Path.Combine("wwwroot", "Uploads");
                var filePath = Path.Combine(basePath, privateFileName);

                using (var memStream = new MemoryStream())
                {
                    await fileToUpload.CopyToAsync(memStream);
                    var byteData = _protector.EncryptStream(memStream);
                    Stream encryptedStream = new MemoryStream(byteData);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await encryptedStream.CopyToAsync(fileStream);
                    }

                }

                model.Token = $"https://{this.Request.Host}/upload/" + privateFileName;
                _context.UploadItems.Add(new UploadItem { Token = privateFileName, OriginalName = safeFileName, CreatedDate = DateTime.Now });
                _context.SaveChanges();
            }

            return View("Index", model);
        }

        [HttpGet("{token}")]
        public IActionResult GetFile(string token)
        {
            var id = _context.UploadItems
            .FirstOrDefault(b => b.Token == token);

            UploadItem item = null;
            var originalName = "";
            var filename = token;
            var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", "Uploads", filename);

            try
            {
                item = _context.UploadItems.Find(id.Id);
                originalName = item.OriginalName;
            }

            catch (NullReferenceException)
            {
                return RedirectToAction("Index", "StaticFile");
            }

            catch (CryptographicException)
            {
                // cleanup file if it has expired
                // TODO: implement scheduled cleanup for expired but never accessed file
                FileOperator.DeleteUploadedFile(path);
                return Ok("The requested file has expired");
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                stream.CopyTo(memory);
            }

            _context.UploadItems.Remove(item);
            _context.SaveChanges();

            if (FileOperator.DeleteUploadedFile(path) == -1)
            {
                return BadRequest("File operation failed");
            }

            var contentType = _protector.GetContentType(path);
            return File(_protector.DecryptStream(memory), contentType, originalName);
        }
    }
}