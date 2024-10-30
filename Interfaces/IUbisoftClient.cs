using AvatarStealer.Models;

namespace AvatarStealer.Interfaces;

public interface IUbisoftClient
{
    Task<UbisoftToken?> GetTokenAsync(string email, string password);

    Task<UbisoftProfileResponse?> GetProfilesAsync(UbisoftToken token, string name);
    
    Task<UbisoftAvatar?> GetAvatarAsync(UbisoftToken token, Guid profileId);
    
    Task<UbisoftAvatar?> ChangeAvatarAsync(UbisoftToken token, Guid AvatarId);
}