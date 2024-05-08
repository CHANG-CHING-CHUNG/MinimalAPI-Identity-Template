using System.ComponentModel.DataAnnotations;

namespace MinimalApiIdentityTemplate.ExampleModels;

public class MessageExample
{
    [Required] public string Message { get; set; } = string.Empty;
}