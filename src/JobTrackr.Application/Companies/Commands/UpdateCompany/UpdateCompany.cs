using JobTrackr.Application.Common.Interfaces;
using JobTrackr.Domain.Entities;
using JobTrackr.Domain.Exceptions;
using MediatR;

namespace JobTrackr.Application.Companies.Commands.UpdateCompany;

public record UpdateCompanyCommand(
    Guid Id,
    string Name,
    string? Industry,
    string? Location,
    string? Website,
    string? Notes
) : IRequest<Unit>;

public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateCompanyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies.FindAsync(request.Id, cancellationToken);

        if (company == null)
            throw new NotFoundException(nameof(Company), request.Id);

        company.UpdateDetails(
            request.Name,
            request.Industry,
            request.Location,
            request.Website,
            request.Notes
        );

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}