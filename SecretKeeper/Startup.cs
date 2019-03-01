using System;
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

            services.AddMvc();
            services.AddHostedService<ExpiredRecordsCleaner>();

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(360);
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("Cache-control", "no-store");
                context.Response.Headers.Add("Pragma", "no-cache");
                await next();
            });

            app.UseHsts();
            app.UseStaticFiles();
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
