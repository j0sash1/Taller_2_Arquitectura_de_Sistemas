using Shortly.Application.DTOs;
using Shortly.Application.Interfaces;

namespace Shortly.Application.Queries.GetUrl;

public sealed class GetUrlQueryHandler
{
    private readonly ILinkReadRepository _repository;

    public GetUrlQueryHandler(
        ILinkReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<LinkResponse> Handle(GetUrlQuery query)
    {
        var link = await _repository.GetByShortUrlAsync(query.ShortUrl);

        if (link is null)
            throw new KeyNotFoundException();

        return link;
    }
}