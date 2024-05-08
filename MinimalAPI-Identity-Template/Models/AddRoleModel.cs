using System.ComponentModel.DataAnnotations;

namespace MinimalApiIdentityTemplate.Models;

public class AddRoleModel
{
    [Required] public string Name { get; set; } = string.Empty;
}