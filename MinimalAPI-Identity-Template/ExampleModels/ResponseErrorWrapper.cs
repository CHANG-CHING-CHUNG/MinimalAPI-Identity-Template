using System.ComponentModel.DataAnnotations;

namespace MinimalApiIdentityTemplate.ExampleModels;

public class ResponseErrorWrapper<T>
{
    [Required] public string Status { get; set; } = string.Empty;
    [Required] public T Erros { get; set; } = default;
}