using TaskManagementSystem.BLL.Errors;
using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DTO;

namespace TaskManagementSystem.BLL.Services;

public interface IUserService
{
    public Task<ServiceResult<User, RegistrationFailed>> RegisterUser(RegisterDto registerDto);
    public Task<ServiceResult<string, LoginFailed>> LoginUser(LoginDto loginDto);
}