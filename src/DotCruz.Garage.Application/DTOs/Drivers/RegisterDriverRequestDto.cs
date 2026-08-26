namespace DotCruz.Garage.Application.DTOs.Drivers;

public record RegisterDriverRequestDto(
    string Name,
    string Cpf,
    string Cnh,
    string CnhCategory,
    DateTimeOffset CnhExpirationDate,
    string Email,
    string Phone);
