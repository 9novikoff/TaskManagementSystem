﻿using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.DAL.Entities;

namespace TaskManagementSystem.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TaskDbContext _context;

    public UserRepository(TaskDbContext context)
    {
        _context = context;
    }

    public IQueryable<User> GetUsers()
    {
        return _context.Users.AsNoTracking();
    }

    public async Task<User> InsertUser(User user)
    {
        _context.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task DeleteUser(User user)
    {
        _context.Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User> UpdateUser(User user)
    {
        _context.Update(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> GetUserById(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }
}