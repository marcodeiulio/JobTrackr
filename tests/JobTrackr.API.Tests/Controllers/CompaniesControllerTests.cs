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
    public async Task GetCompanies_WithoutAuth_Returns401()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "api/companies");
        request.Headers.Add("X-Test-Skip-Auth", "true");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }


    [Fact]
    public async Task GetCompanies_ReturnsEmptyList()
    {
        var response = await _client.GetAsync("api/companies");
        var content = await response.Content.ReadFromJsonAsync<List<CompanyDto>>();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
        content.Should().BeEmpty();
    }

    [Fact]
    public async Task PostCompany_WithoutAuth_Returns401()
    {
        var company = new { Name = "TestCompany" };
        var request = new HttpRequestMessage(HttpMethod.Post, "api/companies")
        {
            Content = JsonContent.Create(company)
        };
        request.Headers.Add("X-Test-Skip-Auth", "true");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}