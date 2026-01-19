using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using JobTrackr.Application.Common.Interfaces;
using JobTrackr.Application.JobApplications.Commands.CreateJobApplication;
using JobTrackr.Domain.Entities;
using NSubstitute;
using Xunit;

namespace JobTrackr.Application.Tests.JobApplications.Commands;

public class CreateJobApplicationCommandHandlerTests
{
    private readonly CreateJobApplicationCommandHandler _handler;
    private readonly IApplicationDbContext _mockContext;

    public CreateJobApplicationCommandHandlerTests()
    {
        _mockContext = Substitute.For<IApplicationDbContext>();
        _handler = new CreateJobApplicationCommandHandler(_mockContext);
    }

    private static CreateJobApplicationCommand CreateJobApplicationCommandBuilder(
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
        return new CreateJobApplicationCommand(
            position, description, appliedDate, location, jobUrl, coverLetter, notes, companyId, jobApplicationStatusId
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsNotEmptyGuid()
    {
        var command =
            CreateJobApplicationCommandBuilder(companyId: Guid.NewGuid(), jobApplicationStatusId: Guid.NewGuid());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesJobApplicationAndCallsSaveChangesAsync()
    {
        var position = "Test Position";
        var description = "Description";
        var appliedDate = DateTime.UtcNow;
        var location = "Location";
        var url = "ulr.com";
        var coverLetter = "Cover Letter";
        var notes = "Notes";
        var companyId = Guid.NewGuid();
        var statusId = Guid.NewGuid();
        var command = CreateJobApplicationCommandBuilder(
            position, description, appliedDate, location, url, coverLetter, notes, companyId, statusId);
        JobApplication? capturedJobApplication = null;
        _mockContext.JobApplications.Add(Arg.Do<JobApplication>(j => capturedJobApplication = j));

        await _handler.Handle(command, CancellationToken.None);

        using (new AssertionScope())
        {
            await _mockContext.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
            capturedJobApplication.Should().NotBeNull();
            capturedJobApplication.Position.Should().Be(position);
            capturedJobApplication.Description.Should().Be(description);
            capturedJobApplication.AppliedDate.Should().Be(appliedDate);
            capturedJobApplication.Location.Should().Be(location);
            capturedJobApplication.JobUrl.Should().Be(url);
            capturedJobApplication.CoverLetter.Should().Be(coverLetter);
            capturedJobApplication.Notes.Should().Be(notes);
            capturedJobApplication.CompanyId.Should().Be(companyId);
            capturedJobApplication.JobApplicationStatusId.Should().Be(statusId);
            capturedJobApplication.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }
    }
}