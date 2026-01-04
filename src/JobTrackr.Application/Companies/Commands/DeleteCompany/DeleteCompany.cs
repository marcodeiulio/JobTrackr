using JobTrackr.Application.Common.Interfaces;
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
            // todo throw NotFoundException(nameof(Company), request.Id);
            throw new Exception($"Company with id {request.Id} not found");

        _context.Companies.Remove(company);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}