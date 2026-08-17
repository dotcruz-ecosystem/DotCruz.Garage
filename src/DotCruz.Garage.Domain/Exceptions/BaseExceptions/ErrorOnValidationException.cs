using DotCruz.Garage.Domain.Exceptions.Enums;

namespace DotCruz.Garage.Domain.Exceptions.BaseExceptions;

public class ErrorOnValidationException : GarageException
{
    private readonly IEnumerable<string> _errors;

    public ErrorOnValidationException(IEnumerable<string> errors) : base(string.Empty) => _errors = errors;
    public ErrorOnValidationException(string error) : base(error) => _errors = [error];

    public override IEnumerable<string> GetErrorsMessages() => _errors;
    public override ErrorType GetErrorType() => ErrorType.Validation;
}
