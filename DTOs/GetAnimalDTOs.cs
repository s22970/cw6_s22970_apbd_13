namespace SqlClientExample.DTOs;

public record GetAnimalDetailsResponse(int IdAnimal, string Name, string Description, string Category, string Area);