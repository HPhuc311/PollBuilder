using Microsoft.AspNetCore.Identity;

namespace PollBuilder.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
}