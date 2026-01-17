using FluentAssertions;
using FluentAssertions.Execution;
using JobTrackr.Application.Companies.Queries.GetCompanies;
using JobTrackr.Application.Tests.Common;
using JobTrackr.Domain.Entities;

namespace JobTrackr.Application.Tests.Companies.Queries;

public class GetCompaniesQueryHandlerTests
{
    [Fact]
    public async Task Handle_EmptyDatabase_ReturnsEmptyList()
    {
        await using var mockContext = Helpers.CreateInMemoryContext();
        var handler = new GetCompaniesQueryHandler(mockContext);
        var query = new GetCompaniesQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WithCompanies_ReturnsAllCompanies()
    {
        await using var mockContext = Helpers.CreateInMemoryContext();
        mockContext.Companies.Add(Company.Create("Company One", null, null, null, null));
        mockContext.Companies.Add(Company.Create("Company Two", null, null, null, null));
        await mockContext.SaveChangesAsync();

        var handler = new GetCompaniesQueryHandler(mockContext);
        var query = new GetCompaniesQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_WithCompanies_ReturnDtoWithCorrectProperties()
    {
        await using var mockContext = Helpers.CreateInMemoryContext();
        var company = Company.Create("Company One", "Industry", "Location", "https://website.com", "Notes");
        mockContext.Companies.Add(company);
        await mockContext.SaveChangesAsync();

        var handler = new GetCompaniesQueryHandler(mockContext);
        var query = new GetCompaniesQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        using (new AssertionScope())
        {
            var dto = result.Should().ContainSingle().Subject;
            dto.Id.Should().Be(company.Id);
            dto.Name.Should().Be(company.Name);
            dto.Industry.Should().Be(company.Industry);
            dto.Location.Should().Be(company.Location);
            dto.Website.Should().Be(company.Website);
            dto.Notes.Should().Be(company.Notes);
        }
    }


    [Fact]
    public async Task Handle_WithCompanies_ReturnsAllCompaniesInCorrectOrder()
    {
        await using var mockContext = Helpers.CreateInMemoryContext();
        var companyA = Company.Create("A Company", null, null, null, null);
        var companyB = Company.Create("B Company", null, null, null, null);
        var companyC = Company.Create("C Company", null, null, null, null);
        mockContext.Companies.AddRange(companyC, companyA, companyB); // add in random order
        await mockContext.SaveChangesAsync();

        var handler = new GetCompaniesQueryHandler(mockContext);
        var query = new GetCompaniesQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        using (new AssertionScope())
        {
            result.Should().HaveCount(3);
            result.Select(c => c.Name).Should().Contain(["A Company", "B Company", "C Company"]);
        }
    }
}