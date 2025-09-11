using Microsoft.AspNetCore.Mvc;
using WorkflowApi.Models;
using WorkflowApi.Services;
using System.Security.Claims;

namespace WorkflowApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkflowController : ControllerBase
    {
        private readonly IWorkflowService _workflowService;
        private readonly ILogger<WorkflowController> _logger;
        
        public WorkflowController(IWorkflowService workflowService, ILogger<WorkflowController> logger)
        {
            _workflowService = workflowService;
            _logger = logger;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Workflow>>> GetWorkflows()
        {
            try
            {
                var userId = GetCurrentUserId();
                var workflows = await _workflowService.GetWorkflowsAsync(userId);
                return Ok(workflows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflows");
                return StatusCode(500, "Internal server error");
            }
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<Workflow>> GetWorkflow(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var workflow = await _workflowService.GetWorkflowAsync(id, userId);
                
                if (workflow == null)
                {
                    return NotFound();
                }
                
                return Ok(workflow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow {WorkflowId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
        
        [HttpPost]
        public async Task<ActionResult<Workflow>> CreateWorkflow([FromBody] CreateWorkflowRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                var workflow = new Workflow
                {
                    Name = request.Name,
                    Description = request.Description,
                    Definition = request.Definition,
                    UserId = userId
                };
                
                var createdWorkflow = await _workflowService.CreateWorkflowAsync(workflow);
                
                return CreatedAtAction(nameof(GetWorkflow), 
                    new { id = createdWorkflow.Id }, createdWorkflow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating workflow");
                return StatusCode(500, "Internal server error");
            }
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<Workflow>> UpdateWorkflow(int id, [FromBody] UpdateWorkflowRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                
                var workflow = new Workflow
                {
                    Name = request.Name,
                    Description = request.Description,
                    Definition = request.Definition
                };
                
                var updatedWorkflow = await _workflowService.UpdateWorkflowAsync(id, workflow, userId);
                
                if (updatedWorkflow == null)
                {
                    return NotFound();
                }
                
                return Ok(updatedWorkflow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating workflow {WorkflowId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
        
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteWorkflow(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var success = await _workflowService.DeleteWorkflowAsync(id, userId);
                
                if (!success)
                {
                    return NotFound();
                }
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting workflow {WorkflowId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
        
        [HttpPost("{id}/execute")]
        public async Task<ActionResult<WorkflowExecution>> ExecuteWorkflow(int id, [FromBody] ExecuteWorkflowRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var execution = await _workflowService.ExecuteWorkflowAsync(id, request.Input, userId);
                
                return Ok(execution);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing workflow {WorkflowId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
        
        [HttpGet("{id}/executions")]
        public async Task<ActionResult<IEnumerable<WorkflowExecution>>> GetExecutionHistory(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var executions = await _workflowService.GetExecutionHistoryAsync(id, userId);
                
                return Ok(executions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving execution history for workflow {WorkflowId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
        
        private int GetCurrentUserId()
        {
            // In a real application, this would extract the user ID from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            
            // For demo purposes, return a default user ID
            return 1;
        }
    }
    
    public class CreateWorkflowRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Definition { get; set; } = "{}";
    }
    
    public class UpdateWorkflowRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Definition { get; set; } = "{}";
    }
    
    public class ExecuteWorkflowRequest
    {
        public string Input { get; set; } = "{}";
    }
}