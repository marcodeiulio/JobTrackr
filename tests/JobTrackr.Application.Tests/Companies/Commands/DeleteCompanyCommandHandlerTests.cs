using FluentAssertions;
using JobTrackr.Application.Common.Interfaces;
using JobTrackr.Application.Companies.Commands.DeleteCompany;
using JobTrackr.Domain.Entities;
using JobTrackr.Domain.Exceptions;
using MediatR;
using NSubstitute;

namespace JobTrackr.Application.Tests.Companies.Commands;

public class DeleteCompanyCommandHandlerTests
{
    private readonly DeleteCompanyCommandHandler _handler;
    private readonly IApplicationDbContext _mockContext;

    public DeleteCompanyCommandHandlerTests()
    {
        _mockContext = Substitute.For<IApplicationDbContext>();
        _handler = new DeleteCompanyCommandHandler(_mockContext);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsSaveChangesAsync()
    {
        var existingCompany = Company.Create("Name", null, null, null, null);
        _mockContext.Companies.FindAsync(existingCompany.Id, Arg.Any<CancellationToken>())
            .Returns(existingCompany);
        var command = new DeleteCompanyCommand(existingCompany.Id);

        await _handler.Handle(command, CancellationToken.None);

        await _mockContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsUnitValue()
    {
        var existingCompany = Company.Create("Name", null, null, null, null);
        _mockContext.Companies.FindAsync(existingCompany.Id, Arg.Any<CancellationToken>())
            .Returns(existingCompany);
        var command = new DeleteCompanyCommand(existingCompany.Id);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task Handle_ValidCommand_RemovesCompany()
    {
        var existingCompany = Company.Create("Name", null, null, null, null);
        _mockContext.Companies.FindAsync(existingCompany.Id, Arg.Any<CancellationToken>())
            .Returns(existingCompany);
        var command = new DeleteCompanyCommand(existingCompany.Id);

        await _handler.Handle(command, CancellationToken.None);

        _mockContext.Companies.Received(1).Remove(existingCompany);
    }

    [Fact]
    public async Task Handle_NonExistentCompany_ThrowsNotFoundException()
    {
        var command = new DeleteCompanyCommand(Guid.NewGuid());

        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }
}