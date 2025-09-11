using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowApi.Models
{
    public class Workflow
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        public string Definition { get; set; } = "{}"; // JSON representation of the workflow
        
        public int Version { get; set; } = 1;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Foreign Key
        public int UserId { get; set; }
        
        // Navigation properties
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
        
        public ICollection<WorkflowExecution> Executions { get; set; } = new List<WorkflowExecution>();
    }
}