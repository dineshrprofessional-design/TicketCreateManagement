using Microsoft.AspNetCore.Mvc;
using TrainBooking.Api.DTOs;
using TrainBooking.Api.Services;
using TrainBooking.Api.Services.Exceptions;

namespace TrainBooking.Api.Controllers;

[ApiController]
[Route("api/tickets")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TicketResponse>> Book(BookTicketRequest request, CancellationToken ct)
    {
        var ticket = await _ticketService.BookAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketResponse>> GetById(int id, CancellationToken ct)
        => Ok(await _ticketService.GetByIdAsync(id, ct));

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TicketResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<TicketResponse>>> GetByTrain([FromQuery] int trainId, CancellationToken ct)
    {
        if (trainId <= 0)
            throw new BadRequestException("trainId query parameter is required and must be a positive integer");

        return Ok(await _ticketService.GetByTrainAsync(trainId, ct));
    }

    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TicketResponse>> Cancel(int id, CancellationToken ct)
        => Ok(await _ticketService.CancelAsync(id, ct));
}
