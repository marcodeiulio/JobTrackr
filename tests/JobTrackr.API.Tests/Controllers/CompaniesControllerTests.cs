using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using JobTrackr.API.Tests.Common;
using JobTrackr.Application.Companies.DTOs;

namespace JobTrackr.API.Tests.Controllers;

public class CompaniesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public CompaniesControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GET_Companies_ReturnsEmptyList()
    {
        var response = await _client.GetAsync("api/companies");
        var content = await response.Content.ReadFromJsonAsync<List<CompanyDto>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Should().BeEmpty();
    }
}