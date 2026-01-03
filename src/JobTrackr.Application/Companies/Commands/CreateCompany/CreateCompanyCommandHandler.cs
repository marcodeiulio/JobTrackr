using JobTrackr.Application.Common.Interfaces;
using JobTrackr.Domain.Entities;
using MediatR;

namespace JobTrackr.Application.Companies.Commands.CreateCompany;

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateCompanyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateCompanyCommand request, CancellationToken ct)
    {
        var company = Company.Create(
            request.Name,
            request.Industry,
            request.Location,
            request.Website,
            request.Notes
        );

        _context.Companies.Add(company);

        await _context.SaveChangesAsync(ct);

        return company.Id;
    }
}