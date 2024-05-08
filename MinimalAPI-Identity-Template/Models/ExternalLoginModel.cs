using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MinimalApiIdentityTemplate.Models;

public class ExternalLoginModel
{
    [Required] [EmailAddress] public string Email { get; set; }

    public ClaimsPrincipal principal { get; set; }
}