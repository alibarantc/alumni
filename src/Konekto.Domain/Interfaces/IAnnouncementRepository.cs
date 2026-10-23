using Konekto.Domain.Entities;

namespace Konekto.Domain.Interfaces;

public interface IAnnouncementRepository
{
    Task<IEnumerable<Announcement>> GetAllAsync();
    Task<IEnumerable<Announcement>> GetActiveAsync();
    Task<Announcement?> GetByIdAsync(int id);
    Task<Announcement> AddAsync(Announcement announcement);
    Task UpdateAsync(Announcement announcement);
    Task<bool> DeleteAsync(int id);
}
