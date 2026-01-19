using FluentValidation.TestHelper;
using JobTrackr.Application.Companies.Commands.CreateCompany;
using JobTrackr.Application.Tests.Common;
using JobTrackr.Domain.Entities;

namespace JobTrackr.Application.Tests.Companies.Commands;

public class CreateCompanyCommandValidatorTests
{
    [Fact]
    public async Task Validate_ValidCompany_PassesValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var validator = new CreateCompanyCommandValidator(context);
        var command = new CreateCompanyCommand("Name", "Industry", "Location", "https://www.website.com", "Long notes");

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_ValidCompanyWithNameOnly_PassesValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var validator = new CreateCompanyCommandValidator(context);
        var command = new CreateCompanyCommand("Name", null, null, null, null);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_ValidCompanyWithEmptyStringParams_PassesValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();
        var validator = new CreateCompanyCommandValidator(context);
        var command = new CreateCompanyCommand("Name", "", "", "", "");

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    // name validation
    [Fact]
    public async Task Validate_DuplicateName_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();

        var existingCompany = Company.Create("Existing company", null, null, null, null);

        context.Companies.Add(existingCompany);
        await context.SaveChangesAsync(CancellationToken.None);

        var validator = new CreateCompanyCommandValidator(context);
        var command = new CreateCompanyCommand("Existing company", null, null, null, null);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public async Task Validate_EmptyName_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();

        var validator = new CreateCompanyCommandValidator(context);
        var command = new CreateCompanyCommand("", null, null, null, null);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Fact]
    public async Task Validate_NameExceedsMaxLength_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();

        var validator = new CreateCompanyCommandValidator(context);
        var command = new CreateCompanyCommand(
            new string('A', 250),
            null, null, null, null);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    // industry validation
    [Fact]
    public async Task Validate_IndustryExceedsMaxLength_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();

        var validator = new CreateCompanyCommandValidator(context);
        var command = new CreateCompanyCommand(
            "Name",
            new string('A', 250),
            null, null, null);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Industry);
    }

    // location validation
    [Fact]
    public async Task Validate_LocationExceedsMaxLength_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();

        var validator = new CreateCompanyCommandValidator(context);
        var command = new CreateCompanyCommand(
            "Name", null,
            new string('A', 250),
            null, null);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Location);
    }

    // website validation
    [Fact]
    public async Task Validate_WebsiteExceedsMaxLength_FailsValidation()
    {
        await using var context = Helpers.CreateInMemoryContext();

        var validator = new CreateCompanyCommandValidator(context);
        var command = new CreateCompanyCommand(
            "Name", null, null,
            new string('A', 600),
            null);

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
    public async Task Validate_UrlVariants_FailsOrPassesValidation(string url, bool expected)
    {
        await using var context = Helpers.CreateInMemoryContext();

        var validator = new CreateCompanyCommandValidator(context);
        var command = new CreateCompanyCommand(
            "Name", null, null,
            url,
            null);

        var result = await validator.TestValidateAsync(command);

        if (expected)
            result.ShouldNotHaveAnyValidationErrors();
        else
            result.ShouldHaveValidationErrorFor(c => c.Website);
    }
}