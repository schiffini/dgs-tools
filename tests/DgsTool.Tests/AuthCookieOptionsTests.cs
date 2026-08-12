using DgsTool.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace DgsTool.Tests;

public class AuthCookieOptionsTests
{
    [Fact]
    public void Configure_SetsThirtyMinuteSlidingExpiration()
    {
        var options = new CookieAuthenticationOptions();

        AuthCookieOptions.Configure(options);

        Assert.Equal(TimeSpan.FromMinutes(30), options.ExpireTimeSpan);
        Assert.True(options.SlidingExpiration);
    }
}
