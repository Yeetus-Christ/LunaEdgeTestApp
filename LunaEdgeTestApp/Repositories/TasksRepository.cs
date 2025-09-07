using LunaEdgeTestApp.Data;
using LunaEdgeTestApp.Enums;

namespace LunaEdgeTestApp.Repositories
{
    public class TasksRepository : ITasksRepository
    {
        private readonly AppDbContext _context;

        public TasksRepository(AppDbContext context)
        {
            _context = context;
        }

        public void AddTask(Models.Task task)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }

        public void DeleteTask(Models.Task task)
        {
            _context.Tasks.Remove(task);
            _context.SaveChanges();
        }

        public Models.Task? GetTaskById(Guid taskId, Guid userId)
        {
            return _context.Tasks.FirstOrDefault(t => t.Id == taskId && t.User!.Id == userId);
        }

        public IQueryable<Models.Task> GetTasks(Guid userId)
        {
            return _context.Tasks.Where(t => t.User!.Id == userId);
        }

        public void UpdateTask(Models.Task task)
        {
            _context.Tasks.Update(task);
            _context.SaveChanges();
        }
    }
}
