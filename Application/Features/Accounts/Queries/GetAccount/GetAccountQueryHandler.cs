using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Accounts.Queries.GetAccount;

public class GetAccountQueryHandler : IRequestHandler<GetAccountQuery, AccountDto>
{
    private readonly IApplicationDbContext _context;

    public GetAccountQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AccountDto> Handle(GetAccountQuery request, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts
            .Where(a => a.AccountID == request.AccountID)
            .Select(a => new AccountDto
            {
                AccountID = a.AccountID,
                Login = a.Login,
                TelegramID = a.TelegramID,
                IsAdmin = a.IsAdmin,
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (account == null)
        {
            throw new NotFoundException(nameof(Domain.Entities.Account), request.AccountID);
        }

        return account;
    }
}


