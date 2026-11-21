using Microsoft.AspNetCore.Mvc;
using TrainBooking.Api.DTOs;
using TrainBooking.Api.Services;

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
    public async Task<ActionResult<TicketResponse>> Book(BookTicketRequest request, CancellationToken ct)
    {
        var ticket = await _ticketService.BookAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TicketResponse>> GetById(int id, CancellationToken ct)
        => Ok(await _ticketService.GetByIdAsync(id, ct));

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TicketResponse>>> GetByTrain([FromQuery] int trainId, CancellationToken ct)
        => Ok(await _ticketService.GetByTrainAsync(trainId, ct));

    [HttpPost("{id:int}/cancel")]
    public async Task<ActionResult<TicketResponse>> Cancel(int id, CancellationToken ct)
        => Ok(await _ticketService.CancelAsync(id, ct));
}
