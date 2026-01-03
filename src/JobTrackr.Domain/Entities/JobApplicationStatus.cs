namespace JobTrackr.Domain.Entities;

public class JobApplicationStatus
{
    private JobApplicationStatus()
    {
    }

    // Primary key
    public Guid Id { get; private set; } = Guid.NewGuid();

    // Properties
    public string Name { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }

    // Audit fields
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    // Navigation properties
    public ICollection<JobApplication> JobApplications { get; private set; } = new List<JobApplication>();

    // Enforce business rules with factory methods
    public static JobApplicationStatus Create(string name, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("JobApplication name cannot be empty", nameof(name));

        return new JobApplicationStatus
        {
            Name = name,
            DisplayOrder = displayOrder
        };
    }

    public void UpdateDetails(string name, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("JobApplication name cannot be empty", nameof(name));

        Name = name;
        DisplayOrder = displayOrder;
    }
}