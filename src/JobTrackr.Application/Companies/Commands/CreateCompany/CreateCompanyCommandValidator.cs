using FluentValidation;

namespace JobTrackr.Application.Companies.Commands.CreateCompany;

public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name cannot be empty.")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");

        RuleFor(c => c.Industry)
            .MaximumLength(100).WithMessage("Industry cannot exceed 100 characters.");

        RuleFor(c => c.Location)
            .MaximumLength(200).WithMessage("Location cannot exceed 200 characters.");

        RuleFor(c => c.Website)
            .MaximumLength(500).WithMessage("Website cannot exceed 500 characters.")
            .Must(BeValidUrlIfProvided).WithMessage("Invalid url.");
    }

    private bool BeValidUrlIfProvided(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}