using LunaEdgeTestApp.Data;
using LunaEdgeTestApp.Dtos;
using LunaEdgeTestApp.Enums;
using LunaEdgeTestApp.Models;
using LunaEdgeTestApp.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LunaEdgeTestApp.Services
{
    public class TasksService : ITasksService
    {
        private readonly ITasksRepository _tasksRepository;
        private readonly IUsersRepository _usersRepository;

        public TasksService(ITasksRepository repository, IUsersRepository usersRepository)
        {
            _tasksRepository = repository;
            _usersRepository = usersRepository;
        }

        public void CreateTask(TaskDto taskDto, Guid userId) 
        {
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

            _tasksRepository.AddTask(newTask);
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
            var tasks = _tasksRepository.GetTasks(userId);

            if (status != null)
                tasks = tasks.Where(t => t.Status == status);
            if (dueDate.HasValue)
                tasks = tasks.Where(t => t.DueDate!.Value.Date == dueDate.Value.Date);
            if (priority != null)
                tasks = tasks.Where(t => t.Priority == priority);

            tasks = sortBy?.ToLower() switch
            {
                "duedate" => tasks.OrderBy(t => t.DueDate),
                "priority" => tasks.OrderBy(t => t.Priority),
                _ => tasks
            };

            return tasks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public Models.Task GetTaskById(Guid taskId, Guid userId)
        {
            var task = _tasksRepository.GetTaskById(taskId, userId);

            if (task == null)
                throw new ArgumentNullException($"Task with id {taskId} doesn't exist");

            return task;
        }

        public void UpdateTask (TaskDto taskDto, Guid userId, Guid taskId)
        {
            var task = _tasksRepository.GetTaskById(taskId, userId);

            if (task == null)
                throw new ArgumentNullException($"Task with id {taskId} doesn't exist");

            task.Status = taskDto.Status;
            task.DueDate = taskDto.DueDate;
            task.Description = taskDto.Description;
            task.Title = taskDto.Title;
            task.Priority = taskDto.Priority;
            task.UpdatedAt = DateTime.Now;

            _tasksRepository.UpdateTask(task);
        }

        public void DeleteTask(Guid taskId, Guid userId) 
        {
            var task = _tasksRepository.GetTaskById(taskId, userId);

            if (task == null)
                throw new ArgumentNullException($"Task with id {taskId} doesn't exist");

            _tasksRepository.DeleteTask(task);
        }
    }
}
