using JobTrackr.Application.Common.Interfaces;
using JobTrackr.Application.Companies.DTOs;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobTrackr.Application.Companies.Queries.GetCompanies;

public record GetCompaniesQuery : IRequest<List<CompanyDto>>;

public class GetCompaniesQueryHandler : IRequestHandler<GetCompaniesQuery, List<CompanyDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCompaniesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CompanyDto>> Handle(GetCompaniesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Companies
            .ProjectToType<CompanyDto>()
            .ToListAsync(cancellationToken);
    }
}