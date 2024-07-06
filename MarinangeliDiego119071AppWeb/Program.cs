using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using WebApp.data;
using Microsoft.AspNetCore.Identity;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Configura il DbContext
builder.Services.AddDbContext<AppDb>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurazione Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
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

// Configurazione dei servizi
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<ProductService>();

// Configurazione di autenticazione e autorizzazione
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "WebAppVetrinaCookie";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
        options.SlidingExpiration = true;
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ReturnUrlParameter = "returnUrl";
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "WebAppVetrinaCookie";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
    options.SlidingExpiration = true;
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.ReturnUrlParameter = "returnUrl";
});

builder.Services.AddAuthorization();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

// Aggiunta della protezione dei dati
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "keys")))
    .ProtectKeysWithDpapi(); // Esempio di protezione con DPAPI

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Gestione delle eccezioni e altre configurazioni
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

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
        pattern: "{controller=Account}/{action=Login}/{id?}");
});

app.Run();
