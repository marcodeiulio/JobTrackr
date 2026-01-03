using JobTrackr.Application.Companies.Commands.CreateCompany;
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

    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateCompanyCommand command)
    {
        var companyId = await _mediator.Send(command);

        return Ok(companyId);
        // todo return location header
        // return CreatedAtAction(nameof(GetById), new { id = companyId }, companyId);
    }
}