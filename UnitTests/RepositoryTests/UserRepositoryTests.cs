using LunaEdgeTestApp.Data;
using LunaEdgeTestApp.Models;
using LunaEdgeTestApp.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.RepositoryTests
{
    public class UserRepositoryTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Creates a unique in-memory DB for each test
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public void AddUser_ShouldAddUserToDb()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new UsersRepository(context);
            var user = new User { Id = Guid.NewGuid(), Email = "Email", Username = "Username", PasswordHash = "Hash" };

            // Act
            repository.AddUser(user);

            // Assert
            Assert.Contains(user, context.Users);
        }

        [Fact]
        public void GetUserByUsername_ShouldReturnCorrectUser()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new UsersRepository(context);
            var user = new User { Id = Guid.NewGuid(), Email = "Email", Username = "Username", PasswordHash = "Hash" };
            context.Users.Add(user);
            context.SaveChanges();

            // Act
            var result = repository.GetUserByUsernameOrEmail("Username", null);

            // Assert
            Assert.Equal(user, result);
        }

        [Fact]
        public void GetUserByEmail_ShouldReturnCorrectUser()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new UsersRepository(context);
            var user = new User { Id = Guid.NewGuid(), Email = "Email", Username = "Username", PasswordHash = "Hash" };
            context.Users.Add(user);
            context.SaveChanges();

            // Act
            var result = repository.GetUserByUsernameOrEmail(null, "Email");

            // Assert
            Assert.Equal(user, result);
        }

        [Fact]
        public void GetUserById_ShouldReturnCorrectUser()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new UsersRepository(context);
            var user = new User { Id = Guid.NewGuid(), Email = "Email", Username = "Username", PasswordHash = "Hash" };
            context.Users.Add(user);
            context.SaveChanges();

            // Act
            var result = repository.GetUserById(user.Id);

            // Assert
            Assert.Equal(user, result);
        }
    }
}
