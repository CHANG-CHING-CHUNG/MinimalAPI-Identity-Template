using System.ComponentModel.DataAnnotations;

namespace MinimalApiIdentityTemplate.ExampleModels;

public class ErrorExample
{
    [Required] public string Code { get; set; } = string.Empty;
    [Required] public string Description { get; set; } = string.Empty;
}