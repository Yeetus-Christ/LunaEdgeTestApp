namespace LunaEdgeTestApp.Repositories
{
    public interface ITasksRepository
    {
        IQueryable<Models.Task> GetTasks(Guid userId);
        Models.Task? GetTaskById(Guid taskId, Guid userId);
        void AddTask(Models.Task task);
        void UpdateTask(Models.Task task);
        void DeleteTask(Models.Task task);
    }
}
