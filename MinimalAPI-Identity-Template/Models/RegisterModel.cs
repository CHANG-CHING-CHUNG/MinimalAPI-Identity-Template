using System.ComponentModel.DataAnnotations;

namespace MinimalApiIdentityTemplate.Models;

public class ApplicationUser
{
    [Required] public string UserName { get; set; }

    [Required] public string Email { get; set; }

    [Required] public string Password { get; set; }

    public string? NickName { get; set; }
}