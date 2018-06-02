using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;

namespace SecretKeeper
{
    public class ShareModel : PageModel
    {
        [BindProperty]
        public string Token { get; set; } = "";
        [BindProperty]
        public string Value { get; set; }
        /*        public void OnGet()
                {

                } 
                */
        public async Task<IActionResult> OnPostAsync()
        {
            HttpClient client = new HttpClient();
            var SecretValue = new Dictionary<string, string>()
            {
                { "Value", Value }
            };

            var content = new FormUrlEncodedContent(SecretValue);
            var response = await client.PostAsync($"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/api/secret/", content);
            Token = await response.Content.ReadAsStringAsync();

            return Page();
        }
    }
}