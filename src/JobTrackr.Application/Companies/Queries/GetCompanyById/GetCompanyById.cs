using JobTrackr.Application.Common.Interfaces;
using JobTrackr.Application.Companies.DTOs;
using JobTrackr.Domain.Entities;
using JobTrackr.Domain.Exceptions;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JobTrackr.Application.Companies.Queries.GetCompanyById;

public record GetCompanyByIdQuery(Guid Id) : IRequest<CompanyDto>;

public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, CompanyDto>
{
    private readonly IApplicationDbContext _context;

    public GetCompanyByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CompanyDto> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies.Where(c => c.Id == request.Id)
            .ProjectToType<CompanyDto>()
            .FirstOrDefaultAsync(cancellationToken);

        if (company is null)
            throw new NotFoundException(nameof(Company), request.Id);

        return company;
    }
}