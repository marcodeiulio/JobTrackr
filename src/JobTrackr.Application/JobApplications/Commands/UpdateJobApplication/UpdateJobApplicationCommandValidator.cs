using FluentValidation;
using JobTrackr.Application.Common.Helpers;
using JobTrackr.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JobTrackr.Application.JobApplications.Commands.UpdateJobApplication;

public class UpdateJobApplicationCommandValidator : AbstractValidator<UpdateJobApplicationCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateJobApplicationCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(j => j.Id)
            .NotEmpty().WithMessage("Id cannot be empty.")
            .NotEqual(Guid.Empty).WithMessage("Id cannot be empty Guid.");

        RuleFor(j => j.Position)
            .NotEmpty().WithMessage("Position cannot be empty.")
            .MaximumLength(200).WithMessage("Position cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

        RuleFor(x => x.Location)
            .MaximumLength(200).WithMessage("Location cannot exceed 200 characters.");

        RuleFor(x => x.JobUrl)
            .MaximumLength(500).WithMessage("Job URL cannot exceed 500 characters.")
            .Must(ValidationHelpers.BeValidUrlIfProvided).WithMessage("Invalid job URL.");

        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("CompanyId cannot be empty.")
            .MustAsync(CompanyExists).WithMessage("Company does not exist.");

        RuleFor(x => x.JobApplicationStatusId)
            .NotEmpty().WithMessage("JobApplicationStatusId cannot be empty.")
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