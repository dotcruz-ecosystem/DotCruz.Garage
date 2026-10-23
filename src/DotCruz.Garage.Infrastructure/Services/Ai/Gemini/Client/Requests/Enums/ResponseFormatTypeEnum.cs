using System.Text.Json.Serialization;

namespace DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<ResponseFormatTypeEnum>))]
public enum ResponseFormatTypeEnum
{
    [JsonStringEnumMemberName("text")]
    Text
}
