using LunaEdgeTestApp.Data;
using LunaEdgeTestApp.Repositories;
using LunaEdgeTestApp.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;

namespace UnitTests.RepositoryTests
{
    public class TaskRepositoryTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Creates a unique in-memory DB for each test
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public void AddTask_ShouldAddTaskToDb()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new TasksRepository(context);
            var task = new LunaEdgeTestApp.Models.Task { Id = Guid.NewGuid(), Title = "Test Task" };

            // Act
            repository.AddTask(task);

            // Assert
            Assert.Contains(task, context.Tasks);
        }

        [Fact]
        public void DeleteTask_ShouldRemoveTaskFromDb()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new TasksRepository(context);
            var task = new LunaEdgeTestApp.Models.Task { Id = Guid.NewGuid(), Title = "Test Task" };
            context.Tasks.Add(task);
            context.SaveChanges();

            // Act
            repository.DeleteTask(task);

            // Assert
            Assert.DoesNotContain(task, context.Tasks);
        }

        [Fact]
        public void GetTaskById_ShouldReturnCorrectTask()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new TasksRepository(context);
            var user = new User { Id = Guid.NewGuid(), Email = "Email", Username = "Username", PasswordHash = "Hash" };
            var task = new LunaEdgeTestApp.Models.Task { Id = Guid.NewGuid(), User = user, Title = "Test Task" };
            context.Tasks.Add(task);
            context.SaveChanges();

            // Act
            var result = repository.GetTaskById(task.Id, user.Id);

            // Assert
            Assert.Equal(task, result);
        }

        [Fact]
        public void GetTasks_ShouldReturnAllTasksForUser()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new TasksRepository(context);
            var user = new User { Id = Guid.NewGuid(), Email = "Email", Username = "Username", PasswordHash = "Hash" };
            var task1 = new LunaEdgeTestApp.Models.Task { Id = Guid.NewGuid(), User = user, Title = "Task 1" };
            var task2 = new LunaEdgeTestApp.Models.Task { Id = Guid.NewGuid(), User = user, Title = "Task 2" };
            context.Tasks.AddRange(task1, task2);
            context.SaveChanges();

            // Act
            var tasks = repository.GetTasks(user.Id).ToList();

            // Assert
            Assert.Contains(task1, tasks);
            Assert.Contains(task2, tasks);
        }

        [Fact]
        public void UpdateTask_ShouldModifyTask()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new TasksRepository(context);
            var task = new LunaEdgeTestApp.Models.Task { Id = Guid.NewGuid(), Title = "Original Title" };
            context.Tasks.Add(task);
            context.SaveChanges();

            // Act
            task.Title = "Updated Title";
            repository.UpdateTask(task);

            // Assert
            var updatedTask = context.Tasks.First();
            Assert.Equal("Updated Title", updatedTask.Title);
        }
    }
}