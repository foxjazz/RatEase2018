using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Serialization;
using AspNetCore.Identity.LiteDB;
using AspNetCore.Identity.LiteDB.Data;
using AspNetCore.Identity.LiteDB.Models;

namespace ServiceFroos
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
            services.AddSingleton<LiteDbContext>();
            services.AddIdentity<ApplicationUser, AspNetCore.Identity.LiteDB.IdentityRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;
                })
                .AddUserStore<LiteDbUserStore<ApplicationUser>>()
                .AddRoleStore<LiteDbRoleStore<AspNetCore.Identity.LiteDB.IdentityRole>>()
                .AddDefaultTokenProviders();

        
        //services.AddSignalR()
        //    .AddMessagePackProtocol(options =>
        //    {
        //        options.FormatterResolvers = new List<MessagePack.IFormatterResolver>()
        //        {
        //            MessagePack.Resolvers.StandardResolver.Instance
        //        };
        //    });

        //services.AddSignalR()
        //    .AddJsonHubProtocol(options => {
        //        options.PayloadSerializerSettings.ContractResolver =
        //            new DefaultContractResolver();
        //    });
        services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSignalR();

        }

        public static string path;
        public static string inpath;
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            path = Directory.GetCurrentDirectory() + "\\bitmaps";
            inpath = Directory.GetCurrentDirectory() + "\\data";
            Directory.CreateDirectory(path);
            Directory.CreateDirectory(inpath);
            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSignalR(route =>
            {
                route.MapHub<RedHub>("/redhub");
            });
            app.UseMvc();
        }
    }
}
