using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using PollBuilder.Application;
using PollBuilder.Application.Interfaces.Notifications;
using PollBuilder.Application.Interfaces.Services;
using PollBuilder.Infrastructure;
using PollBuilder.Infrastructure.Identity;
using PollBuilder.Infrastructure.Persistence;
using PollBuilder.Infrastructure.Services;
using PollBuilder.Web.Hubs;
using System.Threading.RateLimiting;


var builder = WebApplication.CreateBuilder(args);

builder.Configuration.Sources.Clear();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile(
        $"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: true,
        reloadOnChange: true)
    .AddEnvironmentVariables();

// ==============================
// Register Services
// ==============================
builder.Services.AddSignalR();
// MVC
builder.Services.AddControllersWithViews();

// Razor Pages (Identity)
builder.Services.AddRazorPages();

// Clean Architecture
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

Console.WriteLine("--------------------------------");
Console.WriteLine($"Environment = {builder.Environment.EnvironmentName}");
Console.WriteLine("Database provider = PostgreSQL");
Console.WriteLine($"Database host = {new Npgsql.NpgsqlConnectionStringBuilder(builder.Configuration.GetConnectionString("DefaultConnection")).Host}");
Console.WriteLine("--------------------------------");

builder.Services.AddScoped<IPollNotifier,
    SignalRPollNotifier>();

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Home/AccessDenied";

    options.Events.OnRedirectToLogin = context =>
    {
        var returnUrl = Uri.EscapeDataString(context.Request.Path + context.Request.QueryString);

        context.Response.Redirect(
            $"/Home/LoginRequired?returnUrl={returnUrl}");

        return Task.CompletedTask;
    };
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("VoteLimiter", limiter =>
    {
        limiter.PermitLimit = 5;                 // tối đa 5 request
        limiter.Window = TimeSpan.FromSeconds(10); // trong 10 giây
        limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiter.QueueLimit = 0;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddHttpContextAccessor();

var redisConnection = builder.Configuration["Redis:ConnectionString"]
    ?? Environment.GetEnvironmentVariable("REDIS_CONNECTION_RENDER")
    ?? Environment.GetEnvironmentVariable("REDIS_CONNECTION");

if (!string.IsNullOrWhiteSpace(redisConnection) &&
    redisConnection.StartsWith("redis://", StringComparison.OrdinalIgnoreCase))
{
    redisConnection = new Uri(redisConnection).Authority;
}

Console.WriteLine($"Redis = {redisConnection}");

var redisInstanceName = builder.Configuration["Redis:InstanceName"]
    ?? $"PollBuilder:{builder.Environment.EnvironmentName}:";

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnection;
    options.InstanceName = redisInstanceName;
});

Console.WriteLine($"Redis namespace = {redisInstanceName}");

builder.Services.AddScoped<ICacheService, RedisCacheService>();

builder.Services.AddScoped<QrCodeService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    const int maxRetry = 10;

    for (int attempt = 1; attempt <= maxRetry; attempt++)
    {
        try
        {
            var dbContext = services.GetRequiredService<ApplicationDbContext>();

            await dbContext.Database.MigrateAsync();

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = { "Admin", "Member" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            const string adminEmail = "admin@pollbuilder.com";
            const string adminPassword = "Admin@123";

            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "Administrator"
                };

                var result = await userManager.CreateAsync(admin, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            Console.WriteLine("Database initialized successfully.");

            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database is not ready. Attempt {attempt}/{maxRetry}");
            Console.WriteLine(ex.Message);

            if (attempt == maxRetry)
            {
                throw;
            }

            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    }
}



// ==============================
// Configure HTTP Pipeline
// ==============================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

app.MapHub<PollHub>("/pollHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();