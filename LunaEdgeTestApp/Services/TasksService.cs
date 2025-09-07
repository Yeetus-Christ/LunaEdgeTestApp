using LunaEdgeTestApp.Data;
using LunaEdgeTestApp.Dtos;
using LunaEdgeTestApp.Enums;
using LunaEdgeTestApp.Models;
using LunaEdgeTestApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LunaEdgeTestApp.Services
{
    public class TasksService : ITasksService
    {
        private readonly ITasksRepository _tasksRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly ILogger<TasksService> _logger;

        public TasksService(ITasksRepository repository, IUsersRepository usersRepository, ILogger<TasksService> logger)
        {
            _tasksRepository = repository;
            _usersRepository = usersRepository;
            _logger = logger;
        }

        public void CreateTask(TaskDto taskDto, Guid userId) 
        {
            _logger.LogInformation("Creating task for user {UserId}", userId);

            var user = _usersRepository.GetUserById(userId);

            var newTask = new Models.Task
            {
                Id = Guid.NewGuid(),
                Title = taskDto.Title,
                Description = taskDto.Description,
                DueDate = taskDto.DueDate,
                Status = taskDto.Status,
                Priority = taskDto.Priority,
                User = user!,
            };

            try
            {
                _tasksRepository.AddTask(newTask);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating task for user {userId}");
                throw;
            }
            _logger.LogInformation($"Task with ID {newTask.Id} created successfully for user {userId}");

        }

        public IEnumerable<Models.Task> GetAllTasks(
            Guid userId,
            Enums.TaskStatus? status,
            DateTime? dueDate,
            TaskPriority? priority,
            string? sortBy,
            int page,
            int pageSize)
        {
            _logger.LogInformation($"Fetching tasks for user {userId}");

            var tasks = _tasksRepository.GetTasks(userId);

            // Filtering
            if (status != null)
                tasks = tasks.Where(t => t.Status == status);
            if (dueDate.HasValue)
                tasks = tasks.Where(t => t.DueDate!.Value.Date == dueDate.Value.Date);
            if (priority != null)
                tasks = tasks.Where(t => t.Priority == priority);

            // Sorting
            tasks = sortBy?.ToLower() switch
            {
                "duedate" => tasks.OrderBy(t => t.DueDate),
                "priority" => tasks.OrderBy(t => t.Priority),
                _ => tasks
            };

            // Pagination
            return tasks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public Models.Task GetTaskById(Guid taskId, Guid userId)
        {
            _logger.LogInformation($"Fetching task with ID {taskId} for user {userId}");

            var task = _tasksRepository.GetTaskById(taskId, userId);

            if (task == null)
            {
                _logger.LogWarning($"Task with ID {taskId} not found for user {userId}");
                throw new ArgumentNullException($"Task with id {taskId} doesn't exist");
            }

            _logger.LogInformation($"Task with ID {taskId} retrieved successfully for user {userId}");
            return task;
        }

        public void UpdateTask (TaskDto taskDto, Guid userId, Guid taskId)
        {
            _logger.LogInformation($"Attempting to update task with ID {taskId} for user {userId}");

            var task = _tasksRepository.GetTaskById(taskId, userId);

            if (task == null)
            {
                _logger.LogWarning($"Task with ID {taskId} not found for user {userId}");
                throw new ArgumentNullException($"Task with id {taskId} doesn't exist");
            }

            task.Status = taskDto.Status;
            task.DueDate = taskDto.DueDate;
            task.Description = taskDto.Description;
            task.Title = taskDto.Title;
            task.Priority = taskDto.Priority;
            task.UpdatedAt = DateTime.Now;

            try
            {
                _tasksRepository.UpdateTask(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating task with ID {taskId} for user {userId}");
                throw;
            }

            _logger.LogInformation($"Task with ID {taskId} updated successfully for user {userId}");
        }

        public void DeleteTask(Guid taskId, Guid userId) 
        {
            _logger.LogInformation($"Attempting to delete task with ID {taskId} for user {userId}");

            var task = _tasksRepository.GetTaskById(taskId, userId);

            if (task == null)
            {
                _logger.LogWarning($"Task with ID {taskId} not found for user {userId}");
                throw new ArgumentNullException($"Task with id {taskId} doesn't exist");
            }

            try
            {
                _tasksRepository.DeleteTask(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting task with ID {taskId} for user {userId}");
                throw;
            }

            _logger.LogInformation($"Task with ID {taskId} deleted successfully for user {userId}");
        }
    }
}
