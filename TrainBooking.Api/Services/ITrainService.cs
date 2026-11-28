using TrainBooking.Api.DTOs;

namespace TrainBooking.Api.Services;

public interface ITrainService
{
    Task<TrainResponse> CreateAsync(CreateTrainRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<TrainResponse>> GetAllAsync(CancellationToken ct = default);
    Task<TrainResponse> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TrainResponse> UpdateAsync(int id, UpdateTrainRequest request, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
