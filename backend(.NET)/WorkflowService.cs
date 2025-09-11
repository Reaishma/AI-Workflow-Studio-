using Microsoft.EntityFrameworkCore;
using WorkflowApi.Data;
using WorkflowApi.Models;
using System.Text.Json;

namespace WorkflowApi.Services
{
    public interface IWorkflowService
    {
        Task<IEnumerable<Workflow>> GetWorkflowsAsync(int userId);
        Task<Workflow?> GetWorkflowAsync(int id, int userId);
        Task<Workflow> CreateWorkflowAsync(Workflow workflow);
        Task<Workflow?> UpdateWorkflowAsync(int id, Workflow workflow, int userId);
        Task<bool> DeleteWorkflowAsync(int id, int userId);
        Task<WorkflowExecution> ExecuteWorkflowAsync(int workflowId, string input, int userId);
        Task<IEnumerable<WorkflowExecution>> GetExecutionHistoryAsync(int workflowId, int userId);
    }
    
    public class WorkflowService : IWorkflowService
    {
        private readonly WorkflowDbContext _context;
        private readonly IAiServiceIntegration _aiService;
        private readonly ILogger<WorkflowService> _logger;
        
        public WorkflowService(WorkflowDbContext context, IAiServiceIntegration aiService, ILogger<WorkflowService> logger)
        {
            _context = context;
            _aiService = aiService;
            _logger = logger;
        }
        
        public async Task<IEnumerable<Workflow>> GetWorkflowsAsync(int userId)
        {
            return await _context.Workflows
                .Where(w => w.UserId == userId && w.IsActive)
                .OrderByDescending(w => w.UpdatedAt)
                .ToListAsync();
        }
        
        public async Task<Workflow?> GetWorkflowAsync(int id, int userId)
        {
            return await _context.Workflows
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
        }
        
        public async Task<Workflow> CreateWorkflowAsync(Workflow workflow)
        {
            workflow.CreatedAt = DateTime.UtcNow;
            workflow.UpdatedAt = DateTime.UtcNow;
            
            _context.Workflows.Add(workflow);
            await _context.SaveChangesAsync();
            
            return workflow;
        }
        
        public async Task<Workflow?> UpdateWorkflowAsync(int id, Workflow workflow, int userId)
        {
            var existingWorkflow = await _context.Workflows
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
            
            if (existingWorkflow == null)
                return null;
            
            existingWorkflow.Name = workflow.Name;
            existingWorkflow.Description = workflow.Description;
            existingWorkflow.Definition = workflow.Definition;
            existingWorkflow.Version += 1;
            existingWorkflow.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            return existingWorkflow;
        }
        
        public async Task<bool> DeleteWorkflowAsync(int id, int userId)
        {
            var workflow = await _context.Workflows
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
            
            if (workflow == null)
                return false;
            
            workflow.IsActive = false;
            await _context.SaveChangesAsync();
            
            return true;
        }
        
        public async Task<WorkflowExecution> ExecuteWorkflowAsync(int workflowId, string input, int userId)
        {
            var workflow = await _context.Workflows
                .FirstOrDefaultAsync(w => w.Id == workflowId && w.UserId == userId);
            
            if (workflow == null)
                throw new ArgumentException("Workflow not found");
            
            var execution = new WorkflowExecution
            {
                WorkflowId = workflowId,
                Input = input,
                Status = ExecutionStatus.Running,
                StartedAt = DateTime.UtcNow
            };
            
            _context.WorkflowExecutions.Add(execution);
            await _context.SaveChangesAsync();
            
            try
            {
                // Parse workflow definition and execute
                var workflowDef = JsonSerializer.Deserialize<dynamic>(workflow.Definition);
                var result = await ExecuteWorkflowNodes(workflowDef, input);
                
                execution.Status = ExecutionStatus.Completed;
                execution.Output = JsonSerializer.Serialize(result);
                execution.CompletedAt = DateTime.UtcNow;
                execution.ExecutionTimeMs = (int)(execution.CompletedAt.Value - execution.StartedAt).TotalMilliseconds;
            }
            catch (Exception ex)
            {
                execution.Status = ExecutionStatus.Failed;
                execution.ErrorMessage = ex.Message;
                execution.CompletedAt = DateTime.UtcNow;
                _logger.LogError(ex, "Workflow execution failed for workflow {WorkflowId}", workflowId);
            }
            
            await _context.SaveChangesAsync();
            return execution;
        }
        
        public async Task<IEnumerable<WorkflowExecution>> GetExecutionHistoryAsync(int workflowId, int userId)
        {
            var workflow = await _context.Workflows
                .FirstOrDefaultAsync(w => w.Id == workflowId && w.UserId == userId);
            
            if (workflow == null)
                return Enumerable.Empty<WorkflowExecution>();
            
            return await _context.WorkflowExecutions
                .Where(e => e.WorkflowId == workflowId)
                .OrderByDescending(e => e.StartedAt)
                .Take(50)
                .ToListAsync();
        }
        
        private async Task<object> ExecuteWorkflowNodes(dynamic workflowDef, string input)
        {
            // This is a simplified execution engine - in a real implementation,
            // you would parse the workflow graph and execute nodes in proper order
            
            // For demo purposes, we'll assume a simple linear execution
            var result = new { input = input, output = "Workflow executed successfully" };
            
            // Add AI processing if nodes contain AI components
            if (workflowDef.ToString().Contains("ai"))
            {
                var aiResult = await _aiService.ProcessTextAsync(input);
                result = new { input = input, output = aiResult };
            }
            
            return result;
        }
    }
}