using Microsoft.AspNetCore.Authentication.Cookies;

namespace DgsTool.Configuration;

public static class AuthCookieOptions
{
    public static void Configure(CookieAuthenticationOptions options)
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    }
}
