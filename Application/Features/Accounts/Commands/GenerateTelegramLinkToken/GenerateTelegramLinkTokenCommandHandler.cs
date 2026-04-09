using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Accounts.Commands.GenerateTelegramLinkToken;

public class GenerateTelegramLinkTokenCommandHandler : IRequestHandler<GenerateTelegramLinkTokenCommand, string>
{
    private readonly IApplicationDbContext _context;

    public GenerateTelegramLinkTokenCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(GenerateTelegramLinkTokenCommand request, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts.FindAsync(new object[] { request.AccountID }, cancellationToken);

        if (account is null)
            throw new NotFoundException(nameof(Domain.Entities.Account), request.AccountID);

        if (!string.IsNullOrEmpty(account.TelegramID))
            throw new InvalidOperationException("Telegram уже привязан к этому аккаунту.");

        // Генерируем 8-символьный токен (буквы + цифры)
        var token = GenerateToken();
        account.TelegramLinkToken = token;

        await _context.SaveChangesAsync(cancellationToken);
        return token;
    }

    private static string GenerateToken()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        return new string(Enumerable.Range(0, 8).Select(_ => chars[random.Next(chars.Length)]).ToArray());
    }
}
