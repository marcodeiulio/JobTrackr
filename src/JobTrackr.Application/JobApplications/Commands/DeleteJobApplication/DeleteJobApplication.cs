using JobTrackr.Application.Common.Interfaces;
using JobTrackr.Domain.Entities;
using JobTrackr.Domain.Exceptions;
using MediatR;

namespace JobTrackr.Application.JobApplications.Commands.DeleteJobApplication;

public record DeleteJobApplicationCommand(Guid Id) : IRequest<Unit>;

public class DeleteJobApplicationCommandHandler : IRequestHandler<DeleteJobApplicationCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteJobApplicationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteJobApplicationCommand request, CancellationToken cancellationToken)
    {
        var jobApplication = await _context.JobApplications.FindAsync(request.Id, cancellationToken);

        if (jobApplication is null)
            throw new NotFoundException(nameof(JobApplication), request.Id);

        _context.JobApplications.Remove(jobApplication);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}