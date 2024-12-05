using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using TaskManagementSystem.BLL.Services;
using TaskManagementSystem.DTO;

namespace TaskManagementSystem.Controllers;

[Authorize]
[Route("tasks")]
[ApiController]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }
    
    [HttpPost]
    public async Task<IActionResult> PostTask(CreateUserTaskDto userTaskDto)
    {
        var userId = GetUserIdFromJwtToken();

        var serviceResult = await _taskService.CreateUserTask(userId, userTaskDto);
        
        return serviceResult.Match<IActionResult>(t => Ok(t), f => BadRequest(f.ErrorMessage));
    }

    [HttpGet]
    public async Task<IActionResult> GetTasks([FromQuery]TaskFilter filter, [FromQuery]TaskSort sort, [FromQuery]TaskPagination pagination)
    {
        var userId = GetUserIdFromJwtToken();
    
        var serviceResult = await _taskService.GetAllUserTasks(userId, filter, sort, pagination);
    
        return serviceResult.Match<IActionResult>(t => Ok(t), f => BadRequest(f.ErrorMessage));
    }

    [HttpGet("{taskId:guid}")]
    public async Task<IActionResult> GetTaskById(Guid taskId)
    {
        var userId = GetUserIdFromJwtToken();

        var serviceResult = await _taskService.GetUserTaskById(userId, taskId);

        return serviceResult.Match<IActionResult>(t => Ok(t), f => BadRequest(f.ErrorMessage));
    }
    
    [HttpPut("{taskId:guid}")]
    public async Task<IActionResult> PutTask(Guid taskId, CreateUserTaskDto userTaskDto)
    {
        var userId = GetUserIdFromJwtToken();

        var serviceResult = await _taskService.UpdateUserTask(userId, taskId, userTaskDto);

        return serviceResult.Match<IActionResult>(t => Ok(t), f => BadRequest(f.ErrorMessage));
    }
    
    [HttpDelete("{taskId:guid}")]
    public async Task<IActionResult> DeleteTask(Guid taskId)
    {
        var userId = GetUserIdFromJwtToken();

        var serviceResult = await _taskService.DeleteUserTask(userId, taskId);

        return serviceResult.Match<IActionResult>(_ => Ok(), f => BadRequest(f.ErrorMessage));
    }
    
    
    private Guid GetUserIdFromJwtToken()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
        return Guid.Parse(userIdClaim.Value);
    }
}