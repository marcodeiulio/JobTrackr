using FluentAssertions;
using FluentAssertions.Execution;
using JobTrackr.Application.Companies.Queries.GetCompanyById;
using JobTrackr.Application.Tests.Common;
using JobTrackr.Domain.Entities;
using JobTrackr.Domain.Exceptions;

namespace JobTrackr.Application.Tests.Companies.Queries;

public class GetCompanyByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ExistingCompany_ReturnsDtoWithCorrectProperties()
    {
        await using var mockContext = Helpers.CreateInMemoryContext();
        var company = Company.Create("Company", "Industry", "Location", "https://website.com", "Notes");
        mockContext.Companies.Add(company);
        await mockContext.SaveChangesAsync();

        var handler = new GetCompanyByIdQueryHandler(mockContext);
        var query = new GetCompanyByIdQuery(company.Id);

        var dto = await handler.Handle(query, CancellationToken.None);

        using (new AssertionScope())
        {
            dto.Id.Should().Be(company.Id);
            dto.Name.Should().Be(company.Name);
            dto.Industry.Should().Be(company.Industry);
            dto.Location.Should().Be(company.Location);
            dto.Website.Should().Be(company.Website);
            dto.Notes.Should().Be(company.Notes);
        }
    }

    [Fact]
    public async Task Handle_NonExistingCompany_ThrowsNotFoundExceptionWithCorrectId()
    {
        await using var mockContext = Helpers.CreateInMemoryContext();
        var nonExistentId = Guid.NewGuid();
        var handler = new GetCompanyByIdQueryHandler(mockContext);
        var query = new GetCompanyByIdQuery(nonExistentId);

        using (new AssertionScope())
        {
            var exception = await handler.Invoking(h => h.Handle(query, CancellationToken.None))
                .Should().ThrowAsync<NotFoundException>();
            exception.Which.Key.Should().Be(nonExistentId);
        }
    }
}