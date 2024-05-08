using System.ComponentModel.DataAnnotations;

namespace MinimalApiIdentityTemplate.Models;

public class RoleModel
{
    [Required] public string Name { get; set; } = string.Empty;
}