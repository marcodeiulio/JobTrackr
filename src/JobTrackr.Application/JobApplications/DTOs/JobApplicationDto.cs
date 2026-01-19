namespace JobTrackr.Application.JobApplications.DTOs;

public record JobApplicationDto(
    Guid Id,
    string Position,
    string? Description,
    DateTime? AppliedDate,
    string? Location,
    string? JobUrl,
    string? CoverLetter,
    string? Notes,
    Guid CompanyId,
    string CompanyName,
    Guid JobApplicationStatusId,
    string StatusName,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);