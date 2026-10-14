using Konekto.Domain.Entities;

namespace Konekto.Domain.Interfaces;

public interface IAlumniRepository
{
    Task<IEnumerable<Alumni>> GetAllAsync();
    Task<Alumni?> GetByIdAsync(int id);
    Task<Alumni> AddAsync(Alumni alumni);
    Task<bool> UpdateAsync(Alumni alumni);
    Task<bool> DeleteAsync(int id);
}

