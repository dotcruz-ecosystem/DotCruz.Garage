using DotCruz.Garage.Domain.Exceptions.Enums;

namespace DotCruz.Garage.Domain.Exceptions.BaseExceptions;

public class NotFoundException(string message) : GarageException(message)
{
    public override IEnumerable<string> GetErrorsMessages() => [Message];
    public override ErrorType GetErrorType() => ErrorType.NotFound;
}
