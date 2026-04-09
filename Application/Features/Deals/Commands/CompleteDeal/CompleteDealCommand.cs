using MediatR;

namespace Application.Features.Deals.Commands.CompleteDeal;

public record CompleteDealCommand(Guid DealID, Guid AccountID) : IRequest;
