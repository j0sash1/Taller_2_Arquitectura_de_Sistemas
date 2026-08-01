using Microsoft.EntityFrameworkCore;
using Shortly.Domain.Entities;

namespace Shortly.Infrastructure.Persistence;

// Separate DbContext for reads. Points at the same database as
// AppDbContext for now, but as its own instance, so read and write
// stay decoupled.
public class AppReadDbContext(DbContextOptions<AppReadDbContext> options) : DbContext(options)
{
    public DbSet<Link> Links { get; private set; } = null!;
    public DbSet<User> Users { get; private set; } = null!;

    // Denormalized read-model table (item 5). Kept up to date by
    // ILinkReadModelSynchronizer whenever a command writes.
    public DbSet<LinkReadModel> LinkReadModels { get; private set; } = null!;
}