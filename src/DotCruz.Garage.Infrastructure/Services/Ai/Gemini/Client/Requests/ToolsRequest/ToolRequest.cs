using DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.Enums;

namespace DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.ToolsRequest;

public class ToolRequest(ToolTypeEnum toolType) : BaseToolRequest(toolType switch
{
    ToolTypeEnum.GoogleSearch => "google_search",
    ToolTypeEnum.Function => "function",
    _ => toolType.ToString().ToLowerInvariant()
});
