namespace SqlClientExample.DTOs;

public record GetAnimalsResponse(int IdAnimal, string Name, string Description, string Category, string Area);