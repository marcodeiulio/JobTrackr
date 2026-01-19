using FluentAssertions;
using FluentAssertions.Execution;
using JobTrackr.Application.Common.Interfaces;
using JobTrackr.Application.JobApplications.Commands.UpdateJobApplication;
using JobTrackr.Application.Tests.Common.Builders;
using JobTrackr.Domain.Entities;
using JobTrackr.Domain.Exceptions;
using NSubstitute;

namespace JobTrackr.Application.Tests.JobApplications.Commands;

public class UpdateJobApplicationCommandHandlerTests
{
    private readonly UpdateJobApplicationCommandHandler _handler;
    private readonly IApplicationDbContext _mockContext;

    public UpdateJobApplicationCommandHandlerTests()
    {
        _mockContext = Substitute.For<IApplicationDbContext>();
        _handler = new UpdateJobApplicationCommandHandler(_mockContext);
    }

    private static UpdateJobApplicationCommand UpdateJobApplicationCommandBuilder(
        Guid id = default,
        string position = "Test Position",
        string? description = null,
        DateTime? appliedDate = null,
        string? location = null,
        string? jobUrl = null,
        string? coverLetter = null,
        string? notes = null,
        Guid companyId = default,
        Guid jobApplicationStatusId = default
    )
    {
        return new UpdateJobApplicationCommand(
            id, position, description, appliedDate, location, jobUrl, coverLetter, notes, companyId,
            jobApplicationStatusId
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesJobApplicationAndCallsSaveChanges()
    {
        var existingJobApplication = new JobApplicationBuilder().Build();
        _mockContext.JobApplications.FindAsync(existingJobApplication.Id, Arg.Any<CancellationToken>())
            .Returns(existingJobApplication);
        var id = existingJobApplication.Id;
        var position = "New Position";
        var description = "New Description";
        var appliedDate = DateTime.UtcNow;
        var location = "New Location";
        var url = "newulr.com";
        var coverLetter = "New Cover Letter";
        var notes = "New Notes";
        var companyId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        var command = UpdateJobApplicationCommandBuilder(
            id, position, description, appliedDate, location, url, coverLetter, notes, companyId, statusId);

        await _handler.Handle(command, CancellationToken.None);

        using (new AssertionScope())
        {
            existingJobApplication.Should().NotBeNull();
            existingJobApplication.Position.Should().Be(position);
            existingJobApplication.Description.Should().Be(description);
            existingJobApplication.AppliedDate.Should().Be(appliedDate);
            existingJobApplication.Location.Should().Be(location);
            existingJobApplication.JobUrl.Should().Be(url);
            existingJobApplication.CoverLetter.Should().Be(coverLetter);
            existingJobApplication.Notes.Should().Be(notes);
            existingJobApplication.CompanyId.Should().Be(companyId);
            existingJobApplication.JobApplicationStatusId.Should().Be(statusId);
            existingJobApplication.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            await _mockContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
        }
    }

    [Fact]
    public async Task Handle_ValidCommand_ThrowsNotFoundExceptionWithCorrectProperties()
    {
        var nonExistingId = Guid.NewGuid();
        _mockContext.JobApplications
            .FindAsync(Arg.Any<object[]>(),
                Arg.Any<CancellationToken>())
            .Returns((JobApplication?)null);
        var command = UpdateJobApplicationCommandBuilder(nonExistingId);

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