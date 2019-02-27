using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Security.Authentication;
using System.ComponentModel.DataAnnotations;

namespace SecretKeeper.Pages
{
    public class ShareModel : PageModel
    {
        [BindProperty]
        public string Token { get; set; } = "";

        [BindProperty]
        public Ttl TimeToLive { get; set; }

        [BindProperty]
        [MaxLength(3000)]
        public string Value { get; set; }

        public enum Ttl
        {
            [Display(Name = "5 min")]
            A = 5,
            [Display(Name = "20 min")]
            B = 20,
            [Display(Name = "60 min")]
            C = 60
        };

        public async Task<IActionResult> OnPostAsync()
        {
            if (Value != null)
            {
                HttpClientHandler handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; },
                    SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11
                };

                string ttlString;
                switch (@TimeToLive)
                {
                    case Ttl.A:
                        ttlString = "5";
                        break;
                    case Ttl.B:
                        ttlString = "20";
                        break;
                    case Ttl.C:
                        ttlString = "60";
                        break;
                    default:
                        ttlString = "5";
                        break;
                }

                HttpClient client = new HttpClient(handler);
                var SecretValue = new Dictionary<string, string>()
                {

                        { "Value", @Value.Replace("\n", Environment.NewLine)},
                        { "TimeToLive", ttlString}
                };
                var response = await client.PostAsync($"https://{this.Request.Host}/api/secret/",
                    new StringContent(JsonConvert.SerializeObject(SecretValue, Formatting.Indented), Encoding.UTF8, "application/json"));

                Token = await response.Content.ReadAsStringAsync();

            }

            Value = "Processed";
            return Page();
        }
    }
}