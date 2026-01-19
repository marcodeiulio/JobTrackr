using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using JobTrackr.Application.Common.Interfaces;
using JobTrackr.Application.JobApplications.Commands.UpdateJobApplication;
using JobTrackr.Application.Tests.Common;
using JobTrackr.Domain.Entities;
using Xunit;

namespace JobTrackr.Application.Tests.JobApplications.Commands;

public class UpdateJobApplicationCommandValidatorTests
{
    private static UpdateJobApplicationCommand UpdateJobApplicationCommandBuilder(
        Guid id = default,
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
        return new UpdateJobApplicationCommand(id,
            position, description, appliedDate, location, jobUrl, coverLetter, notes, companyId, jobApplicationStatusId
        );
    }

    private static async
        Task<(Company company, JobApplicationStatus status, UpdateJobApplicationCommandValidator validator)>
        GetArrangement(
            IApplicationDbContext context)
    {
        var company = Company.Create("Test Company", null, null, null, null);
        var status = JobApplicationStatus.Create("Status", 0);
        context.Companies.Add(company);
        context.JobApplicationStatuses.Add(status);
        await context.SaveChangesAsync(CancellationToken.None);
        var validator = new UpdateJobApplicationCommandValidator(context);

        return (company, status, validator);
    }

    // Happy path
    [Fact]
    public async Task Validate_ValidJobApplication_PassesValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (company, status, validator) = await GetArrangement(context);
        var command = UpdateJobApplicationCommandBuilder(
            Guid.NewGuid(),
            companyId: company.Id,
            jobApplicationStatusId: status.Id);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    // Id validation
    [Fact]
    public async Task Validate_EmptyId_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (company, status, validator) = await GetArrangement(context);
        var command = UpdateJobApplicationCommandBuilder(
            Guid.Empty,
            companyId: company.Id,
            jobApplicationStatusId: status.Id);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(j => j.Id);
    }

    // Position validation
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validate_InvalidPosition_FailsValidation(string position)
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (company, status, validator) = await GetArrangement(context);
        var command = UpdateJobApplicationCommandBuilder(
            Guid.NewGuid(),
            position,
            companyId: company.Id,
            jobApplicationStatusId: status.Id);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(j => j.Position);
    }

    [Fact]
    public async Task Validate_PositionExceedsMaxLength_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (company, status, validator) = await GetArrangement(context);
        var command = UpdateJobApplicationCommandBuilder(
            Guid.NewGuid(),
            new string('A', 250),
            companyId: company.Id,
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
        var command = UpdateJobApplicationCommandBuilder(
            Guid.NewGuid(),
            description: new string('A', 1050),
            companyId: company.Id,
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
        var command = UpdateJobApplicationCommandBuilder(
            Guid.NewGuid(),
            location: new string('A', 250),
            companyId: company.Id,
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
        var command = UpdateJobApplicationCommandBuilder(
            Guid.NewGuid(),
            jobUrl: new string('A', 250),
            companyId: company.Id,
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
    public async Task Validate_UrlVariants_FailsOrPassesValidation(string url, bool expected)
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (company, status, validator) = await GetArrangement(context);
        var command = UpdateJobApplicationCommandBuilder(
            Guid.NewGuid(),
            jobUrl: url,
            companyId: company.Id,
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
        var command = UpdateJobApplicationCommandBuilder(
            Guid.NewGuid(),
            companyId: Guid.Empty,
            jobApplicationStatusId: status.Id);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(j => j.CompanyId);
    }

    [Fact]
    public async Task Validate_NonExistentCompany_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (company, status, validator) = await GetArrangement(context);
        var command = UpdateJobApplicationCommandBuilder(
            Guid.NewGuid(),
            companyId: Guid.NewGuid(),
            jobApplicationStatusId: status.Id);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(j => j.CompanyId);
    }

    // JobApplicationStatusId validation
    [Fact]
    public async Task Validate_InvalidStatusId_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (company, status, validator) = await GetArrangement(context);
        var command = UpdateJobApplicationCommandBuilder(
            Guid.NewGuid(),
            companyId: company.Id,
            jobApplicationStatusId: Guid.Empty);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(j => j.JobApplicationStatusId);
    }

    [Fact]
    public async Task Validate_NonExistentStatus_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (company, status, validator) = await GetArrangement(context);
        var command = UpdateJobApplicationCommandBuilder(
            Guid.NewGuid(),
            companyId: company.Id,
            jobApplicationStatusId: Guid.NewGuid());

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(j => j.JobApplicationStatusId);
    }
}