using DTC.API.DTO;
using DTC.API.ErrorHandlers;
using DTC.Domain.Entities.Identity;
using DTC.Domain.Interfaces;
using DTC.Infrastructure.Data;
using DTC.Infrastructure.Services;
using DTC.Tests.Fake;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DTC.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<FakeUserManager> _userManagerMock;
        private readonly Mock<FakeSignInManager> _signInManagerMock;
        private readonly Mock<ITokenService> _tokenServiceMock = new();
        private readonly Mock<IConfiguration> _configMock = new();
        private readonly ApplicationDataBaseContext _context;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            // Инициализация UserManager и SignInManager через мок-объекты
            var userStore = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<FakeUserManager>(userStore.Object,
                null, null, null, null, null, null, null, null);

            _signInManagerMock = new Mock<FakeSignInManager>(
                _userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                null, null, null, null);

            // Подключаем InMemory базу
            var options = new DbContextOptionsBuilder<ApplicationDataBaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDataBaseContext(options);

            _authService = new AuthService(
                _context,
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _tokenServiceMock.Object,
                _configMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_ReturnsUserDto()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                Username = "testuser",
                Password = "Test123$"
            };

            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<User>(), "Member"))
                            .ReturnsAsync(IdentityResult.Success);

            _tokenServiceMock.Setup(t => t.GenerateJwtToken(It.IsAny<User>()))
                             .Returns("fake-jwt");

            // Act
            var result = await _authService.RegisterAsync(registerDto);

            // Assert
            result.Should().NotBeNull();
            result.Username.Should().Be("testuser");
            result.Email.Should().Be("test@mail.com");
            result.Token.Should().Be("fake-jwt");
        }

        [Fact]
        public async Task RegisterAsync_WhenCreateFails_ThrowsHttpException()
        {
            // Arrange
            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                            .ReturnsAsync(IdentityResult.Failed());

            var registerDto = new RegisterDTO
            {
                Username = "fail",
                Password = "failpass"
            };

            // Act
            Func<Task> act = async () => await _authService.RegisterAsync(registerDto);

            // Assert
            await act.Should().ThrowAsync<HttpExeption>()
                .Where(e => e.StatusCode == 422);
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ReturnsUserDto()
        {
            // Arrange
            var loginDto = new LoginDTO { Email = "test@mail.com", Password = "Test123$" };

            var user = new User { Id = 1, UserName = "testuser", Email = "test@mail.com" };

            _userManagerMock.Setup(u => u.FindByEmailAsync(loginDto.Email))
                            .ReturnsAsync(user);

            _signInManagerMock.Setup(s => s.CheckPasswordSignInAsync(user, loginDto.Password, false))
                              .ReturnsAsync(SignInResult.Success);

            _tokenServiceMock.Setup(t => t.GenerateJwtToken(user))
                             .Returns("jwt-token");

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            result.Username.Should().Be("testuser");
            result.Email.Should().Be("test@mail.com");
            result.Token.Should().Be("jwt-token");
        }

        [Fact]
        public async Task LoginAsync_WhenUserNotFound_ThrowsUnauthorized()
        {
            // Arrange
            _userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>()))
                            .ReturnsAsync((User)null);

            // Act
            Func<Task> act = async () => await _authService.LoginAsync(new LoginDTO
            {
                Email = "nouser@mail.com",
                Password = "wrong"
            });

            // Assert
            await act.Should().ThrowAsync<HttpExeption>()
                .Where(e => e.StatusCode == StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public async Task GenerateRefreshToken_CreatesTokenInDb()
        {
            // Arrange
            var user = new User { Id = 99 };

            // Act
            var token = await _authService.GenerateRefreshToken(user);

            // Assert
            token.Should().NotBeNull();
            token.UserId.Should().Be(99);
            token.Token.Should().NotBeNullOrWhiteSpace();

            var dbToken = await _context.RefreshTokens.FindAsync(token.Id);
            dbToken.Should().NotBeNull();
        }
    }

}
