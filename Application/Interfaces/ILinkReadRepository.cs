using Shortly.Domain.Entities;

namespace Shortly.Application.Interfaces;

// Used only by query handlers. Reads are untracked (no EF change tracker).
public interface ILinkReadRepository
{
    Task<Link?> GetByShortUrlAsync(string shortUrl);

    Task<List<Link>> GetAllAsync();

    Task<List<Link>> GetByUserIdAsync(long userId);
}