using MediatR;

namespace Application.Features.Deals.Commands.CancelDeal;

public record CancelDealCommand(Guid DealID, Guid AccountID) : IRequest;
