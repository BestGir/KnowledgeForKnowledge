using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

namespace Application.Features.Accounts.Commands.CreateAccount;

public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public CreateAccountCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = new Account
        {
            AccountID   = Guid.NewGuid(),
            Login       = request.Login,
            PasswordHash = _passwordHasher.Hash(request.Password),
            TelegramID  = request.TelegramID,
            IsAdmin     = false,
            CreatedAt   = DateTime.UtcNow
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync(cancellationToken);

        return account.AccountID;
    }
}
