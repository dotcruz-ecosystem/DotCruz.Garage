using CommonTestUtilities.Data;
using CommonTestUtilities.Entities;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Services;
using DotCruz.Garage.Application.DTOs.Drivers;
using DotCruz.Garage.Application.UseCases.Drivers.Commands.AssignDriverToVehicle;

namespace UseCases.Test.Drivers;

public class AssignDriverToVehicleCommandHandlerTest
{
    [Fact]
    public async Task Successfully_Assigns_Driver_And_Closes_Previous_Assignment()
    {
        var workspaceId = Guid.NewGuid();
        var vehicle = VehicleBuilder.Build(workspaceId: workspaceId);
        var previousDriver = DriverBuilder.Build(workspaceId: workspaceId);
        var newDriver = DriverBuilder.Build(workspaceId: workspaceId);

        var previousAssignment = VehicleDriverAssignmentBuilder.Build(
            workspaceId: workspaceId,
            vehicleId: vehicle.Id,
            driverId: previousDriver.Id);

        var (driverRead, _) = new DriverRepositoryBuilder().SetupGetById(newDriver).Build();
        var (vehicleRead, _) = new VehicleRepositoryBuilder().SetupGetById(vehicle).Build();
        var (_, assignWrite) = new VehicleDriverAssignmentRepositoryBuilder()
            .SetupGetCurrentByVehicleIdToUpdate(previousAssignment)
            .Build();

        var workspaceProvider = new WorkspaceProviderBuilder().AsWorkspace(workspaceId).Build();

        var handler = new AssignDriverToVehicleCommandHandler(
            driverRead,
            vehicleRead,
            assignWrite,
            workspaceProvider,
            UnitOfWorkBuilder.Build());

        var reqDto = new AssignDriverRequestDto(vehicle.Id, Notes: "Novo condutor responsável");
        var command = new AssignDriverToVehicleCommand(newDriver.Id, reqDto);

        var assignmentId = await handler.Handle(command, TestContext.Current.CancellationToken);

        Assert.NotEqual(Guid.Empty, assignmentId);
        Assert.NotNull(previousAssignment.EndedAt);
        Assert.False(previousAssignment.IsCurrent);
    }
}
