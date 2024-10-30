using System.Text.Json.Serialization;

namespace AvatarStealer.Models;

public class UbisoftToken
{
    [JsonPropertyName("sessionId")] public Guid SessionId { get; set; }
    [JsonPropertyName("ticket")] public string? Ticket { get; set; }
    [JsonPropertyName("environment")] public string? Environment { get; set; }
    [JsonPropertyName("expiration")] public DateTimeOffset Expiration { get; set; }
    [JsonPropertyName("profileId")] public Guid ProfileId { get; set; }
    [JsonPropertyName("userId")] public Guid UserId { get; set; }

    [JsonPropertyName("nameOnPlatform")] public string? NameOnPlatform { get; set; }
}