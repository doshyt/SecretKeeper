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
        private readonly RNGCryptoServiceProvider _rndProvider;

        public SecretController(SecretContext context)
        {
            _protector = DataProtectionProvider.Create("SecretKeeper").CreateProtector("Secrets.TimeLimited").ToTimeLimitedDataProtector();
            _rndProvider = new RNGCryptoServiceProvider();
            _context = context;

        }

        [HttpGet("{token}", Name ="GetByToken")]
        public IActionResult GetByToken(string token)
        {

            var id = _context.SecretItems
                    .FirstOrDefault(b => b.Token == token);
            try
            {
                var item = _context.SecretItems.Find(id.Id);
                var secretValue = item.Value;
                _context.SecretItems.Remove(item);
                _context.SaveChanges();

                return Ok(_protector.Unprotect(secretValue));
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

            var token = Hash.GetSecureToken(_rndProvider);


            if (item.TimeToLive == null)
            {
                // defaults to 5 minutes
                item.TimeToLive = "5";
            }

            var lifespan = TimeSpan.FromMinutes(Convert.ToInt32(item.TimeToLive));
            var protectedValue = _protector.Protect(item.Value, lifespan);

            _context.SecretItems.Add(new SecretItem { Value = protectedValue, Token = token, ExpiredBy = DateTime.Now + lifespan });
            _context.SaveChanges();
            var link = $"https://{this.Request.Host}/api/secret/" + token;
            return Ok(link);
        }
    
    }
}
