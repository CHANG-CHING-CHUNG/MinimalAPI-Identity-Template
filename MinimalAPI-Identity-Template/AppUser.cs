using Microsoft.AspNetCore.Identity;

namespace MinimalApiIdentityTemplate;

public class AppUser : IdentityUser
{
    public string? NickName { get; set; }
}