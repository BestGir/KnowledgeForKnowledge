using FluentValidation;

namespace Application.Features.Applications.Commands.CreateApplication;

public class CreateApplicationCommandValidator : AbstractValidator<CreateApplicationCommand>
{
    public CreateApplicationCommandValidator()
    {
        RuleFor(x => x)
            .Must(x => x.OfferID.HasValue ^ x.SkillRequestID.HasValue)
            .WithMessage("Укажите ровно одно из полей: OfferID или SkillRequestID.");

        RuleFor(x => x.Message)
            .MaximumLength(1000).WithMessage("Message must not exceed 1000 characters.")
            .When(x => x.Message != null);
    }
}
