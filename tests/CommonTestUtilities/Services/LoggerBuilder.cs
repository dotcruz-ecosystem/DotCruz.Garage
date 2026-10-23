using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace CommonTestUtilities.Services;

public static class LoggerBuilder
{
    public static ILogger<T> Build<T>() => NullLogger<T>.Instance;
}
