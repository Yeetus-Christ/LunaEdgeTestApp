using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using LunaEdgeTestApp.Enums;

namespace LunaEdgeTestApp.Models
{
    public class Task
    {
        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; }

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        [Required]
        public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.Pending;

        [Required]
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public User? User { get; set; }
    }
}
