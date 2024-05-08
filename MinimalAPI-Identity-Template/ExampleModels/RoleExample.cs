using System.ComponentModel.DataAnnotations;

namespace MinimalApiIdentityTemplate.ExampleModels;

public class RoleExample
{
    [Required] public string Id { get; set; } = string.Empty;
    [Required] public string Name { get; set; } = string.Empty;
}