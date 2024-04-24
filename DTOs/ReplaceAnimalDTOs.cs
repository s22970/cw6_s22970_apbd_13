using System.ComponentModel.DataAnnotations;

namespace SqlClientExample.DTOs;

public record ReplaceAnimalRequest(
    [Required][MaxLength(200)] string Name,
    [Required][MaxLength(200)] string Description,
    [Required][MaxLength(200)] string Category,
    [Required][MaxLength(200)] string Area
);