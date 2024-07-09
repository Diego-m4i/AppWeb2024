using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Models;
using WebApp.data;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var context = services.GetRequiredService<AppDb>();
                await context.Database.MigrateAsync();
                await SeedUsersAndRoles(userManager, roleManager);
                await SeedProducts(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred seeding the DB: {ex.Message}");
            }
        }

        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

    private static async Task SeedUsersAndRoles(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { "Admin", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminUser = new ApplicationUser
        {
            UserName = "admin@example.com",
            Email = "admin@example.com",
            EmailConfirmed = true,
            FirstName = "Admin",
            LastName = "User"
        };

        if (userManager.Users.All(u => u.UserName != adminUser.UserName))
        {
            var user = await userManager.FindByEmailAsync(adminUser.Email);
            if (user == null)
            {
                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        var regularUser = new ApplicationUser
        {
            UserName = "user@example.com",
            Email = "user@example.com",
            EmailConfirmed = true,
            FirstName = "Regular",
            LastName = "User"
        };

        if (userManager.Users.All(u => u.UserName != regularUser.UserName))
        {
            var user = await userManager.FindByEmailAsync(regularUser.Email);
            if (user == null)
            {
                var result = await userManager.CreateAsync(regularUser, "User@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(regularUser, "User");
                }
            }
        }
    }

    private static async Task SeedProducts(AppDb context)
{
    if (!context.Products.Any())
    {
        var products = new[]
        {
            new Product { Name = "Mouse da Gaming", Description = "Mouse da gaming con sensore ottico avanzato e RGB", Price = 60.00m, ImageUrl = "https://resource.logitechg.com/w_692,c_lpad,ar_4:3,q_auto,f_auto,dpr_1.0/d_transparent.gif/content/dam/gaming/en/non-braid/hyjal-g502-hero/g502-hero-gallery-1-nb.png?v=1", Category = "Periferiche" },
            new Product { Name = "Apple MacBook Pro 14″ con chip M3", Description = "Splendido e potente.\nIl nuovo MacBook Pro 14″ fa un salto nel futuro con il nuovo chip M3. Il più evoluto mai progettato per un personal computer, grazie alla tecnologia a 3 nanometri sprigiona ancora più potenza, al massimo livello professionale. Il Ray tracing con accelerazione hardware insieme alla nuovissima architettura della GPU, permette alle app professionali di eseguire rendering fino a 2,5 volte più veloci e ai giochi di creare ombre e riflessi più realistici. E con un autonomia fino a 22 ore, uno splendido display Liquid Retina XDR e tutte le porte al posto giusto, MacBook Pro è un portatile Pro senza rivali.", Price = 2399, ImageUrl = "https://masterlabrepair.it/wp-content/uploads/2023/11/apple-macbook-pro-14-chip-m3-pro-galleria-1-scaled.jpeg", Category = "PC" },
            new Product { Name = "ASUS TUF GAMING A15", Description = "Progettato per il gaming professionale, assicurando durabilità e potenza anche nell'uso di tutti i giorni, il TUF Gaming A15 è un notebook equipaggiato con sistema operativo fino a Windows 11 Home, in grado di portarti alla vittoria! La nuova GPU fino a GeForce RTX\u2122 3070 offre un gameplay fluido su un display fino a 240Hz con 100% sRGB, mentre la potente CPU fino a AMD Ryzen\u2122 7 è equipaggiata con un sistema di raffreddamento migliorato che migliora le prestazioni della CPU in modo silenzioso. La batteria a lunga durata da 90 Wh abbinata alla consueta resistenza di livello militare dei TUF ti accompagneranno ovunque.", Price = 729, ImageUrl = "https://dlcdnwebimgs.asus.com/gain/1387056a-60c6-4579-a3f7-ccf65affd7fa/", Category = "PC" },
            new Product { Name = "Powerbank", Description = "Powerbank da 20000mAh per ricaricare i tuoi dispositivi in movimento.", Price = 50, ImageUrl = "https://ae01.alicdn.com/kf/S03dd5ef236a3437c9c27356ccdfdb2dfB/INIU-Power-Bank-10500mAh-caricabatterie-portatile-a-ricarica-rapida-con-supporto-per-telefono-batteria-esterna-per.png", Category = "Accessori" },
            new Product { Name = "Scrivania Automatizzata", Description = "Scrivania automatizzata regolabile in altezza con memoria di posizione", Price = 250m, ImageUrl = "https://m.media-amazon.com/images/I/61-E7zyijWL.jpg", Category = "Arredamento" },
            new Product { Name = "iPhone 15 Pro", Description = "Forgiati nel titanio, con il rivoluzionario chip A17 Pro, il tasto Azione personalizzabile e un sistema di fotocamere Pro ancora più versatile.", Price = 1139, ImageUrl = "https://images.ctfassets.net/evnrpfa1vdk4/19ppXPEmEtAWZfb8eBcIdJ/94a86c4f021984f4ae87d1ce69363cf4/iPhone_15_Pro_Max_Natural_Titanium_PDP_Image_Position-1__CAEN-copy.png", Category = "Smartphone" }
        };

        context.Products.AddRange(products);
        await context.SaveChangesAsync();
    }
}

}
