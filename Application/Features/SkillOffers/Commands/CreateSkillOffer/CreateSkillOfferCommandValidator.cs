using FluentValidation;

namespace Application.Features.SkillOffers.Commands.CreateSkillOffer;

public class CreateSkillOfferCommandValidator : AbstractValidator<CreateSkillOfferCommand>
{
    public CreateSkillOfferCommandValidator()
    {
        RuleFor(x => x.SkillID)
            .NotEmpty().WithMessage("SkillID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

        RuleFor(x => x.Details)
            .MaximumLength(2000).WithMessage("Details must not exceed 2000 characters.")
            .When(x => x.Details != null);
    }
}
