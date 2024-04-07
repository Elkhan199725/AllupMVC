using Microsoft.AspNetCore.Identity;

namespace AllupWebApplication.Models;

public class AppUser : IdentityUser
{
    public string? FullName { get; set; }
}
