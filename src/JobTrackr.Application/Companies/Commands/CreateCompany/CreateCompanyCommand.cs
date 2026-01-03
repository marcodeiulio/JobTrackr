using MediatR;

namespace JobTrackr.Application.Companies.Commands.CreateCompany;

public record CreateCompanyCommand(
    string Name,
    string? Industry,
    string? Location,
    string? Website,
    string? Notes
) : IRequest<Guid>;