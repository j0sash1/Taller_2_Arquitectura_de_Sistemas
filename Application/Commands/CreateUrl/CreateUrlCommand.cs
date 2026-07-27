namespace Shortly.Application.Commands.CreateUrl;

public sealed record CreateUrlCommand(
    string Url,
    long UserId
);