using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using WebApp.data;
using Services;
using Models;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // Configurazione del DbContext
        services.AddDbContext<AppDb>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

        // Configurazione dei servizi
        services.AddScoped<CartService>();
        services.AddScoped<ProductService>();

        // Configurazione di Identity
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;
        })
        .AddEntityFrameworkStores<AppDb>()
        .AddDefaultTokenProviders();

        // Configurazione della protezione dei dati
        services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "keys")))
            .ProtectKeysWithDpapi();

        services.AddControllersWithViews();
        services.AddRazorPages();

        services.AddHttpContextAccessor();
        services.AddSession();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSession();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            endpoints.MapControllerRoute(
                name: "cart",
                pattern: "{controller=Cart}/{action=ViewCart}/{id?}");

            endpoints.MapControllerRoute(
                name: "account",
                pattern: "Account",
                defaults: new { controller = "Account", action = "Index" });
            
            endpoints.MapControllerRoute(
                name: "checkout",
                pattern: "Cart/Checkout",
                defaults: new { controller = "Cart", action = "Checkout" });

        });
    }
}
