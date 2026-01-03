namespace JobTrackr.Application.Companies.DTOs;

public record CompanyDto(
    Guid Id,
    string Name,
    string? Industry,
    string? Location,
    string? Website,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);