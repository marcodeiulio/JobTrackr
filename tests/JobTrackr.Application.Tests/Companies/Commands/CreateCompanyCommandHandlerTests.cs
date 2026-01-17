using FluentAssertions;
using FluentAssertions.Execution;
using JobTrackr.Application.Common.Interfaces;
using JobTrackr.Application.Companies.Commands.CreateCompany;
using JobTrackr.Domain.Entities;
using NSubstitute;

namespace JobTrackr.Application.Tests.Companies.Commands;

public class CreateCompanyCommandHandlerTests
{
    private readonly CreateCompanyCommandHandler _handler;
    private readonly IApplicationDbContext _mockContext;

    public CreateCompanyCommandHandlerTests()
    {
        _mockContext = Substitute.For<IApplicationDbContext>();
        _handler = new CreateCompanyCommandHandler(_mockContext);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsId()
    {
        var command = new CreateCompanyCommand("Test Name", null, null, null, null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_ValidCommand_SavesCompany()
    {
        var command = new CreateCompanyCommand("Test Name", null, null, null, null);

        var result = await _handler.Handle(command, CancellationToken.None);

        await _mockContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesCompanyWithCorrectProperties()
    {
        var command = new CreateCompanyCommand("Test Name", "Industry", "Location", "https://website.com", "Notes");

        Company? capturedCompany = null;
        _mockContext.Companies.Add(Arg.Do<Company>(c => capturedCompany = c));

        await _handler.Handle(command, CancellationToken.None);

        using (new AssertionScope())
        {
            capturedCompany.Should().NotBeNull();
            capturedCompany.Name.Should().Be("Test Name");
            capturedCompany.Industry.Should().Be("Industry");
            capturedCompany.Location.Should().Be("Location");
            capturedCompany.Website.Should().Be("https://website.com");
            capturedCompany.Notes.Should().Be("Notes");
            capturedCompany.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }
}