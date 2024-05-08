using System.ComponentModel.DataAnnotations;

namespace MinimalApiIdentityTemplate.Models;

public class ConfirmResetPasswordModel
{
    [Required] public string UserId { get; set; }

    [Required] public string Token { get; set; }

    [Required] public string NewPassword { get; set; }
}