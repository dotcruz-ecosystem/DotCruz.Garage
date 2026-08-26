using DotCruz.Garage.Application.DTOs.Drivers;
using DotCruz.Garage.Application.DTOs.Fines;
using DotCruz.Garage.Application.Mappers.Fines;
using DotCruz.Garage.Application.UseCases.Drivers.Commands.RegisterDriver;
using DotCruz.Garage.Domain.Entities.Drivers;

namespace DotCruz.Garage.Application.Mappers.Drivers;

public static class DriverMapper
{
    public static Driver ToDomain(this RegisterDriverCommand command, Guid tenantId, Guid userId)
    {
        return Driver.Create(
            tenantId,
            userId,
            command.RequestDto.Name,
            command.RequestDto.Cpf,
            command.RequestDto.Cnh,
            command.RequestDto.CnhCategory,
            command.RequestDto.CnhExpirationDate,
            command.RequestDto.Email,
            command.RequestDto.Phone
        );
    }

    public static DriverDto ToDto(this Driver driver)
    {
        return new DriverDto(
            driver.Id,
            driver.Name,
            driver.Cpf,
            driver.Cnh,
            driver.CnhCategory,
            driver.CnhExpirationDate,
            driver.Email,
            driver.Phone,
            driver.IsActive,
            driver.IsCnhExpired(),
            driver.CalculateAccumulatedPoints(),
            driver.IsSuspensionRisk()
        );
    }

    public static DriverFinesSummaryDto ToSummaryDto(this Driver driver)
    {
        var fineDtos = driver.Fines
            .Where(f => f.DeletedAt == null)
            .OrderByDescending(f => f.InfractionDate)
            .Select(f => f.ToDto())
            .ToList();

        var totalPoints = driver.CalculateAccumulatedPoints();
        var isSuspensionRisk = driver.IsSuspensionRisk();
        var totalAmount = fineDtos.Sum(f => f.Amount);

        return new DriverFinesSummaryDto(
            driver.Id,
            driver.Name,
            driver.Cpf,
            driver.Cnh,
            totalPoints,
            isSuspensionRisk,
            fineDtos.Count,
            totalAmount,
            fineDtos
        );
    }

    public static VehicleDriverAssignmentDto ToDto(this VehicleDriverAssignment assignment)
    {
        return new VehicleDriverAssignmentDto(
            assignment.Id,
            assignment.VehicleId,
            assignment.DriverId,
            assignment.Driver?.Name,
            assignment.StartedAt,
            assignment.EndedAt,
            assignment.IsCurrent,
            assignment.Notes
        );
    }
}
