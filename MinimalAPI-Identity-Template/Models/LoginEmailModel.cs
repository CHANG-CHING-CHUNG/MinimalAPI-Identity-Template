using System.ComponentModel.DataAnnotations;

namespace MinimalApiIdentityTemplate.Models;

public class LoginEmailModel
{
    [Required] public string Email { get; set; } = string.Empty;
}