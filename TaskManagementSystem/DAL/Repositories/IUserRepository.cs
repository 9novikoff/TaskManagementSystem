using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.DAL.Entities;

namespace TaskManagementSystem.DAL.Repositories;

public interface IUserRepository
{
    public IQueryable<User> GetUsers();
    public Task<User> InsertUser(User user);
    public Task DeleteUser(User user);
    public Task<User> UpdateUser(User user);
    public Task<User> GetUserById(Guid id);
}