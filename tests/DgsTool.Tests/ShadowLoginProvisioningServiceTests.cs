using DgsTool.Models;
using DgsTool.Services;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace DgsTool.Tests;

public class ShadowLoginProvisioningServiceTests
{
    private static Mock<UserManager<IdentityUser>> CreateUserManagerMock()
    {
        var store = new Mock<IUserStore<IdentityUser>>();
        return new Mock<UserManager<IdentityUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    [Fact]
    public async Task GetOrCreateAsync_NewUser_CreatesAndAssignsRole()
    {
        var userManagerMock = CreateUserManagerMock();
        userManagerMock.Setup(m => m.FindByNameAsync("jdoe")).ReturnsAsync((IdentityUser?)null);
        userManagerMock.Setup(m => m.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);
        userManagerMock.Setup(m => m.IsInRoleAsync(It.IsAny<IdentityUser>(), "Admin")).ReturnsAsync(false);
        userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<IdentityUser>(), "Admin")).ReturnsAsync(IdentityResult.Success);

        var sut = new ShadowLoginProvisioningService(userManagerMock.Object);
        var externalUser = new AppUser { Id = 1, LoginName = "jdoe", Password = "x", Status = true, Type = 0 };

        var result = await sut.GetOrCreateAsync(externalUser, "Admin");

        Assert.NotNull(result);
        Assert.Equal("jdoe", result!.UserName);
        userManagerMock.Verify(m => m.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
        userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<IdentityUser>(), "Admin"), Times.Once);
    }

    [Fact]
    public async Task GetOrCreateAsync_ExistingUserAlreadyInRole_DoesNotRecreateOrDuplicateRole()
    {
        var existing = new IdentityUser { UserName = "jdoe" };
        var userManagerMock = CreateUserManagerMock();
        userManagerMock.Setup(m => m.FindByNameAsync("jdoe")).ReturnsAsync(existing);
        userManagerMock.Setup(m => m.IsInRoleAsync(existing, "Admin")).ReturnsAsync(true);

        var sut = new ShadowLoginProvisioningService(userManagerMock.Object);
        var externalUser = new AppUser { Id = 1, LoginName = "jdoe", Password = "x", Status = true, Type = 0 };

        var result = await sut.GetOrCreateAsync(externalUser, "Admin");

        Assert.Same(existing, result);
        userManagerMock.Verify(m => m.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
        userManagerMock.Verify(m => m.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetOrCreateAsync_CreationFails_ReturnsNull()
    {
        var userManagerMock = CreateUserManagerMock();
        userManagerMock.Setup(m => m.FindByNameAsync("jdoe")).ReturnsAsync((IdentityUser?)null);
        userManagerMock.Setup(m => m.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "boom" }));

        var sut = new ShadowLoginProvisioningService(userManagerMock.Object);
        var externalUser = new AppUser { Id = 1, LoginName = "jdoe", Password = "x", Status = true, Type = 0 };

        var result = await sut.GetOrCreateAsync(externalUser, "Admin");

        Assert.Null(result);
    }
}
