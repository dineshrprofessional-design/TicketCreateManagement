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
    public async Task<ActionResult<TrainResponse>> Create(CreateTrainRequest request, CancellationToken ct)
    {
        var train = await _trainService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = train.Id }, train);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<TrainResponse>>> GetAll(CancellationToken ct)
        => Ok(await _trainService.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TrainResponse>> GetById(int id, CancellationToken ct)
        => Ok(await _trainService.GetByIdAsync(id, ct));

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TrainResponse>> Update(int id, UpdateTrainRequest request, CancellationToken ct)
        => Ok(await _trainService.UpdateAsync(id, request, ct));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _trainService.DeleteAsync(id, ct);
        return NoContent();
    }
}
