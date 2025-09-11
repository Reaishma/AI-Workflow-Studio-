using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowApi.Models
{
    public class WorkflowExecution
    {
        public int Id { get; set; }
        
        public int WorkflowId { get; set; }
        
        public ExecutionStatus Status { get; set; } = ExecutionStatus.Pending;
        
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? CompletedAt { get; set; }
        
        public string Input { get; set; } = "{}"; // JSON input data
        
        public string Output { get; set; } = "{}"; // JSON output data
        
        public string? ErrorMessage { get; set; }
        
        public string ExecutionLog { get; set; } = "[]"; // JSON array of log entries
        
        public int ExecutionTimeMs { get; set; }
        
        // Navigation properties
        [ForeignKey("WorkflowId")]
        public Workflow Workflow { get; set; } = null!;
    }
    
    public enum ExecutionStatus
    {
        Pending = 0,
        Running = 1,
        Completed = 2,
        Failed = 3,
        Cancelled = 4
    }
}