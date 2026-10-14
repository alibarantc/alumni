using Microsoft.EntityFrameworkCore;
using Konekto.DataAccess.Context;
using Konekto.Domain.Entities;
using Konekto.Domain.Interfaces;

namespace Konekto.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly KonektoDbContext _context;

    public UserRepository(KonektoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User> AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> UpdateAsync(User user)
    {
        var existing = await _context.Users.FindAsync(user.Id);
        if (existing == null) return false;

        existing.FirstName = user.FirstName;
        existing.LastName = user.LastName;
        existing.Email = user.Email;
        existing.Role = user.Role;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Users.FindAsync(id);
        if (existing == null) return false;

        _context.Users.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
