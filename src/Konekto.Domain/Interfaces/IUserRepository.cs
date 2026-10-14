using Konekto.Domain.Entities;

namespace Konekto.Domain.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User> AddAsync(User user);
    Task<bool> UpdateAsync(User user);
    Task<bool> DeleteAsync(int id);
}
