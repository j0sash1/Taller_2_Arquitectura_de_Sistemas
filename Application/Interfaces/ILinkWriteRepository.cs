using Shortly.Domain.Entities;

namespace Shortly.Application.Interfaces;

// Used only by command handlers (create, delete, etc).
public interface ILinkWriteRepository
{
    Task<Link?> GetByIdAsync(long id);

    Task AddAsync(Link link);

    Task DeleteAsync(Link link);

    Task SaveChangesAsync();
}