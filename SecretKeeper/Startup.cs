using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using SecretKeeper.Models;

namespace SecretKeeper
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SecretContext>(opt =>
                opt.UseInMemoryDatabase("Secrets"));

            services.AddDbContext<UploadContext>(opt =>
                opt.UseInMemoryDatabase("Uploads"));

            services.AddDataProtection().SetApplicationName("SecretKeeper");


            // TODO: Add secure headers
            // TODO: Use HSTS

            /* HTTPS
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });
            */

           services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            /* HTTPS
            var options = new RewriteOptions().AddRedirectToHttps();
            app.UseRewriter(options);
            */
            app.UseStaticFiles();
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Index}/{action}/");
                routes.MapRoute(
                    name: "upload",
                    template: "{controller=Upload}/{token}/"); 
            });
        }
    }
}
