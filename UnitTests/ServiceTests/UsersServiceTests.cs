using LunaEdgeTestApp.Models;
using LunaEdgeTestApp.Repositories;
using LunaEdgeTestApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.ServiceTests
{
    public class UsersServiceTests
    {
        private Mock<IUsersRepository> GetUsersRepositoryMock()
        {
            var mock = new Mock<IUsersRepository>();
            mock.Setup(repo => repo.GetUserByUsernameOrEmail(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new User { Id = Guid.NewGuid(), Username = "TestUser", PasswordHash = BCrypt.Net.BCrypt.HashPassword("TestPassword123!") });
            return mock;
        }

        private Mock<IConfiguration> GetConfigurationMock()
        {
            var mock = new Mock<IConfiguration>();
            mock.Setup(config => config["Jwt:Key"]).Returns("dfrbbgtrbgtrbgtrbgrbgtrbgftrdbgf");
            mock.Setup(config => config["Jwt:Issuer"]).Returns("testIssuer");
            mock.Setup(config => config["Jwt:Audience"]).Returns("testAudience");
            return mock;
        }

        [Fact]
        public void Authenticate_ShouldReturnUserIfValid()
        {
            // Arrange
            var userRepoMock = GetUsersRepositoryMock();
            var configMock = GetConfigurationMock();
            var loggerMock = new Mock<ILogger<UsersService>>();
            var service = new UsersService(configMock.Object, userRepoMock.Object, loggerMock.Object);

            // Act
            var result = service.Authenticate("TestUser", null, "TestPassword123!");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TestUser", result.Username);
        }

        [Fact]
        public void Register_ShouldThrowExceptionIfPasswordInvalid()
        {
            // Arrange
            var userRepoMock = GetUsersRepositoryMock();
            var configMock = GetConfigurationMock();
            var loggerMock = new Mock<ILogger<UsersService>>();
            var service = new UsersService(configMock.Object, userRepoMock.Object, loggerMock.Object);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => service.Register("NewUser", "newuser@example.com", "short"));
        }

        [Fact]
        public void Register_ShouldAddUserToRepo()
        {
            // Arrange
            var userRepoMock = GetUsersRepositoryMock();
            var configMock = GetConfigurationMock();
            var loggerMock = new Mock<ILogger<UsersService>>();
            var service = new UsersService(configMock.Object, userRepoMock.Object, loggerMock.Object);

            // Act
            service.Register("NewUser", "newuser@example.com", "ValidPassword123!");

            // Assert
            userRepoMock.Verify(repo => repo.AddUser(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public void GenerateJwtToken_ShouldReturnToken()
        {
            // Arrange
            var userRepoMock = GetUsersRepositoryMock();
            var configMock = GetConfigurationMock();
            var loggerMock = new Mock<ILogger<UsersService>>();
            var service = new UsersService(configMock.Object, userRepoMock.Object, loggerMock.Object);
            var user = new User { Id = Guid.NewGuid(), Username = "TestUser" };

            // Act
            var token = service.GenerateJwtToken(user);

            // Assert
            Assert.NotNull(token);
        }
    }
}
