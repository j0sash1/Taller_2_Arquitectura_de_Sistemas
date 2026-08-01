using Microsoft.EntityFrameworkCore;
using Shortly.Application.Interfaces;
using Shortly.Domain.Entities;
using Shortly.Infrastructure.Persistence;

namespace Shortly.Infrastructure.ReadRepositories;

public sealed class LinkReadRepository : ILinkReadRepository
{
    private readonly AppReadDbContext _context;

    public LinkReadRepository(AppReadDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task<Link?> GetByShortUrlAsync(string shortUrl)
        => _context.Links.AsNoTracking().FirstOrDefaultAsync(l => l.ShortUrl == shortUrl);

    public Task<List<Link>> GetAllAsync()
        => _context.Links.AsNoTracking().ToListAsync();

    public Task<List<Link>> GetByUserIdAsync(long userId)
        => _context.Links.AsNoTracking().Where(l => l.UserId == userId).ToListAsync();
}