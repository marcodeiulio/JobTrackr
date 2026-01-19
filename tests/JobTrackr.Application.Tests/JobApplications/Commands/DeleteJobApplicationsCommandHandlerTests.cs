using FluentAssertions;
using FluentAssertions.Execution;
using JobTrackr.Application.Common.Interfaces;
using JobTrackr.Application.JobApplications.Commands.DeleteJobApplication;
using JobTrackr.Application.Tests.Common.Builders;
using JobTrackr.Domain.Entities;
using JobTrackr.Domain.Exceptions;
using MediatR;
using NSubstitute;

namespace JobTrackr.Application.Tests.JobApplications.Commands;

public class DeleteJobApplicationHandlerTests
{
    private readonly DeleteJobApplicationCommandHandler _handler;
    private readonly IApplicationDbContext _mockContext;

    public DeleteJobApplicationHandlerTests()
    {
        _mockContext = Substitute.For<IApplicationDbContext>();
        _handler = new DeleteJobApplicationCommandHandler(_mockContext);
    }

    [Fact]
    public async Task Handle_ValidCommand_RemovesSavesAndReturnsUnit()
    {
        var existingJobApplication = new JobApplicationBuilder().Build();
        _mockContext.JobApplications.FindAsync(existingJobApplication.Id, Arg.Any<CancellationToken>())
            .Returns(existingJobApplication);
        var command = new DeleteJobApplicationCommand(existingJobApplication.Id);

        var result = await _handler.Handle(command, CancellationToken.None);

        using (new AssertionScope())
        {
            _mockContext.JobApplications.Received(1).Remove(existingJobApplication);
            await _mockContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
            result.Should().Be(Unit.Value);
        }
    }

    [Fact]
    public async Task Handle_ValidCommand_ThrowsNotFoundExceptionWithCorrectProperties()
    {
        var nonExistingId = Guid.NewGuid();
        var command = new DeleteJobApplicationCommand(nonExistingId);

        var act = async () => await _handler.Handle(command, CancellationToken.None);

        using (new AssertionScope())
        {
            var exception = await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"*{nonExistingId}*");
            exception.Which.EntityName.Should().Be(nameof(JobApplication));
            exception.Which.Key.Should().Be(nonExistingId);
        }
    }
}