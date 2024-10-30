using System.Text;
using System.Text.Json;
using AvatarStealer.Interfaces;
using AvatarStealer.Models;
using Microsoft.Extensions.Logging;

namespace AvatarStealer;

public class UbisoftClient : IUbisoftClient
{
    private readonly HttpClient _httpClient = new();
    private readonly ILogger<UbisoftClient> _logger;

    public UbisoftClient(ILogger<UbisoftClient> logger)
    {
        _logger = logger;
        _httpClient.DefaultRequestHeaders.Add("Ubi-AppId", "e3d5ea9e-50bd-43b7-88bf-39794f4e3d40");
    }

    public async Task<UbisoftToken?> GetTokenAsync(string email, string password)
    {
        using var request =
            new HttpRequestMessage(HttpMethod.Post, "https://public-ubiservices.ubi.com/v3/profiles/sessions");

        request.Headers.Add("Authorization",
            $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{email}:{password}"))}");
        request.Content = new StringContent("{\"Content-Type\": \"application/json\", \"rememberMe\": \"false\"}",
            Encoding.UTF8, "application/json");

        var tokenResponse = await _httpClient.SendAsync(request);

        if (!tokenResponse.IsSuccessStatusCode)
        {
            var errorResponse = JsonSerializer.Deserialize<UbisoftErrorResponse>(
                await tokenResponse.Content.ReadAsStringAsync());

            if (errorResponse != null)
            {
                _logger.LogError("Failed to get token: {Message}", errorResponse.Message);
            }
        }

        return await JsonSerializer.DeserializeAsync<UbisoftToken>(await tokenResponse.Content.ReadAsStreamAsync());
    }


    public async Task<UbisoftProfileResponse?> GetProfilesAsync(UbisoftToken token, string name)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get,
            $"https://public-ubiservices.ubi.com/v3/profiles?nameOnPlatform={name}&platformType=uplay");

        request.Headers.Add("Authorization", $"ubi_v1 t={token.Ticket}");

        var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = JsonSerializer.Deserialize<UbisoftErrorResponse>(
                await response.Content.ReadAsStringAsync());

            if (errorResponse != null)
            {
                _logger.LogError("Failed to get profiles: {Message}", errorResponse.Message);
            }
        }

        return await JsonSerializer.DeserializeAsync<UbisoftProfileResponse>(await response.Content.ReadAsStreamAsync());
    }

    public async Task<UbisoftAvatar?> GetAvatarAsync(UbisoftToken token, Guid profileId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get,
            $"https://public-ubiservices.ubi.com/v1/profiles/{profileId}/avatars");

        request.Headers.Add("Authorization", $"ubi_v1 t={token.Ticket}");

        var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = JsonSerializer.Deserialize<UbisoftErrorResponse>(
                await response.Content.ReadAsStringAsync());

            if (errorResponse != null)
            {
                _logger.LogError("Failed to get avatar: {Message}", errorResponse.Message);
            }
        }

        return await JsonSerializer.DeserializeAsync<UbisoftAvatar>(await response.Content.ReadAsStreamAsync());
    }
    
    public async Task<UbisoftAvatar?> ChangeAvatarAsync(UbisoftToken token, Guid avatarId)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post,
            $"https://public-ubiservices.ubi.com/v1/profiles/{token.UserId}/avatars/galleries");

        request.Headers.Add("Authorization", $"ubi_v1 t={token.Ticket}");
        request.Content = new StringContent($"{{\n\t\"galleryAvatarId\": \"{avatarId}\"\n}}", Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorResponse = JsonSerializer.Deserialize<UbisoftErrorResponse>(
                await response.Content.ReadAsStringAsync());

            if (errorResponse != null)
            {
                _logger.LogError("Failed to change avatar: {Message}", errorResponse.Message);
            }
        }

        return await JsonSerializer.DeserializeAsync<UbisoftAvatar>(await response.Content.ReadAsStreamAsync());
    }
}