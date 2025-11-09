using DTC.Application.DTO.Account;
using DTC.Application.DTO.Profile;
using DTC.Application.ErrorHandlers;
using DTC.Application.Interfaces.Services;
using DTC.Domain.Entities.Identity;
using DTC.Infrastructure.Data;
using DTC.Infrastructure.Repositories;
using DTC.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DTC.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDataBaseContext _dbContext;

        public AuthServiceTests()
        {
            _userManagerMock = MockUserManager();
            _signInManagerMock = new Mock<SignInManager<User>>(
                _userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<User>>(),
                null, null, null, null);

            _tokenServiceMock = new Mock<ITokenService>();
            _emailServiceMock = new Mock<IEmailService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            var inMemorySettings = new Dictionary<string, string>();
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var options = new DbContextOptionsBuilder<ApplicationDataBaseContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            _dbContext = new ApplicationDataBaseContext(options);
        }

        private AuthService CreateService() =>
            new AuthService(
                _dbContext,
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _tokenServiceMock.Object,
                _configuration,
                _httpContextAccessorMock.Object);

        // --- RegisterAsync ---
        [Fact]
        public async Task RegisterAsync_ShouldReturnUserDto_WhenSuccess()
        {
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            _tokenServiceMock.Setup(x => x.GenerateJwtToken(It.IsAny<User>()))
                .ReturnsAsync("jwt_token");
            _tokenServiceMock.Setup(x => x.GenerateRefreshToken(It.IsAny<User>()))
                .ReturnsAsync(new RefreshToken { Token = "refresh_token" });

            var service = CreateService();
            var dto = new RegisterDTO { Username = "user", Password = "Pass123!" };

            var result = await service.RegisterAsync(dto);

            Assert.Equal("jwt_token", result.AccessToken);
            Assert.Equal("refresh_token", result.RefreshToken);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrow_WhenCreateFails()
        {
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed());

            var service = CreateService();
            var dto = new RegisterDTO { Username = "user", Password = "fail" };

            await Assert.ThrowsAsync<HttpExeption>(() => service.RegisterAsync(dto));
        }

        // --- LoginAsync ---
        [Fact]
        public async Task LoginAsync_ShouldReturnUserDto_WhenValid()
        {
            var user = new User { UserName = "loginuser" };
            _userManagerMock.Setup(x => x.FindByNameAsync("loginuser"))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, "Pass123!"))
                .ReturnsAsync(true);

            _tokenServiceMock.Setup(x => x.GenerateJwtToken(user))
                .ReturnsAsync("jwt_token");
            _tokenServiceMock.Setup(x => x.GenerateRefreshToken(user))
                .ReturnsAsync(new RefreshToken { Token = "refresh_token" });

            var context = new DefaultHttpContext();
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

            var service = CreateService();
            var result = await service.LoginAsync(new LoginDTO
            {
                Username = "loginuser",
                Password = "Pass123!"
            });

            Assert.Equal("jwt_token", result.AccessToken);
            Assert.Equal("refresh_token", result.RefreshToken);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrow_WhenInvalid()
        {
            _userManagerMock.Setup(x => x.FindByNameAsync("nouser"))
                .ReturnsAsync((User)null);

            var service = CreateService();
            await Assert.ThrowsAsync<HttpExeption>(() =>
                service.LoginAsync(new LoginDTO { Username = "nouser", Password = "bad" }));
        }

        // --- RefreshTokenAsync ---
        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnNewAccessToken_WhenValid()
        {
            var user = new User { UserName = "refreshUser" };
            var oldToken = new RefreshToken
            {
                Token = "hashed",
                User = user,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };
            _dbContext.RefreshTokens.Add(oldToken);
            await _dbContext.SaveChangesAsync();

            _tokenServiceMock.Setup(x => x.HashToken("raw")).Returns("hashed");
            _tokenServiceMock.Setup(x => x.GenerateJwtToken(user))
                .ReturnsAsync("new_jwt");
            _tokenServiceMock.Setup(x => x.GenerateRefreshToken(user))
                .ReturnsAsync(new RefreshToken { Token = "new_refresh", ExpiresAt = DateTime.UtcNow.AddDays(1) });

            var context = new DefaultHttpContext();
            context.Request.Cookies = new RequestCookieCollection(new Dictionary<string, string>
            {
                { "refresh_token", "raw" }
            });
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

            var service = CreateService();
            var result = await service.RefreshTokenAsync();

            Assert.Equal("new_jwt", result.AccessToken);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldThrow_WhenMissingToken()
        {
            var context = new DefaultHttpContext();
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

            var service = CreateService();
            await Assert.ThrowsAsync<HttpExeption>(() => service.RefreshTokenAsync());
        }

        // --- LogoutAsync ---
        [Fact]
        public async Task LogoutAsync_ShouldRemoveToken()
        {
            var token = new RefreshToken
            {
                Token = "hashed",
                ExpiresAt = DateTime.UtcNow.AddMinutes(10)
            };
            _dbContext.RefreshTokens.Add(token);
            await _dbContext.SaveChangesAsync();

            _tokenServiceMock.Setup(x => x.HashToken("raw")).Returns("hashed");

            var context = new DefaultHttpContext();
            context.Request.Cookies = new RequestCookieCollection(new Dictionary<string, string>
            {
                { "refresh_token", "raw" }
            });
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

            var service = CreateService();
            await service.LogoutAsynс();

            Assert.Empty(_dbContext.RefreshTokens);
        }

        // --- RequestPasswordResetAsync ---
        [Fact]
        public async Task RequestPasswordResetAsync_ShouldSendEmail_WhenUserExists()
        {
            var user = new User { Email = "test@mail.com" };
            _userManagerMock.Setup(x => x.FindByEmailAsync("test@mail.com"))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.GeneratePasswordResetTokenAsync(user))
                .ReturnsAsync("reset_token");

            _emailServiceMock.Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var service = new AuthService(
                _dbContext,
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _tokenServiceMock.Object,
                _configuration,
                _httpContextAccessorMock.Object)
            {
            };

            await service.RequestPasswordResetAsync("test@mail.com");

            _emailServiceMock.Verify(x =>
                x.SendAsync("test@mail.com", It.IsAny<string>(), It.Is<string>(msg => msg.Contains("reset-password"))),
                Times.Once);
        }

        // --- ResetPasswordAsync ---
        [Fact]
        public async Task ResetPasswordAsync_ShouldSucceed_WhenValid()
        {
            var user = new User { Email = "reset@mail.com" };
            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.ResetPasswordAsync(user, "token", "NewPass123!"))
                .ReturnsAsync(IdentityResult.Success);

            var service = CreateService();
            await service.ResetPasswordAsync(new PasswordResetDTO
            {
                Email = "reset@mail.com",
                Token = "token",
                NewPassword = "NewPass123!"
            });
        }

        // --- ConfirmEmailAsync ---
        [Fact]
        public async Task ConfirmEmailAsync_ShouldThrow_WhenInvalid()
        {
            _userManagerMock.Setup(x => x.FindByIdAsync("bad")).ReturnsAsync((User)null);

            var service = CreateService();
            await Assert.ThrowsAsync<HttpExeption>(() => service.ConfirmEmailAsync("bad", "token"));
        }

        // --- GetUserProfileAsync ---
        [Fact]
        public async Task GetUserProfileAsync_ShouldReturnProfile()
        {
            var user = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@doe.com",
                CreatedAt = DateTime.UtcNow,
                Birthday = DateTime.UtcNow.AddYears(-30),
                Gender = "M",
                IsAuthor = true
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var service = CreateService();
            var profile = await service.GetUserProfileAsync(1);

            Assert.Equal("John", profile.FirstName);
            Assert.Equal("Doe", profile.LastName);
        }

        // --- UpdateProfileAsync ---
        [Fact]
        public async Task UpdateProfileAsync_ShouldUpdateFields()
        {
            var user = new User { Id = 2, FirstName = "Old", LastName = "Name" };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var service = CreateService();
            var updated = await service.UpdateProfileAsync(2, new UpdateProfileDto
            {
                FirstName = "New",
                LastName = "Surname",
                Birthday = DateTime.UtcNow.AddYears(-25),
                Gender = "F"
            });

            Assert.Equal("New", updated.FirstName);
            Assert.Equal("Surname", updated.LastName);
        }

        // Helper: Mock UserManager
        private static Mock<UserManager<User>> MockUserManager()
        {
            var store = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(store.Object,
                null, null, null, null, null, null, null, null);
        }
    }
}
