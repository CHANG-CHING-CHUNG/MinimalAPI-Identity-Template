using System.ComponentModel.DataAnnotations;

namespace MinimalApiIdentityTemplate.Models;

public class ForgotPasswordModel
{
    [Required] public string Email { get; set; }
}