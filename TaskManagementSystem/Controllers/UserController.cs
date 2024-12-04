using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.BLL.Services;
using TaskManagementSystem.DTO;

namespace TaskManagementSystem.Controllers;

[Route("users")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(RegisterDto registerDto)
    {
        var result = await _service.RegisterUser(registerDto);
        return result.Match<IActionResult>(u => Ok(), f => StatusCode(403, f.ErrorMessage));
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> LoginUser(LoginDto loginDto)
    {
        var result = await _service.LoginUser(loginDto);
        return result.Match<IActionResult>(token => Ok(token), f => StatusCode(403, f.ErrorMessage));
    }
    
    
}