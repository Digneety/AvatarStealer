using System.Text.Json.Serialization;

namespace AvatarStealer.Models;

public class UbisoftErrorResponse
{
    [JsonPropertyName("moreInfo")] public string MoreInfo { get; set; }
    [JsonPropertyName("httpCode")] public int HttpCode { get; set; }
    [JsonPropertyName("message")] public string Message { get; set; }
}