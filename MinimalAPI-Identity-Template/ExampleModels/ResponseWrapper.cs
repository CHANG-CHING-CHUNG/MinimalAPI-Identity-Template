using System.ComponentModel.DataAnnotations;

namespace MinimalApiIdentityTemplate.ExampleModels;

public class ResponseWrapper<T>
{
    [Required] public string Status { get; set; } = string.Empty;
    [Required] public T Data { get; set; } = default;
}