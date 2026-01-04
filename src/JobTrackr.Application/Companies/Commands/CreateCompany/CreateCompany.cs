using JobTrackr.Application.Common.Interfaces;
using JobTrackr.Domain.Entities;
using MediatR;

namespace JobTrackr.Application.Companies.Commands.CreateCompany;

public record CreateCompanyCommand(
    string Name,
    string? Industry,
    string? Location,
    string? Website,
    string? Notes
) : IRequest<Guid>;

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateCompanyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = Company.Create(
            request.Name,
            request.Industry,
            request.Location,
            request.Website,
            request.Notes
        );

        _context.Companies.Add(company);

        await _context.SaveChangesAsync(cancellationToken);

        return company.Id;
    }
}