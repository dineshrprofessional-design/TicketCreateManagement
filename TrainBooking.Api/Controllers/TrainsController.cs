using Microsoft.AspNetCore.Mvc;
using TrainBooking.Api.DTOs;
using TrainBooking.Api.Services;

namespace TrainBooking.Api.Controllers;

[ApiController]
[Route("api/trains")]
public class TrainsController : ControllerBase
{
    private readonly ITrainService _trainService;

    public TrainsController(ITrainService trainService)
    {
        _trainService = trainService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(TrainResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TrainResponse>> Create(CreateTrainRequest request, CancellationToken ct)
    {
        var train = await _trainService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = train.Id }, train);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TrainResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TrainResponse>>> GetAll(CancellationToken ct)
        => Ok(await _trainService.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TrainResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TrainResponse>> GetById(int id, CancellationToken ct)
        => Ok(await _trainService.GetByIdAsync(id, ct));

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TrainResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TrainResponse>> Update(int id, UpdateTrainRequest request, CancellationToken ct)
        => Ok(await _trainService.UpdateAsync(id, request, ct));

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _trainService.DeleteAsync(id, ct);
        return NoContent();
    }
}
