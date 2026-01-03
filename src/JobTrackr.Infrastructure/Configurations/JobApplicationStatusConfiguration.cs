using JobTrackr.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTrackr.Infrastructure.Configurations;

public class JobApplicationStatusConfiguration : IEntityTypeConfiguration<JobApplicationStatus>
{
    public void Configure(EntityTypeBuilder<JobApplicationStatus> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasMany(js => js.JobApplications)
            .WithOne(j => j.Status)
            .HasForeignKey(j => j.JobApplicationStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(js => js.Name).IsUnique();
        builder.HasIndex(js => js.DisplayOrder);
    }
}