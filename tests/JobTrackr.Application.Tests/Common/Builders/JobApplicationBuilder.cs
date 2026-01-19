using System;
using JobTrackr.Domain.Entities;

namespace JobTrackr.Application.Tests.Common.Builders;

public class JobApplicationBuilder
{
    private DateTime? _appliedDate;
    private Guid _companyId = Guid.NewGuid();
    private string? _coverLetter;
    private string? _description;
    private Guid _jobApplicationStatusId = Guid.NewGuid();
    private string? _jobUrl;
    private string? _location;
    private string? _notes;
    private string _position = "Test Position";

    public JobApplicationBuilder WithPosition(string position)
    {
        _position = position;
        return this;
    }

    public JobApplicationBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public JobApplicationBuilder WithAppliedDate(DateTime? appliedDate)
    {
        _appliedDate = appliedDate;
        return this;
    }

    public JobApplicationBuilder WithLocation(string? location)
    {
        _location = location;
        return this;
    }

    public JobApplicationBuilder WithJobUrl(string? jobUrl)
    {
        _jobUrl = jobUrl;
        return this;
    }

    public JobApplicationBuilder WithCoverLetter(string? coverLetter)
    {
        _coverLetter = coverLetter;
        return this;
    }

    public JobApplicationBuilder WithNotes(string? notes)
    {
        _notes = notes;
        return this;
    }

    public JobApplicationBuilder WithCompanyId(Guid companyId)
    {
        _companyId = companyId;
        return this;
    }

    public JobApplicationBuilder WithStatusId(Guid statusId)
    {
        _jobApplicationStatusId = statusId;
        return this;
    }

    public JobApplication Build()
    {
        return JobApplication.Create(
            _position,
            _description,
            _appliedDate,
            _location,
            _jobUrl,
            _coverLetter,
            _notes,
            _companyId,
            _jobApplicationStatusId
        );
    }
}