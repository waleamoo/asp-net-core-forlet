using ForLet.Infrastructure;
using ForLet.Models;
using ForLet.Repository;
using ForLet.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForLet
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // add lowercase routes 
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddControllersWithViews();
            // add custom cl
            // add db context
            services.AddDbContext<ForLetDbContext>(options => options.UseSqlServer(_configuration.GetConnectionString("ForLetContext")));
            // configure Identity 
            services.AddIdentity<AppUser, IdentityRole>(options => {
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.User.RequireUniqueEmail = true; // requires unique email 
            }).
                AddEntityFrameworkStores<ForLetDbContext>().
                AddDefaultTokenProviders();

            services.AddControllers().AddNewtonsoftJson(); // for json
            // add custom services 
            services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, ApplicationUserClaimsPrincipalFactory>();
            services.Configure<SMTPConfigModel>(_configuration.GetSection("SMTPConfig"));
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAccountRepository, AccountRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // use Identity 
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
                  );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    "home", 
                    "/rental/{slug}",
                    defaults: new { controller = "Home", action = "RentalDetails" }
                );

            });
        }
    }
}
