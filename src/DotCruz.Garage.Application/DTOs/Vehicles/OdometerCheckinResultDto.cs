namespace DotCruz.Garage.Application.DTOs.Vehicles;

public record OdometerCheckinResultDto(
    Guid VehicleId,
    string VehicleName,
    int OldOdometer,
    int NewOdometer,
    DateTimeOffset RecordedAt,
    bool SchedulesRecalculated);
