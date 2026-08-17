using DotCruz.Garage.Api.Extensions;
using DotCruz.Garage.Application.DTOs.Base;
using DotCruz.Garage.CrossCutting.LogResources;
using DotCruz.Garage.Domain.Exceptions.BaseExceptions;
using DotCruz.Garage.Domain.Exceptions.Resources;
using Microsoft.AspNetCore.Diagnostics;

namespace DotCruz.Garage.Api.Handlers;

public class GarageExceptionHandler(ILogger<GarageExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is GarageException garageException)
        {
            var (domainError, domainStatus) = HandleDomainException(garageException);

            httpContext.Response.StatusCode = domainStatus;
            await httpContext.Response.WriteAsJsonAsync(domainError, cancellationToken);
            return true;
        }

        var (unknownError, unknownStatus) = HandleUnknownException(exception);

        httpContext.Response.StatusCode = unknownStatus;
        await httpContext.Response.WriteAsJsonAsync(unknownError, cancellationToken);

        return true;
    }

    private (ErrorResponseDto errorResponse, int statusCode) HandleDomainException(GarageException garageException)
    {
        var errors = string.Join(", ", garageException.GetErrorsMessages());

        logger.LogWarning(
            garageException,
            ResourceMessagesLog.HANDLED_EXCEPTION,
            garageException.GetErrorType(),
            garageException.GetErrorType().ToStatusCode(),
            errors
        );

        return (
            new ErrorResponseDto(garageException.GetErrorsMessages()),
            garageException.GetErrorType().ToStatusCode()
        );
    }

    private (ErrorResponseDto errorResponse, int statusCode) HandleUnknownException(Exception exception)
    {
        logger.LogError(
            exception,
            ResourceMessagesLog.UNHANDLED_EXCEPTION,
            StatusCodes.Status500InternalServerError,
            exception.Message
        );

        return (
            new ErrorResponseDto(ResourceMessagesException.UNKNOWN_ERROR),
            StatusCodes.Status500InternalServerError
        );
    }
}
