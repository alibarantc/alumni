using Microsoft.EntityFrameworkCore;
using Konekto.DataAccess.Context;
using Konekto.Domain.Entities;
using Konekto.Domain.Interfaces;

namespace Konekto.DataAccess.Repositories;

public class AnnouncementRepository : IAnnouncementRepository
{
    private readonly KonektoDbContext _context;

    public AnnouncementRepository(KonektoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Announcement>> GetAllAsync()
    {
        return await _context.Announcements
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Announcement>> GetActiveAsync()
    {
        return await _context.Announcements
            .Where(a => a.IsActive)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<Announcement?> GetByIdAsync(int id)
    {
        return await _context.Announcements.FindAsync(id);
    }

    public async Task<Announcement> AddAsync(Announcement announcement)
    {
        _context.Announcements.Add(announcement);
        await _context.SaveChangesAsync();
        return announcement;
    }

    public async Task UpdateAsync(Announcement announcement)
    {
        _context.Announcements.Update(announcement);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var announcement = await _context.Announcements.FindAsync(id);
        if (announcement == null) return false;

        _context.Announcements.Remove(announcement);
        await _context.SaveChangesAsync();
        return true;
    }
}

