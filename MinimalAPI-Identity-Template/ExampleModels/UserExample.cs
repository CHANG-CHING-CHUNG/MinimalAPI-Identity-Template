using System.ComponentModel.DataAnnotations;

namespace MinimalApiIdentityTemplate.ExampleModels;

public class UserExample
{
    [Required] public string Id { get; set; } = string.Empty;
    [Required] public string UserName { get; set; } = string.Empty;

    [Required] public string Email { get; set; } = string.Empty;

    [Required] public string NickName { get; set; } = string.Empty;
    [Required] public bool EmailConfirmed { get; set; }

    [Required] public List<string> Roles { get; set; } = [""];
}