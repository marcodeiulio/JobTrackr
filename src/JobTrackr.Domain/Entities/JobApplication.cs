namespace JobTrackr.Domain.Entities;

public class JobApplication
{
    private JobApplication()
    {
    }

    // Primary key
    public Guid Id { get; private set; } = Guid.NewGuid();

    // Properties
    public string Position { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime? AppliedDate { get; private set; }
    public string? Location { get; private set; }
    public string? JobUrl { get; private set; }
    public string? CoverLetter { get; private set; }
    public string? Notes { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid JobApplicationStatusId { get; private set; }

    // Audit fields
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    // Navigation properties
    public Company? Company { get; private set; }
    public JobApplicationStatus? Status { get; private set; }

    // Enforce business rules with factory methods
    public static JobApplication Create(string position, string? description, DateTime? appliedDate, string? location,
        string? jobUrl, string? coverLetter, string? notes, Guid companyId, Guid jobApplicationStatusId)
    {
        if (string.IsNullOrWhiteSpace(position))
            throw new ArgumentException("JobDescription position cannot be empty", nameof(position));

        if (companyId == Guid.Empty)
            throw new ArgumentException("CompanyId cannot be empty", nameof(companyId));

        if (jobApplicationStatusId == Guid.Empty)
            throw new ArgumentException("JobApplicationStatusId cannot be empty", nameof(jobApplicationStatusId));

        return new JobApplication
        {
            Position = position,
            Description = description,
            AppliedDate = appliedDate,
            Location = location,
            JobUrl = jobUrl,
            CoverLetter = coverLetter,
            Notes = notes,
            CompanyId = companyId,
            JobApplicationStatusId = jobApplicationStatusId
        };
    }

    public void UpdateDetails(string position, string? description, DateTime? appliedDate, string? location,
        string? jobUrl, string? coverLetter, string? notes, Guid companyId, Guid jobApplicationStatusId)
    {
        if (string.IsNullOrWhiteSpace(position))
            throw new ArgumentException("JobDescription position cannot be empty", nameof(position));

        if (companyId == Guid.Empty)
            throw new ArgumentException("CompanyId cannot be empty", nameof(companyId));

        if (jobApplicationStatusId == Guid.Empty)
            throw new ArgumentException("JobApplicationStatusId cannot be empty", nameof(jobApplicationStatusId));

        Position = position;
        Description = description;
        AppliedDate = appliedDate;
        Location = location;
        JobUrl = jobUrl;
        CoverLetter = coverLetter;
        Notes = notes;
        CompanyId = companyId;
        JobApplicationStatusId = jobApplicationStatusId;
        UpdatedAt = DateTime.UtcNow;
    }
}