using JobTrackr.Application.Common.Interfaces;
using JobTrackr.Domain.Entities;
using JobTrackr.Domain.Exceptions;
using MediatR;

namespace JobTrackr.Application.Companies.Commands.DeleteCompany;

public record DeleteCompanyCommand(Guid Id) : IRequest<Unit>;

public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteCompanyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies.FindAsync(request.Id, cancellationToken);

        if (company == null)
            throw new NotFoundException(nameof(Company), request.Id);

        _context.Companies.Remove(company);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}