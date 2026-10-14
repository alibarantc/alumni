using Microsoft.EntityFrameworkCore;
using Konekto.DataAccess.Context;
using Konekto.Domain.Entities;
using Konekto.Domain.Interfaces;

namespace Konekto.DataAccess.Repositories;

public class AlumniRepository : IAlumniRepository
{
    private readonly KonektoDbContext _context;

    public AlumniRepository(KonektoDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Alumni>> GetAllAsync()
    {
        return await _context.Alumni.ToListAsync();
    }

    public async Task<Alumni?> GetByIdAsync(int id)
    {
        return await _context.Alumni.FindAsync(id);
    }

    public async Task<Alumni> AddAsync(Alumni alumni)
    {
        await _context.Alumni.AddAsync(alumni);
        await _context.SaveChangesAsync();
        return alumni;
    }

    public async Task<bool> UpdateAsync(Alumni alumni)
    {
        var existing = await _context.Alumni.FindAsync(alumni.Id);
        if (existing == null) return false;

        existing.Name = alumni.Name;
        existing.GraduationYear = alumni.GraduationYear;
        existing.CurrentCompany = alumni.CurrentCompany;
        existing.CurrentTitle = alumni.CurrentTitle;
        existing.DepartmentId = alumni.DepartmentId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.Alumni.FindAsync(id);
        if (existing == null) return false;

        _context.Alumni.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}

