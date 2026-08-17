using System.Globalization;

namespace DotCruz.Garage.Api.Middlewares;

public class CultureMiddleware(RequestDelegate next)
{
    private static readonly HashSet<string> _supportedCultureNames =
        new(["en", "pt-BR"], StringComparer.OrdinalIgnoreCase);

    public async Task Invoke(HttpContext context)
    {
        var requestedCulture = context.Request.Headers.AcceptLanguage.FirstOrDefault();

        var cultureInfo = new CultureInfo("en");

        if (!string.IsNullOrEmpty(requestedCulture))
        {
            var primaryCulture = requestedCulture.Split(',')[0].Split(';')[0].Trim();
            if (_supportedCultureNames.Contains(primaryCulture))
                cultureInfo = new CultureInfo(primaryCulture);
        }

        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;

        await next(context);
    }
}
