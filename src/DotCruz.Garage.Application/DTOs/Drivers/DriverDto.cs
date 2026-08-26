namespace DotCruz.Garage.Application.DTOs.Drivers;

public record DriverDto(
    Guid Id,
    string Name,
    string Cpf,
    string Cnh,
    string CnhCategory,
    DateTimeOffset CnhExpirationDate,
    string Email,
    string Phone,
    bool IsActive,
    bool IsCnhExpired,
    int AccumulatedPointsLast12Months,
    bool IsSuspensionRisk);
