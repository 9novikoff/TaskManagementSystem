using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using TaskManagementSystem.BLL;
using TaskManagementSystem.BLL.Services;
using TaskManagementSystem.BLL.Validators;
using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DAL.Repositories;
using TaskManagementSystem.DTO;

namespace TaskManagementSystem.Tests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<JwtGenerator> _jwtGeneratorMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        Mock<ILogger<UserService>> loggerMock = new();
        _jwtGeneratorMock = new Mock<JwtGenerator>(null);
        
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<RegisterDto, User>();
            cfg.CreateMap<User, UserDto>();
        });
        var mapper = config.CreateMapper();

        _userService = new UserService(
            _userRepositoryMock.Object,
            new RegisterDtoValidator(),
            new LoginDtoValidator(),
            _jwtGeneratorMock.Object,
            loggerMock.Object,
            mapper
        );
    }

    [Fact]
    public async Task RegisterUser_ValidationFails_ReturnsRegistrationFailed()
    {
        var registerDto = new RegisterDto { Username = "", Email = "", Password = "" };

        var result = await _userService.RegisterUser(registerDto);

        Assert.Equal("'Username' must not be empty. The length of 'Username' must be at least 3 characters. " +
                     "You entered 0 characters. 'Email' must not be empty. 'Email' is not a valid email address. " +
                     "'Password' must not be empty. The length of 'Password' must be at least 3 characters. " +
                     "You entered 0 characters. Password must contain at least one uppercase letter. Password must " +
                     "contain at least one lowercase letter. Password must contain at least one number. Password must " +
                     "contain at least one special character.", 
            result.Match<string>(s => "", f => f.ErrorMessage));
    }

    [Fact]
    public async Task RegisterUser_UsernameAlreadyExists_ReturnsRegistrationFailed()
    {
        var registerDto = new RegisterDto { Username = "TestUser", Email = "test@test.com", Password = "Password123_" };
        _userRepositoryMock.Setup(r => r.GetUsers())
            .Returns(new List<User> { new User { Username = "TestUser" } }.AsQueryable().BuildMock());

        var result = await _userService.RegisterUser(registerDto);

        Assert.Equal("Username already exists", result.Match<string>(s => "", f => f.ErrorMessage));
    }

    [Fact]
    public async Task RegisterUser_Success_ReturnsUserDto()
    {
        var registerDto = new RegisterDto { Username = "NewUser", Email = "new@user.com", Password = "Password123_" };
        _userRepositoryMock.Setup(r => r.GetUsers()).Returns(new List<User>().AsQueryable().BuildMock());
        _userRepositoryMock.Setup(r => r.InsertUser(It.IsAny<User>())).Returns(Task.FromResult(new User{ Username = "NewUser", Email = "new@user.com"}));

        var result = await _userService.RegisterUser(registerDto);

        var userDto = result.Match<UserDto?>(s => s, f => null);

        Assert.NotNull(userDto);
        Assert.Equal(registerDto.Username, userDto.Username);
    }

    [Fact]
    public async Task LoginUser_ValidationFails_ReturnsLoginFailed()
    {
        var loginDto = new LoginDto { UsernameOrEmail = "", Password = "" };

        var result = await _userService.LoginUser(loginDto);

        Assert.Equal("'Username Or Email' must not be empty. The length of 'Username Or Email' must be at " +
                     "least 3 characters. You entered 0 characters. 'Password' must not be empty. The length of " +
                     "'Password' must be at least 3 characters. You entered 0 characters. Password must contain at least " +
                     "one uppercase letter. Password must contain at least one lowercase letter. Password must contain at least one" +
                     " number. Password must contain at least one special character.",
            result.Match<string>(s => s, f => f.ErrorMessage));
    }

    [Fact]
    public async Task LoginUser_UserNotFound_ReturnsLoginFailed()
    {
        var loginDto = new LoginDto { UsernameOrEmail = "notfound@test.com", Password = "Password123_" };
        _userRepositoryMock.Setup(r => r.GetUsers()).Returns(new List<User>().AsQueryable().BuildMock());

        var result = await _userService.LoginUser(loginDto);

        Assert.Equal("No user with that username or email", result.Match<string>(s => s, f => f.ErrorMessage));
    }

    [Fact]
    public async Task LoginUser_InvalidPassword_ReturnsLoginFailed()
    {
        var loginDto = new LoginDto { UsernameOrEmail = "test@test.com", Password = "Wrongpassword123_" };
        var user = new User { Email = "test@test.com", PasswordHash = BcryptPasswordHasher.HashPassword("Password123_") };
        _userRepositoryMock.Setup(r => r.GetUsers()).Returns(new List<User> { user }.AsQueryable().BuildMock());

        var result = await _userService.LoginUser(loginDto);

        Assert.Equal("Invalid password", result.Match<string>(s => s, f => f.ErrorMessage));
    }
    
}