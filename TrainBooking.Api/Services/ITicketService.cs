using TrainBooking.Api.DTOs;

namespace TrainBooking.Api.Services;

public interface ITicketService
{
    Task<TicketResponse> BookAsync(BookTicketRequest request, CancellationToken ct = default);
    Task<TicketResponse> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<TicketResponse>> GetByTrainAsync(int trainId, CancellationToken ct = default);
    Task<TicketResponse> CancelAsync(int id, CancellationToken ct = default);
}
