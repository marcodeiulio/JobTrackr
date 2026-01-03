using JobTrackr.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobTrackr.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Company> Companies { get; }
    DbSet<JobApplication> JobApplications { get; }
    DbSet<JobApplicationStatus> JobApplicationStatuses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}