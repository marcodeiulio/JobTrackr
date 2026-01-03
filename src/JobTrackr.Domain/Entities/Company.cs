namespace JobTrackr.Domain.Entities;

public class Company
{
    // Private constructor for EF Core
    private Company()
    {
    }

    // Primary key
    public Guid Id { get; private set; } = Guid.NewGuid();

    // Properties
    public string Name { get; private set; } = string.Empty;
    public string? Industry { get; private set; }
    public string? Location { get; private set; }
    public string? Website { get; private set; }
    public string? Notes { get; private set; }

    // Audit fields
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    // Navigation properties
    public ICollection<JobApplication> JobApplications { get; private set; } = new List<JobApplication>();

    // Enforce business rules with factory methods
    public static Company Create(string name, string? industry, string? location, string? website, string? notes)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Company name cannot be empty", nameof(name));

        return new Company
        {
            Name = name,
            Industry = industry,
            Location = location,
            Website = website,
            Notes = notes
        };
    }


    public void UpdateDetails(string name, string? industry, string? location, string? website, string? notes)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Company name cannot be empty", nameof(name));

        Name = name;
        Industry = industry;
        Location = location;
        Website = website;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}