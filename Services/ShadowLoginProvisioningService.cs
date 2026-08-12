using DgsTool.Models;
using Microsoft.AspNetCore.Identity;

namespace DgsTool.Services;

/// <summary>
/// Finds or creates the local IdentityUser that mirrors an externally-validated
/// dbo.USERS login, and ensures it carries the given role. The local password is
/// random and never surfaced — the only real credential check happens upstream,
/// against dbo.USERS.
/// </summary>
public class ShadowLoginProvisioningService
{
    private readonly UserManager<IdentityUser> _userManager;

    public ShadowLoginProvisioningService(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityUser?> GetOrCreateAsync(AppUser externalUser, string role)
    {
        var shadowUser = await _userManager.FindByNameAsync(externalUser.LoginName);
        if (shadowUser is null)
        {
            shadowUser = new IdentityUser { UserName = externalUser.LoginName };
            var randomLocalPassword = $"{Guid.NewGuid():N}{Guid.NewGuid():N}Aa1!";
            var createResult = await _userManager.CreateAsync(shadowUser, randomLocalPassword);
            if (!createResult.Succeeded)
            {
                return null;
            }
        }

        if (!await _userManager.IsInRoleAsync(shadowUser, role))
        {
            await _userManager.AddToRoleAsync(shadowUser, role);
        }

        return shadowUser;
    }
}
