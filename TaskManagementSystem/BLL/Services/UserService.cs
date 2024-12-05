using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.BLL.Errors;
using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DAL.Repositories;
using TaskManagementSystem.DTO;

namespace TaskManagementSystem.BLL.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IValidator<LoginDto> _loginValidator;
    private readonly IValidator<RegisterDto> _registerValidator;
    private readonly JwtGenerator _jwtGenerator;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public UserService(IUserRepository repository, IValidator<RegisterDto> registerValidator, IValidator<LoginDto> loginValidator, JwtGenerator jwtGenerator, ILogger<UserService> logger, IMapper mapper)
    {
        _repository = repository;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _jwtGenerator = jwtGenerator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<ServiceResult<UserDto, RegistrationFailed>> RegisterUser(RegisterDto registerDto)
    {
        var validationResult = await _registerValidator.ValidateAsync(registerDto);

        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Failed registration attempt with username {username} and email {email}: Validation failed", registerDto.Username, registerDto.Email);
            return new RegistrationFailed(string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        if(_repository.GetUsers().Any(u => u.Username == registerDto.Username))
        {
            _logger.LogInformation("Failed registration attempt with username {username} and email {email}: Username already exists", registerDto.Username, registerDto.Email);
            return new RegistrationFailed("Username already exists");
        }
        
        if(_repository.GetUsers().Any(u => u.Email == registerDto.Email))
        {
            _logger.LogInformation("Failed registration attempt with username {username} and email {email}: User with that email already exists", registerDto.Username, registerDto.Email);
            return new RegistrationFailed("User with that email already exists");
        }

        var user = _mapper.Map<User>(registerDto, opt => 
            opt.AfterMap((_, dest) => dest.PasswordHash = BcryptPasswordHasher.HashPassword(registerDto.Password)));

        await _repository.InsertUser(user);
        _logger.LogInformation("Successful registration attempt with username {username} and email {email}", registerDto.Username, registerDto.Email);

        return _mapper.Map<UserDto>(user);
    }

    public async Task<ServiceResult<string, LoginFailed>> LoginUser(LoginDto loginDto)
    {
        var validationResult = await _loginValidator.ValidateAsync(loginDto);
        
        if (!validationResult.IsValid)
        {
            _logger.LogInformation("Failed login attempt with username or email {username}: Validation failed", loginDto.UsernameOrEmail);
            return new LoginFailed(string.Join(" ", validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var user = await _repository.GetUsers()
            .SingleOrDefaultAsync(u => u.Email == loginDto.UsernameOrEmail || u.Username == loginDto.UsernameOrEmail);

        if (user == null)
        {
            _logger.LogInformation("Failed login attempt with username or email {username}: No user with that username or email", loginDto.UsernameOrEmail);
            return new LoginFailed("No user with that username or email");
        }

        if (!BcryptPasswordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            _logger.LogInformation("Failed login attempt with username or email {username}: Invalid password", loginDto.UsernameOrEmail);
            return new LoginFailed("Invalid password");
        }

        _logger.LogInformation("Successful login attempt with username or email {username}", loginDto.UsernameOrEmail);
        return _jwtGenerator.GenerateToken(user);
    }
}