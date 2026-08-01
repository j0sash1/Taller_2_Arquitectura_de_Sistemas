using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shortly.Domain.Entities;

// Read-only, denormalized table used only by queries. Stores the owner's
// email already resolved, so listing links never needs to join "users" at
// query time — that join happens once, here, when the write side changes.
[Table("link_read_models")]
public class LinkReadModel
{
    [Key]
    public long Id { get; private set; }

    public string Url { get; private set; } = null!;
    public string ShortUrl { get; private set; } = null!;
    public int Clicks { get; private set; }
    public long UserId { get; private set; }
    public string OwnerEmail { get; private set; } = null!;

    private LinkReadModel()
    {
    }

    public LinkReadModel(long id, string url, string shortUrl, int clicks, long userId, string ownerEmail)
    {
        Id = id;
        Url = url;
        ShortUrl = shortUrl;
        Clicks = clicks;
        UserId = userId;
        OwnerEmail = ownerEmail;
    }

    public void Update(string url, string shortUrl, int clicks, string ownerEmail)
    {
        Url = url;
        ShortUrl = shortUrl;
        Clicks = clicks;
        OwnerEmail = ownerEmail;
    }
}