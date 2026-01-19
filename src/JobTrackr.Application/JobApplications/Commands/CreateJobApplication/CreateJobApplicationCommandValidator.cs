using FluentValidation;
using JobTrackr.Application.Common.Helpers;
using JobTrackr.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JobTrackr.Application.JobApplications.Commands.CreateJobApplication;

public class CreateJobApplicationCommandValidator : AbstractValidator<CreateJobApplicationCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateJobApplicationCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(j => j.Position)
            .NotEmpty().WithMessage("Position cannot be empty.")
            .MaximumLength(200).WithMessage("Position cannot exceed 200 characters.");

        RuleFor(j => j.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(j => j.Location)
            .MaximumLength(200).WithMessage("Location cannot exceed 200 characters.");

        RuleFor(j => j.JobUrl)
            .MaximumLength(500).WithMessage("JobUrl cannot exceed 500 characters.")
            .Must(ValidationHelpers.BeValidUrlIfProvided).WithMessage("Invalid URL");

        RuleFor(j => j.CompanyId)
            .NotEmpty().WithMessage("Company Id cannot be empty.")
            .MustAsync(CompanyExists).WithMessage("Company does not exist.");

        RuleFor(j => j.JobApplicationStatusId)
            .NotEmpty().WithMessage("Status Id cannot be empty.")
            .MustAsync(StatusExists).WithMessage("Status does not exist.");
    }

    private async Task<bool> CompanyExists(Guid companyId, CancellationToken cancellationToken)
    {
        return await _context.Companies.AnyAsync(c => c.Id == companyId, cancellationToken);
    }

    private async Task<bool> StatusExists(Guid statusId, CancellationToken cancellationToken)
    {
        return await _context.JobApplicationStatuses.AnyAsync(s => s.Id == statusId, cancellationToken);
    }
}