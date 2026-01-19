using FluentValidation;
using JobTrackr.Application.Common.Helpers;
using JobTrackr.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JobTrackr.Application.Companies.Commands.CreateCompany;

public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateCompanyCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name cannot be empty.")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.")
            .MustAsync(BeUniqueName).WithMessage("A company with this name already exists.");

        RuleFor(c => c.Industry)
            .MaximumLength(100).WithMessage("Industry cannot exceed 100 characters.");

        RuleFor(c => c.Location)
            .MaximumLength(200).WithMessage("Location cannot exceed 200 characters.");

        RuleFor(c => c.Website)
            .MaximumLength(500).WithMessage("Website cannot exceed 500 characters.")
            .Must(ValidationHelpers.BeValidUrlIfProvided).WithMessage("Invalid URL.");
    }

    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return !await _context.Companies.AnyAsync(c => c.Name == name, cancellationToken);
    }
}