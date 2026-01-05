using FluentAssertions;
using JobTrackr.Application.Common.Interfaces;
using JobTrackr.Application.Companies.Commands.CreateCompany;
using NSubstitute;

namespace JobTrackr.Application.Tests.Companies.Commands;

public class CreateCompanyCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_ReturnsId()
    {
        var mockContext = Substitute.For<IApplicationDbContext>();
        var handler = new CreateCompanyCommandHandler(mockContext);
        var command = new CreateCompanyCommand("Test Name", null, null, null, null);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_ValidCommand_SavesCompany()
    {
        var mockContext = Substitute.For<IApplicationDbContext>();
        var handler = new CreateCompanyCommandHandler(mockContext);
        var command = new CreateCompanyCommand("Test Name", null, null, null, null);

        var result = await handler.Handle(command, CancellationToken.None);

        await mockContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}