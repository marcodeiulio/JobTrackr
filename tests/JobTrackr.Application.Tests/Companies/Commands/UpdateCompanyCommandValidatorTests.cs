using FluentValidation.TestHelper;
using JobTrackr.Application.Companies.Commands.CreateCompany;
using JobTrackr.Application.Companies.Commands.UpdateCompany;
using JobTrackr.Application.Tests.Common;
using JobTrackr.Domain.Entities;
using JobTrackr.Infrastructure.Data;

namespace JobTrackr.Application.Tests.Companies.Commands;

public class UpdateCompanyCommandValidatorTests
{
    private static async Task<(UpdateCompanyCommandValidator validator, Guid toBeUpdatedCompanyId)> MockSetup(
        ApplicationDbContext context,
        CreateCompanyCommand company)
    {
        var validator = new UpdateCompanyCommandValidator(context);
        var toBeUpdatedCompany = Company.Create(company.Name, company.Industry, company.Location, company.Website,
            company.Notes);
        context.Add(toBeUpdatedCompany);
        await context.SaveChangesAsync(CancellationToken.None);
        var toBeUpdatedCompanyId = toBeUpdatedCompany.Id;
        return (validator, toBeUpdatedCompanyId);
    }

    // Id validation
    [Fact]
    public async Task Validate_EmptyId_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var validator = new UpdateCompanyCommandValidator(context);
        var command = new UpdateCompanyCommand(Guid.Empty, "New name", null, null, null, null);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Id);
    }

    // Name validation
    [Fact]
    public async Task Validate_ValidCompanyNameUpdate_PassesValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (validator, toBeUpdatedCompanyId) =
            await MockSetup(context, new CreateCompanyCommand("Name", null, null, null, null));
        var command = new UpdateCompanyCommand(toBeUpdatedCompanyId, "New name", null, null, null, null);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_SameNameForSameCompany_PassesValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (validator, toBeUpdatedCompanyId) =
            await MockSetup(context, new CreateCompanyCommand("Name", null, null, null, null));
        var command = new UpdateCompanyCommand(toBeUpdatedCompanyId, "Name", null, null, null, null);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_DifferentCompanyWithSameName_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var existingCompany = Company.Create("Existing Company Name", null, null, null, null);
        context.Add(existingCompany);
        var (validator, toBeUpdatedCompanyId) =
            await MockSetup(context, new CreateCompanyCommand("Name", null, null, null, null));
        await context.SaveChangesAsync(CancellationToken.None);
        var command = new UpdateCompanyCommand(toBeUpdatedCompanyId, "Existing Company Name", null, null, null, null);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public async Task Validate_NameExceedsMaxLength_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (validator, toBeUpdatedCompanyId) =
            await MockSetup(context, new CreateCompanyCommand("Name", null, null, null, null));
        var command = new UpdateCompanyCommand(toBeUpdatedCompanyId, new string('A', 250), null, null, null, null);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    // Industry validation
    [Fact]
    public async Task Validate_IndustryExceedsMaxLength_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (validator, toBeUpdatedCompanyId) =
            await MockSetup(context, new CreateCompanyCommand("Name", null, null, null, null));
        var command = new UpdateCompanyCommand(toBeUpdatedCompanyId, "Name", new string('A', 250), null, null, null);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Industry);
    }

    // Location validation
    [Fact]
    public async Task Validate_LocationExceedsMaxLength_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (validator, toBeUpdatedCompanyId) =
            await MockSetup(context, new CreateCompanyCommand("Name", null, null, null, null));
        var command = new UpdateCompanyCommand(toBeUpdatedCompanyId, "Name", null, new string('A', 250), null, null);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Location);
    }

    // Website validation
    [Fact]
    public async Task Validate_WebsiteExceedsMaxLength_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (validator, toBeUpdatedCompanyId) =
            await MockSetup(context, new CreateCompanyCommand("Name", null, null, null, null));
        var command = new UpdateCompanyCommand(toBeUpdatedCompanyId, "Name", null, null, new string('A', 250), null);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Website);
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
    public async Task Validate_InvalidWebsiteUrl_FailsValidation(string invalidUrl, bool expected)
    {
        await using var context = Helpers.CreateInMemoryContext();
        var (validator, toBeUpdatedCompanyId) =
            await MockSetup(context, new CreateCompanyCommand("Name", null, null, null, null));
        var command = new UpdateCompanyCommand(toBeUpdatedCompanyId, "Name", null, null, invalidUrl, null);

        var result = await validator.TestValidateAsync(command);

        if (!expected)
            result.ShouldHaveValidationErrorFor(c => c.Website);
        else result.ShouldNotHaveAnyValidationErrors();
    }
}