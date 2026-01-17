using FluentAssertions;
using FluentAssertions.Execution;
using JobTrackr.Application.Common.Interfaces;
using JobTrackr.Application.Companies.Commands.UpdateCompany;
using JobTrackr.Domain.Entities;
using JobTrackr.Domain.Exceptions;
using MediatR;
using NSubstitute;

namespace JobTrackr.Application.Tests.Companies.Commands;

public class UpdateCompanyCommandHandlerTests
{
    private readonly UpdateCompanyCommandHandler _handler;
    private readonly IApplicationDbContext _mockContext;

    public UpdateCompanyCommandHandlerTests()
    {
        _mockContext = Substitute.For<IApplicationDbContext>();
        _handler = new UpdateCompanyCommandHandler(_mockContext);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsSaveChangesAsync()
    {
        var existingCompany = Company.Create("Name", null, null, null, null);
        _mockContext.Companies.FindAsync(existingCompany.Id, Arg.Any<CancellationToken>())
            .Returns(existingCompany);
        var command = new UpdateCompanyCommand(existingCompany.Id, "New Name", null, null, null, null);

        await _handler.Handle(command, CancellationToken.None);

        await _mockContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUnitValue()
    {
        var existingCompany = Company.Create("Name", null, null, null, null);
        _mockContext.Companies.FindAsync(existingCompany.Id, Arg.Any<CancellationToken>())
            .Returns(existingCompany);
        var command = new UpdateCompanyCommand(existingCompany.Id, "New Name", null, null, null, null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesCompany()
    {
        var existingCompany = Company.Create("Name", null, null, null, null);
        _mockContext.Companies.FindAsync(existingCompany.Id, Arg.Any<CancellationToken>())
            .Returns(existingCompany);
        var command = new UpdateCompanyCommand(existingCompany.Id, "New Name", "New Industry", "New Location",
            "https://newwebsite.com", "New Notes");

        await _handler.Handle(command, CancellationToken.None);

        using (new AssertionScope())
        {
            existingCompany.Name.Should().Be("New Name");
            existingCompany.Industry.Should().Be("New Industry");
            existingCompany.Location.Should().Be("New Location");
            existingCompany.Website.Should().Be("https://newwebsite.com");
            existingCompany.Notes.Should().Be("New Notes");
            existingCompany.UpdatedAt.Should().BeAfter(existingCompany.CreatedAt);
        }
    }

    [Fact]
    public async Task Handle_NonExistentCompany_ThrowsNotFoundException()
    {
        var command = new UpdateCompanyCommand(Guid.NewGuid(), "New Name", null, null, null, null);

        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }
}