using FluentValidation;
using JobTrackr.Application.Common.Interfaces;
using JobTrackr.Application.Companies.Commands.Shared;
using Microsoft.EntityFrameworkCore;

namespace JobTrackr.Application.Companies.Commands.UpdateCompany;

public class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateCompanyCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(c => c.Id)
            .NotEmpty().WithMessage("Id cannot be empty.")
            .NotEqual(Guid.Empty).WithMessage("Id must be valid GUID");

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
            .Must(CompanyCommandValidatorHelpers.BeValidUrlIfProvided).WithMessage("Invalid url.");
    }

    private async Task<bool> BeUniqueName(UpdateCompanyCommand command, string name,
        CancellationToken cancellationToken)
    {
        return !await _context.Companies.AnyAsync(c => c.Id != command.Id && c.Name == name, cancellationToken);
    }
}