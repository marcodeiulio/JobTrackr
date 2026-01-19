using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using JobTrackr.Application.Common.Interfaces;
using JobTrackr.Application.JobApplications.Commands.CreateJobApplication;
using JobTrackr.Application.Tests.Common;
using JobTrackr.Domain.Entities;
using Xunit;

namespace JobTrackr.Application.Tests.JobApplications.Commands;

public class CreateJobApplicationCommandValidatorTests
{
    private static CreateJobApplicationCommand CreateJobApplicationCommandBuilder(
        string position = "Test Position",
        string? description = null,
        DateTime? appliedDate = null,
        string? location = null,
        string? jobUrl = null,
        string? coverLetter = null,
        string? notes = null,
        Guid companyId = default,
        Guid jobApplicationStatusId = default
    )
    {
        return new CreateJobApplicationCommand(
            position, description, appliedDate, location, jobUrl, coverLetter, notes, companyId, jobApplicationStatusId
        );
    }

    private static async
        Task<(Company company, JobApplicationStatus status, CreateJobApplicationCommandValidator validator)>
        GetArrangement(
            IApplicationDbContext context)
    {
        var company = Company.Create("Test Company", null, null, null, null);
        var status = JobApplicationStatus.Create("Status", 0);
        context.Companies.Add(company);
        context.JobApplicationStatuses.Add(status);
        await context.SaveChangesAsync(CancellationToken.None);
        var validator = new CreateJobApplicationCommandValidator(context);

        return (company, status, validator);
    }

    // Happy paths
    [Fact]
    public async Task Validate_ValidJobApplicationWithAllProperties_PassesValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (company, status, validator) = await GetArrangement(context);
        var command = CreateJobApplicationCommandBuilder("Test Position",
            "Description", DateTime.UtcNow, "Location", "joburl.com", "Cover Letter", "Notes", company.Id, status.Id);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_ValidJobApplicationWithRequiredFieldsOnly_PassesValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (company, status, validator) = await GetArrangement(context);
        var command = CreateJobApplicationCommandBuilder(companyId: company.Id, jobApplicationStatusId: status.Id);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    // Position validation
    [Fact]
    public async Task Validate_EmptyPosition_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (company, status, validator) = await GetArrangement(context);
        var command = CreateJobApplicationCommandBuilder("", companyId: company.Id, jobApplicationStatusId: status.Id);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(j => j.Position);
    }

    [Fact]
    public async Task Validate_PositionExceedsMaxLength_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (company, status, validator) = await GetArrangement(context);
        var command = CreateJobApplicationCommandBuilder(new string('A', 250), companyId: company.Id,
            jobApplicationStatusId: status.Id);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(j => j.Position);
    }

    // Description validation
    [Fact]
    public async Task Validate_DescriptionExceedsMaxLength_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (company, status, validator) = await GetArrangement(context);
        var command = CreateJobApplicationCommandBuilder(description: new string('A', 1050), companyId: company.Id,
            jobApplicationStatusId: status.Id);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(j => j.Description);
    }

    // Location validation
    [Fact]
    public async Task Validate_LocationExceedsMaxLength_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (company, status, validator) = await GetArrangement(context);
        var command = CreateJobApplicationCommandBuilder(location: new string('A', 250), companyId: company.Id,
            jobApplicationStatusId: status.Id);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(j => j.Location);
    }

    // JobUrl validation
    [Fact]
    public async Task Validate_JobUrlExceedsMaxLength_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (company, status, validator) = await GetArrangement(context);
        var command = CreateJobApplicationCommandBuilder(jobUrl: new string('A', 550), companyId: company.Id,
            jobApplicationStatusId: status.Id);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(j => j.JobUrl);
    }

    [Theory]
    [InlineData("https://valid.com", true)]
    [InlineData("http://valid.com", true)]
    [InlineData("www.valid.com", true)]
    [InlineData("valid.com", true)]
    [InlineData("sub.valid.co.uk", true)]
    [InlineData("invalid", false)]
    [InlineData("http:invalid", false)]
    [InlineData("https://invalid", false)]
    [InlineData("https://.invalid", false)]
    [InlineData("https://invalid.", false)]
    [InlineData("ftp://invalid.com", false)]
    public async Task Validate_JobUrlVariants_FailsOrPassesValidation(string url, bool expected)
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (company, status, validator) = await GetArrangement(context);
        var command = CreateJobApplicationCommandBuilder(jobUrl: url, companyId: company.Id,
            jobApplicationStatusId: status.Id);

        var result = await validator.TestValidateAsync(command);

        if (expected)
            result.ShouldNotHaveAnyValidationErrors();
        else result.ShouldHaveValidationErrorFor(j => j.JobUrl);
    }

    // CompanyId validation
    [Fact]
    public async Task Validate_InvalidCompanyId_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (company, status, validator) = await GetArrangement(context);
        var command = CreateJobApplicationCommandBuilder(companyId: Guid.Empty,
            jobApplicationStatusId: status.Id);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(j => j.CompanyId);
    }

    [Fact]
    public async Task Validate_NonExistentCompany_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (company, status, validator) = await GetArrangement(context);
        var command = CreateJobApplicationCommandBuilder(companyId: Guid.NewGuid(),
            jobApplicationStatusId: status.Id);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(j => j.CompanyId);
    }

    // StatusId validation
    [Fact]
    public async Task Validate_InvalidStatusId_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (company, status, validator) = await GetArrangement(context);
        var command = CreateJobApplicationCommandBuilder(companyId: company.Id, jobApplicationStatusId: Guid.Empty);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(j => j.JobApplicationStatusId);
    }

    [Fact]
    public async Task Validate_NonExistentStatus_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (company, status, validator) = await GetArrangement(context);
        var command = CreateJobApplicationCommandBuilder(companyId: company.Id,
            jobApplicationStatusId: Guid.NewGuid());

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(j => j.JobApplicationStatusId);
    }
}