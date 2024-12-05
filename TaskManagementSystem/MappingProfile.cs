using AutoMapper;
using TaskManagementSystem.DAL.Entities;
using TaskManagementSystem.DTO;

namespace TaskManagementSystem;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterDto, User>();
        CreateMap<UserTaskDto, UserTask>();
        CreateMap<UserTask, UserTaskDto>();
        CreateMap<User, UserDto>();
    }
    
}