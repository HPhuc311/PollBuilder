using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PollBuilder.Infrastructure.Identity;
using PollBuilder.Infrastructure.Persistence;
using PollBuilder.Application.Interfaces.Repositories;
using PollBuilder.Infrastructure.Repositories;

namespace PollBuilder.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? Environment.GetEnvironmentVariable("DATABASE_CONNECTION")
            ?? throw new InvalidOperationException(
                "Database connection string 'ConnectionStrings:DefaultConnection' is missing.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });
        // Identity
        services
            .AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;

                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IPollRepository, PollRepository>();

        services.AddScoped<IVoteRepository, VoteRepository>();

        return services;
    }
}