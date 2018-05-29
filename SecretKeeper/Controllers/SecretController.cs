﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecretKeeper.Models;
using System.Text;
using SecretKeeper.Engine;

namespace SecretKeeper.Controllers
{
    [Route("api/[controller]")]
    public class SecretController : ControllerBase
    {
        private readonly SecretContext _context;

        public SecretController(SecretContext context)
        {
            _context = context;

            if (_context.SecretItems.Count() == 0)
            {
  //              _context.SecretItems.Add(new SecretItem { Value = "Secret1", Token = Hash.GetToken() });
  //              _context.SaveChanges();
            }
        }

        [HttpGet]
        public List<SecretItem> GetAll()
        {
            return _context.SecretItems.ToList();
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

                return Ok(SecretValue);
            }
            catch (NullReferenceException)
            {
                return NotFound();
            }
            

        }

        // POST api/secrets
        [HttpPost]
        public IActionResult Create([FromBody] SecretItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }
            string token = Hash.GetToken();
            _context.SecretItems.Add(new SecretItem { Value = item.Value, Token = token });
            _context.SaveChanges();
            string link = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/api/secret/" + token;
            return Ok(link);
        }

        // PUT api/secrets/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/secrets/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}