using JobTrackr.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace JobTrackr.Infrastructure.Data.Seeding;

public class JobApplicationStatusSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, ILogger logger)
    {
        if (context.JobApplicationStatuses.Any())
        {
            logger.LogInformation("JobApplicationStatuses already seeded");
            return;
        }

        logger.LogInformation("Seeding JobApplicationStatuses");

        var statuses = new[]
        {
            JobApplicationStatus.Create("Wishlist", 1),
            JobApplicationStatus.Create("Applied", 2),
            JobApplicationStatus.Create("Phone Screen", 3),
            JobApplicationStatus.Create("Interview", 4),
            JobApplicationStatus.Create("Technical Assessment", 5),
            JobApplicationStatus.Create("Offer", 6),
            JobApplicationStatus.Create("Accepted", 7),
            JobApplicationStatus.Create("Rejected", 8),
            JobApplicationStatus.Create("Withdrawn", 9)
        };

        context.JobApplicationStatuses.AddRange(statuses);
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded {StatusesLength} JobApplicationStatuses", statuses.Length);
    }
}