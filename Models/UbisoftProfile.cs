using System.Text.Json.Serialization;

namespace AvatarStealer.Models;

public class UbisoftProfileResponse
{
    [JsonPropertyName("profiles")] public IEnumerable<UbisoftProfile> Profiles { get; set; }
}

public class UbisoftProfile
{
    [JsonPropertyName("profileId")] public Guid ProfileId { get; set; }
    [JsonPropertyName("userId")] public Guid UserId { get; set; }
    [JsonPropertyName("nameOnPlatform")] public string? NameOnPlatform { get; set; }
}