using Microsoft.EntityFrameworkCore;
using Data;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Configurazione della stringa di connessione
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Aggiungi il DbContext al container dei servizi
builder.Services.AddDbContext<AppDb>(options =>
    options.UseSqlServer(connectionString));

// Aggiungi i servizi dell'applicazione
builder.Services.AddScoped<OrderService>();

// Aggiungi controller con viste
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configura il middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();