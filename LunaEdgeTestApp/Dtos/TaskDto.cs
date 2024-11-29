using LunaEdgeTestApp.Enums;

namespace LunaEdgeTestApp.Dtos
{
    public class TaskDto
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? DueDate { get; set; }

        public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.Pending;

        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    }
}
