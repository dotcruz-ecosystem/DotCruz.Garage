using System.Text.Json.Serialization;

namespace DotCruz.Garage.Infrastructure.Services.Ai.Gemini.Client.Requests.Enums;

[JsonConverter(typeof(JsonStringEnumConverter<ResponseFormatSchemaTypeEnum>))]
public enum ResponseFormatSchemaTypeEnum
{
    [JsonStringEnumMemberName("object")]
    Object,
    [JsonStringEnumMemberName("array")]
    Array,
    [JsonStringEnumMemberName("string")]
    String,
    [JsonStringEnumMemberName("integer")]
    Integer,
    [JsonStringEnumMemberName("number")]
    Number,
    [JsonStringEnumMemberName("boolean")]
    Boolean
}
