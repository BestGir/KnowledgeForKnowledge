using FluentValidation;

namespace Application.Features.UserProfiles.Commands.UpsertUserProfile;

public class UpsertUserProfileCommandValidator : AbstractValidator<UpsertUserProfileCommand>
{
    public UpsertUserProfileCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("FullName is required.")
            .MaximumLength(150).WithMessage("FullName must not exceed 150 characters.");

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow).WithMessage("Date of birth must be in the past.")
            .GreaterThan(DateTime.UtcNow.AddYears(-120)).WithMessage("Date of birth is not valid.")
            .When(x => x.DateOfBirth.HasValue);

        RuleFor(x => x.ContactInfo)
            .MaximumLength(255).WithMessage("ContactInfo must not exceed 255 characters.")
            .When(x => x.ContactInfo != null);

        RuleFor(x => x.Description)
            .MaximumLength(3000).WithMessage("Description must not exceed 3000 characters.")
            .When(x => x.Description != null);
    }
}
