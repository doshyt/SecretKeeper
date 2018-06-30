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
        public async Task<IActionResult> Post(UploadItem model, IFormFile FileToUpload)
        {

            if (FileToUpload != null)
            {
                // limit size to 100MB
                if (FileToUpload.Length > 104857600)
                {
                    return BadRequest("File is to large. Allowed size < 100MB");
                }

                string privateFileName = Hash.GetToken(_rndController);
                string safeFileName = WebUtility.HtmlEncode(Path.GetFileName(FileToUpload.FileName));
                string fileExtension = Path.GetExtension(safeFileName);
                
                if (!String.IsNullOrEmpty(fileExtension))
                {
                    privateFileName += "." + fileExtension;
                }
                
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
                _context.UploadItems.Add(new UploadItem { Token = privateFileName, OriginalName = safeFileName, CreatedDate = DateTime.Now });
                _context.SaveChanges();
            }

            return View("Index", model);
        }

        [HttpGet("{token}")]
        public IActionResult GetFile(string token)
        {
            var id = _context.UploadItems
           .Where(b => b.Token == token)
            .FirstOrDefault();

            UploadItem item = null;
            string originalName = "";
            string filename = token;
            string path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot", "Uploads", filename);

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

            String contentType = _protector.GetContentType(path);
            return File(_protector.DecryptStream(memory), contentType, originalName);
        }
    }
}