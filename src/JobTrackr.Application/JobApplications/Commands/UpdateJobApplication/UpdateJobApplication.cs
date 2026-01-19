using JobTrackr.Application.Common.Interfaces;
using JobTrackr.Domain.Entities;
using JobTrackr.Domain.Exceptions;
using MediatR;

namespace JobTrackr.Application.JobApplications.Commands.UpdateJobApplication;

public record UpdateJobApplicationCommand(
    Guid Id,
    string Position,
    string? Description,
    DateTime? AppliedDate,
    string? Location,
    string? JobUrl,
    string? CoverLetter,
    string? Notes,
    Guid CompanyId,
    Guid JobApplicationStatusId
) : IRequest<Unit>;

public class UpdateJobApplicationCommandHandler : IRequestHandler<UpdateJobApplicationCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateJobApplicationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateJobApplicationCommand request, CancellationToken cancellationToken)
    {
        var jobApplication = await _context.JobApplications.FindAsync(request.Id, cancellationToken);

        if (jobApplication is null)
            throw new NotFoundException(nameof(JobApplication), request.Id);

        jobApplication.UpdateDetails(
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

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}