using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using osVodigiWeb7.Domain.Business;
using osVodigiWeb7x.Models;

namespace Application.WebsiteCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
               
            });

            services.AddHealthChecks();

            services.AddControllersWithViews();

            ConfigureAppServices(services);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            //app.UseAuthentication();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                /**/
                endpoints.MapAreaControllerRoute(
                        name: "areas",
                        areaName: "Backoffice",
                        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                
                endpoints.MapHealthChecks("/health");

                endpoints.MapGet("/check", async context =>
                {
                    await context.Response.WriteAsync("I'm here");
                });
            });

            /* ------------------------
            

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            //app.UseAuthentication();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");

                endpoints.MapGet("/check", async context =>
                {
                    await context.Response.WriteAsync("I'm here");
                });


                endpoints.MapControllerRoute(
                     name: "default",
                     pattern: "{controller}/{action}/{id?}",
                     defaults: new { controller = "Home", action = "Index" });

                endpoints.MapAreaControllerRoute(
                        name: "areas",
                        areaName: "Backoffice",
                        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                //endpoints.MapRazorPages();
            });
            */
        }



        private void ConfigureAppServices(IServiceCollection services)
        {

            services.AddDbContext<VodigiContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("VodigiContext")));

            services.AddDbContext<VodigiLogsContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("VodigiLogsContext")));

            services.AddScoped<IAssetManager, AssetManager>();
            services.AddScoped<ILoginRepository, EntityLoginRepository>();
            services.AddScoped<IAccountRepository, EntityAccountRepository>();
            services.AddScoped<IImageRepository, EntityImageRepository>();
            services.AddScoped<IMusicRepository, EntityMusicRepository>();
            services.AddScoped<IVideoRepository, EntityVideoRepository>();
        }

    }
}
