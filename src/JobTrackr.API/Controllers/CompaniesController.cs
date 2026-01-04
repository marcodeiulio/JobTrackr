using JobTrackr.Application.Companies.Commands.CreateCompany;
using JobTrackr.Application.Companies.Commands.DeleteCompany;
using JobTrackr.Application.Companies.Commands.UpdateCompany;
using JobTrackr.Application.Companies.DTOs;
using JobTrackr.Application.Companies.Queries.GetCompanies;
using JobTrackr.Application.Companies.Queries.GetCompanyById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JobTrackr.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CompaniesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<CompanyDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var companies = await _mediator.Send(new GetCompaniesQuery(), cancellationToken);

        return Ok(companies);
    }

    [HttpGet("{id}")]
    [ActionName(nameof(GetByIdAsync))]
    public async Task<ActionResult<CompanyDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var company = await _mediator.Send(new GetCompanyByIdQuery(id), cancellationToken);

        if (company is null)
            return NotFound();

        return Ok(company);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateCompanyCommand command,
        CancellationToken cancellationToken = default)
    {
        var companyId = await _mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = companyId }, companyId);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(Guid id, UpdateCompanyCommand command,
        CancellationToken cancellationToken = default)
    {
        if (id != command.Id)
            return BadRequest();

        await _mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteCompanyCommand(id), cancellationToken);

        return NoContent();
    }
}