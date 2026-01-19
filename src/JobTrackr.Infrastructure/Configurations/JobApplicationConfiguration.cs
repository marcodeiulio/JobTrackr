using JobTrackr.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTrackr.Infrastructure.Configurations;

public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Position)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Location)
            .HasMaxLength(200);

        builder.Property(c => c.JobUrl)
            .HasMaxLength(500);

        builder.HasOne(c => c.Company)
            .WithMany(c => c.JobApplications)
            .HasForeignKey(j => j.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.Status)
            .WithMany(js => js.JobApplications)
            .HasForeignKey(j => j.JobApplicationStatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(j => j.CompanyId);
        builder.HasIndex(j => j.JobApplicationStatusId);
        builder.HasIndex(j => j.Position);
        builder.HasIndex(j => j.AppliedDate);
    }
}