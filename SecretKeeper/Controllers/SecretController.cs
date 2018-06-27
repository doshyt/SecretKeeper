using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SecretKeeper.Models;
using SecretKeeper.Engine;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using System.Security.Cryptography;

namespace SecretKeeper.Controllers
{

    [Route("api/[controller]")]
    public class SecretController : ControllerBase
    {
        private readonly ITimeLimitedDataProtector _protector;
        private readonly SecretContext _context;
        private readonly Random _rndController;

        public SecretController(SecretContext context)
        {
            _protector = DataProtectionProvider.Create(new DirectoryInfo(@"c:\app\Certificates\")).CreateProtector("Secrets.TimeLimited").ToTimeLimitedDataProtector();
            _rndController = new Random();
            _context = context;

        }

        [HttpGet("{token}", Name ="GetByToken")]
        public IActionResult GetByToken(string token)
        {

            var id = _context.SecretItems
                    .Where(b => b.Token == token)
                    .FirstOrDefault();
            try
            {
                var item = _context.SecretItems.Find(id.Id);
                string SecretValue = item.Value;
                _context.SecretItems.Remove(item);
                _context.SaveChanges();

                return Ok(_protector.Unprotect(SecretValue));
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
            
            catch (CryptographicException)
            {
                return Ok("This secret has expired");
            }

        }

        [HttpPost]
        public IActionResult Create([FromBody] SecretItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            if (item.Value == null)
            {
                return Ok();
            }

            string token = Hash.GetToken(_rndController);
            string protectedValue = _protector.Protect(item.Value, lifetime: TimeSpan.FromMinutes(5));
            _context.SecretItems.Add(new SecretItem { Value = protectedValue, Token = token });
            _context.SaveChanges();
            string link = $"https://{this.Request.Host}/api/secret/" + token;
            return Ok(link);
        }
    
    }
}
