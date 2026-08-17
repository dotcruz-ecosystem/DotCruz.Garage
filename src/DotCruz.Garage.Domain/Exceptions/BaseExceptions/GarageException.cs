using DotCruz.Garage.Domain.Exceptions.Enums;

namespace DotCruz.Garage.Domain.Exceptions.BaseExceptions;

public abstract class GarageException(string message) : Exception(message)
{
    public abstract IEnumerable<string> GetErrorsMessages();
    public abstract ErrorType GetErrorType();
}
