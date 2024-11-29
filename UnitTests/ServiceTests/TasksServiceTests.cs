using LunaEdgeTestApp.Dtos;
using LunaEdgeTestApp.Enums;
using LunaEdgeTestApp.Models;
using LunaEdgeTestApp.Repositories;
using LunaEdgeTestApp.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.ServiceTests
{
    public class TasksServiceTests
    {
        private Mock<ITasksRepository> GetTasksRepositoryMock()
        {
            var mock = new Mock<ITasksRepository>();
            var user = new User { Id = Guid.NewGuid(), Email = "Email", Username = "Username", PasswordHash = "Hash" };
            mock.Setup(repo => repo.GetTaskById(Guid.Parse("EA5479CB-F35C-42FF-AA81-2BC1B385D7CD"), Guid.Parse("4F9D2DF7-555D-441F-BA66-66FA88A2E4F1")))
                .Returns<Guid, Guid>((taskId, userId) => new LunaEdgeTestApp.Models.Task { Id = taskId, User = user });
            mock.Setup(repo => repo.GetTasks(It.IsAny<Guid>()))
                .Returns(new List<LunaEdgeTestApp.Models.Task> { new LunaEdgeTestApp.Models.Task { Id = Guid.NewGuid(), User = user, Title = "Test Task" } }.AsQueryable());
            return mock;
        }

        private Mock<IUsersRepository> GetUsersRepositoryMock()
        {
            var mock = new Mock<IUsersRepository>();
            mock.Setup(repo => repo.GetUserById(It.IsAny<Guid>()))
                .Returns(new User { Id = Guid.NewGuid(), Email = "Email", Username = "Username", PasswordHash = "Hash" });
            return mock;
        }

        [Fact]
        public void CreateTask_ShouldAddTask()
        {
            // Arrange
            var tasksRepoMock = GetTasksRepositoryMock();
            var usersRepoMock = GetUsersRepositoryMock();
            var loggerMock = new Mock<ILogger<TasksService>>();
            var service = new TasksService(tasksRepoMock.Object, usersRepoMock.Object, loggerMock.Object);
            var taskDto = new TaskDto
            {
                Title = "New Task",
                Description = "Task Description",
                DueDate = DateTime.Now.AddDays(1),
                Status = LunaEdgeTestApp.Enums.TaskStatus.Pending,
                Priority = TaskPriority.Medium
            };
            var userId = Guid.NewGuid();

            // Act
            service.CreateTask(taskDto, userId);

            // Assert
            tasksRepoMock.Verify(repo => repo.AddTask(It.IsAny<LunaEdgeTestApp.Models.Task>()), Times.Once);
        }

        [Fact]
        public void GetAllTasks_ShouldReturnFilteredTasks()
        {
            // Arrange
            var tasksRepoMock = GetTasksRepositoryMock();
            var usersRepoMock = GetUsersRepositoryMock(); 
            var loggerMock = new Mock<ILogger<TasksService>>();
            var service = new TasksService(tasksRepoMock.Object, usersRepoMock.Object, loggerMock.Object);
            var userId = Guid.NewGuid();

            // Act
            var tasks = service.GetAllTasks(userId, LunaEdgeTestApp.Enums.TaskStatus.Pending, null, null, "duedate", 1, 10).ToList();

            // Assert
            Assert.NotEmpty(tasks);
        }

        [Fact]
        public void GetTaskById_ShouldThrowIfNotFound()
        {
            // Arrange
            var tasksRepoMock = GetTasksRepositoryMock();
            var usersRepoMock = GetUsersRepositoryMock();
            var loggerMock = new Mock<ILogger<TasksService>>();
            var service = new TasksService(tasksRepoMock.Object, usersRepoMock.Object, loggerMock.Object);
            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => service.GetTaskById(taskId, userId));
        }

        [Fact]
        public void GetTaskById_ShouldReturnIfFound()
        {
            // Arrange
            var tasksRepoMock = GetTasksRepositoryMock();
            var usersRepoMock = GetUsersRepositoryMock();
            var loggerMock = new Mock<ILogger<TasksService>>();
            var service = new TasksService(tasksRepoMock.Object, usersRepoMock.Object, loggerMock.Object);
            var userId = Guid.Parse("4F9D2DF7-555D-441F-BA66-66FA88A2E4F1");
            var taskId = Guid.Parse("EA5479CB-F35C-42FF-AA81-2BC1B385D7CD");

            var user = new User { Id = userId, Email = "Email", Username = "Username", PasswordHash = "Hash" };
            var task = new LunaEdgeTestApp.Models.Task { Id = taskId, User = user };

            // Act

            var result = service.GetTaskById(taskId, userId);

            // Assert

            Assert.Equal(task.Id, result.Id);
        }

        [Fact]
        public void UpdateTask_ShouldModifyTask()
        {
            // Arrange
            var tasksRepoMock = GetTasksRepositoryMock();
            var usersRepoMock = GetUsersRepositoryMock();
            var loggerMock = new Mock<ILogger<TasksService>>();
            var service = new TasksService(tasksRepoMock.Object, usersRepoMock.Object, loggerMock.Object);
            var taskDto = new TaskDto
            {
                Title = "Updated Task",
                Description = "Updated Description",
                DueDate = DateTime.Now.AddDays(2),
                Status = LunaEdgeTestApp.Enums.TaskStatus.InProgress,
                Priority = TaskPriority.High
            };
            var userId = Guid.Parse("4F9D2DF7-555D-441F-BA66-66FA88A2E4F1");
            var taskId = Guid.Parse("EA5479CB-F35C-42FF-AA81-2BC1B385D7CD");

            // Act
            service.UpdateTask(taskDto, userId, taskId);

            // Assert
            tasksRepoMock.Verify(repo => repo.UpdateTask(It.IsAny<LunaEdgeTestApp.Models.Task>()), Times.Once);
        }

        [Fact]
        public void DeleteTask_ShouldRemoveTask()
        {
            // Arrange
            var tasksRepoMock = GetTasksRepositoryMock();
            var usersRepoMock = GetUsersRepositoryMock();
            var loggerMock = new Mock<ILogger<TasksService>>();
            var service = new TasksService(tasksRepoMock.Object, usersRepoMock.Object, loggerMock.Object);
            var userId = Guid.Parse("4F9D2DF7-555D-441F-BA66-66FA88A2E4F1");
            var taskId = Guid.Parse("EA5479CB-F35C-42FF-AA81-2BC1B385D7CD");

            // Act
            service.DeleteTask(taskId, userId);

            // Assert
            tasksRepoMock.Verify(repo => repo.DeleteTask(It.IsAny<LunaEdgeTestApp.Models.Task>()), Times.Once);
        }
    }
}
