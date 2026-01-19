using JobTrackr.Application.Common.Interfaces;
using JobTrackr.Domain.Entities;
using MediatR;

namespace JobTrackr.Application.JobApplications.Commands.CreateJobApplication;

public record CreateJobApplicationCommand(
    string Position,
    string? Description,
    DateTime? AppliedDate,
    string? Location,
    string? JobUrl,
    string? CoverLetter,
    string? Notes,
    Guid CompanyId,
    Guid JobApplicationStatusId
) : IRequest<Guid>;

public class CreateJobApplicationCommandHandler : IRequestHandler<CreateJobApplicationCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateJobApplicationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateJobApplicationCommand request, CancellationToken cancellationToken)
    {
        var jobApplication = JobApplication.Create(
            request.Position,
            request.Description,
            request.AppliedDate,
            request.Location,
            request.JobUrl,
            request.CoverLetter,
            request.Notes,
            request.CompanyId,
            request.JobApplicationStatusId
        );

        _context.JobApplications.Add(jobApplication);

        await _context.SaveChangesAsync(cancellationToken);

        return jobApplication.Id;
    }
}