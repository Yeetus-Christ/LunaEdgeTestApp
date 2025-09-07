using LunaEdgeTestApp.Dtos;
using LunaEdgeTestApp.Enums;

namespace LunaEdgeTestApp.Services
{
    public interface ITasksService
    {
        public void CreateTask(TaskDto taskDto, Guid userId);
        public IEnumerable<Models.Task> GetAllTasks(
            Guid userId,
            Enums.TaskStatus? status,
            DateTime? dueDate,
            TaskPriority? priority,
            string? sortBy,
            int page,
            int pageSize);
        public Models.Task GetTaskById(Guid taskId, Guid userId);
        public void UpdateTask(TaskDto task, Guid userId, Guid taskId);
        public void DeleteTask(Guid taskId, Guid userId);
    }
}
