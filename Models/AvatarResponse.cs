using System.Text.Json.Serialization;

namespace AvatarStealer.Models;

public class UbisoftAvatar
{
    [JsonPropertyName("profileId")] public Guid ProfileId { get; set; }
    [JsonPropertyName("spaceId")] public Guid SpaceId { get; set; }
    [JsonPropertyName("type")] public string? Type { get; set; }
    [JsonPropertyName("imageVariations")] public IEnumerable<ImageVariation>? ImageVariations { get; set; }
}

public class ImageVariation
{
    [JsonPropertyName("width")] public int Width { get; set; }
    [JsonPropertyName("url")] public string? Url { get; set; }
}