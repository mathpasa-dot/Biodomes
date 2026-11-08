using System.Threading.RateLimiting;
using BioDomes.Domains.BiomesDom;
using BioDomes.Domains.EquipmentsDom;
using BioDomes.Domains.SpeciesDom;
using BioDomes.Infrastructures;
using BioDomes.Infrastructures.Model;
using BioDomes.Web.Middlewares;
using BioDomes.Web.Services;
using BioDomes.Web.Transformers;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Dossier des données persistantes (base SQLite + clés de Data Protection).
// En conteneur, il est monté sur un volume Docker (voir docker-compose.yml).
var dataDirectory = builder.Configuration["DataDirectory"];
if (string.IsNullOrWhiteSpace(dataDirectory))
    dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "Data");
Directory.CreateDirectory(dataDirectory);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto |
        ForwardedHeaders.XForwardedHost;

    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();

    // Derrière un reverse proxy dans un réseau Docker (Caddy), l'IP du proxy n'est pas fixe :
    // on fait confiance aux en-têtes X-Forwarded-* uniquement si c'est explicitement activé
    // et que l'application n'est pas exposée directement sur Internet.
    if (!builder.Configuration.GetValue("ReverseProxy:TrustForwardedHeaders", false))
        options.KnownProxies.Add(System.Net.IPAddress.Loopback);
});

// Clés de chiffrement des cookies persistées : les sessions survivent aux redémarrages du conteneur.
builder.Services.AddDataProtection()
    .SetApplicationName("BioDomes")
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(dataDirectory, "keys")));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.Configure<RouteOptions>(options => { options.ConstraintMap.Add("kebab", typeof(SlugTransformer)); });
builder.Services.AddScoped<ReverseProxyLinksMiddleware>();
builder.Services.AddWebOptimizer(pipeline =>
{
    pipeline.MinifyCssFiles("/css/bundled.css", "css/site.css");
    pipeline.MinifyJsFiles("/js/bundled.js", "js/site.js");
});
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddScoped<ISpeciesRepository, SpeciesRepository>();
builder.Services.AddScoped<IEquipmentRepository, EquipmentRepository>();
builder.Services.AddScoped<IBiomeRepository, BiomeRepository>();
builder.Services.AddScoped<FileService>();

var connectionString = builder.Configuration.GetConnectionString("BioDomesDbContext")
                       ?? $"Data Source={Path.Combine(dataDirectory, "biodomes.sqlite")}";

builder.Services.AddDbContext<BioDomesDbContext>(options =>
    options.UseSqlite(connectionString));


builder.Services
    .AddDefaultIdentity<User>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<BioDomesDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Mot de passe
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;

    // Verrouillage après tentatives échouées
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;

    // Email unique obligatoire
    options.User.RequireUniqueEmail = true;

    // Confirmation email requise pour se connecter
    options.SignIn.RequireConfirmedEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Login";
    options.AccessDeniedPath = "/Identity/AccessDenied";
});

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromSeconds(10),
                QueueLimit = 0
            }));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BioDomesDbContext>();
    await db.Database.MigrateAsync();
}

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Seed roles
    foreach (var role in new[] { "Admin", "Employee" })
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
}

// Comptes de démonstration (désactivables via Seed__DemoUsers=false)
if (app.Configuration.GetValue("Seed:DemoUsers", true))
{
    using var scope = app.Services.CreateScope();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

    // Seed users
    // Les mots de passe viennent de la configuration (variables d'environnement en production).
    var adminPassword = app.Configuration["Seed:AdminPassword"] ?? "Test1234$";
    var demoPassword = app.Configuration["Seed:DemoPassword"] ?? "Test1234$";

    var seeds = new[]
    {
        new { UserName = "Alice", Email = "alice@company.com", Password = adminPassword, Role = "Admin" },
        new { UserName = "Bob", Email = "bob@company.com", Password = demoPassword, Role = "Employee"    },
        new { UserName = "Celia", Email = "celia@company.com", Password = demoPassword, Role = "Employee"    }
    };

    foreach (var seed in seeds)
    {
        var user = await userManager.FindByEmailAsync(seed.Email);
        if (user == null)
        {
            user = new User
            {
                UserName = seed.UserName,
                Email = seed.Email,
                EmailConfirmed = true,
                DateOfBirth = new DateTime(1990, 1, 1),
            };
            var created = await userManager.CreateAsync(user, seed.Password);
            if (!created.Succeeded)
            {
                app.Logger.LogError("Impossible de créer le compte de démo {Email} : {Errors}", seed.Email,
                    string.Join(", ", created.Errors.Select(e => e.Description)));
                continue;
            }
        }

        if (!await userManager.IsInRoleAsync(user, seed.Role))
            await userManager.AddToRoleAsync(user, seed.Role);
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseForwardedHeaders();

// IMPORTANT: Apply reverse proxy middleware FIRST
app.ReverseProxyMiddleware();

app.UseHttpsRedirection();

// THEN apply WebOptimizer (needs to know about path base)
app.UseWebOptimizer();

// Configure static files with explicit content type provider
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".css"] = "text/css";
provider.Mappings[".js"] = "application/javascript";
provider.Mappings[".json"] = "application/json";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider,
    OnPrepareResponse = ctx =>
    {
        // Force correct content types
        if (ctx.File.Name.EndsWith(".css"))
            ctx.Context.Response.ContentType = "text/css";
        else if (ctx.File.Name.EndsWith(".js")) ctx.Context.Response.ContentType = "application/javascript";
    }
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.MapRazorPages();

app.Run();