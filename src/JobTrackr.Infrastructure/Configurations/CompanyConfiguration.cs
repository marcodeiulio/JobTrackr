using JobTrackr.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTrackr.Infrastructure.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Industry)
            .HasMaxLength(100);

        builder.Property(c => c.Location)
            .HasMaxLength(200);

        builder.Property(c => c.Website)
            .HasMaxLength(500);

        builder.HasMany(c => c.JobApplications)
            .WithOne(j => j.Company)
            .HasForeignKey(j => j.CompanyId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete of JobApplications

        builder.HasIndex(c => c.Name);
    }
}