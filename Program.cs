using System.Text.RegularExpressions;
using AvatarStealer;
using AvatarStealer.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var serviceCollection = new ServiceCollection();
ConfigureServices(serviceCollection);

var serviceProvider = serviceCollection.BuildServiceProvider();

Console.Title = "Ubisoft Avatar Stealer - by @Digneety and @KyoDaHotDog";
Console.Clear();

using var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger<Program>();

logger.LogInformation("Welcome to Ubisoft Avatar Stealer by @Digneety and @KyoDaHotDog");

logger.LogInformation("Enter your ubisoft email:");
var email = Console.ReadLine();

if (string.IsNullOrWhiteSpace(email))
{
    logger.LogError("Email cannot be empty.");
    return;
}

logger.LogInformation("Enter your ubisoft password:");
var password = Console.ReadLine();

if (string.IsNullOrWhiteSpace(password))
{
    logger.LogError("Password cannot be empty.");
    return;
}

var ubisoftClient = serviceProvider.GetRequiredService<IUbisoftClient>();

var token = await ubisoftClient.GetTokenAsync(email, password);

if (token == null)
{
    logger.LogError("Failed to get authentication token.");
    return;
}

logger.LogInformation("Logged in as {0}", token.NameOnPlatform);

logger.LogInformation("Enter the name of the profile you want to steal the avatar from:");
var name = Console.ReadLine();

if (string.IsNullOrWhiteSpace(name))
{
    logger.LogError("Name cannot be empty.");
    return;
}

var profiles = await ubisoftClient.GetProfilesAsync(token, name);

if (profiles is { Profiles: null } or null)
{
    logger.LogError("Failed to get profile.");
    return;
}

var profile = profiles.Profiles.FirstOrDefault();

if (profile is null)
{
    logger.LogError("Failed to get profile {0}.", name);
    return;
}

logger.LogInformation("Requesting {0}'s avatar", profile.NameOnPlatform);

var avatar = await ubisoftClient.GetAvatarAsync(token, profile.ProfileId);

if (avatar == null)
{
    logger.LogError("Failed to get avatar.");
    return;
}

if (avatar.ImageVariations is null || !avatar.ImageVariations.Any())
{
    logger.LogError("Failed to get avatar image variations. Aborting.");
    return;
}

var avatarUrl = avatar.ImageVariations.Last().Url;

if (string.IsNullOrWhiteSpace(avatarUrl))
{
    logger.LogError("Failed to get avatar URL.");
    return;
}

var avatarRegex = new Regex(@"https:\/\/ubiservices\.cdn\.ubi\.com\/[a-f0-9\-]+\/upload\/([a-f0-9_]+)\.png\?rs=500");

var avatarMatch = avatarRegex.Match(avatarUrl);

if (!avatarMatch.Success)
{
    logger.LogError("Avatar URL does not match the expected pattern. {0}", avatarUrl);
    return;
}

logger.LogDebug("Parsing avatar ID from URL");

var avatarString = avatarMatch.Groups[1].Value.Replace("_", "-");

var avatarId = Guid.Parse(avatarString);

logger.LogInformation("Changing avatar to {0}'s avatar", profile.NameOnPlatform);

var changedAvatar = await ubisoftClient.ChangeAvatarAsync(token, avatarId);

if (changedAvatar == null)
{
    logger.LogError("Failed to change avatar.");
    return;
}

logger.LogInformation("Successfully changed avatar to {0}'s avatar", profile.NameOnPlatform);

return;

void ConfigureServices(IServiceCollection services)
{
    services.AddLogging(builder =>
    {
        builder
            .AddFilter("Microsoft", LogLevel.Warning)
            .AddFilter("System", LogLevel.Warning)
            .AddFilter("AvatarStealer.Program", LogLevel.Information)
            .AddFilter("AvatarStealer.UbisoftClient", LogLevel.Warning)
            .AddConsole();
    });

    services.AddSingleton<IUbisoftClient, UbisoftClient>();
}