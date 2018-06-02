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

namespace SecretKeeper.Pages
{
    public class ShareModel : PageModel
    {
        [BindProperty]
        public string Token { get; set; } = "";
        [BindProperty]
        public string Value { get; set; }
        public async Task<IActionResult> OnPostAsync()
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; },
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11
            };

            HttpClient client = new HttpClient(handler);
            var SecretValue = new Dictionary<string, string>()
            {
                { "Value", Value }
            };
            var response = await client.PostAsync($"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/api/secret/", 
                new StringContent(JsonConvert.SerializeObject(SecretValue, Formatting.Indented), Encoding.UTF8, "application/json"));

            Token = await response.Content.ReadAsStringAsync();

            return Page();
        }
    }
}