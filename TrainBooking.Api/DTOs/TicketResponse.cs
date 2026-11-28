using TrainBooking.Api.Enums;

namespace TrainBooking.Api.DTOs;

public class TicketResponse
{
    public int Id { get; set; }
    public int TrainId { get; set; }
    public string TrainNumber { get; set; } = null!;
    public string PassengerName { get; set; } = null!;
    public int SeatNumber { get; set; }
    public TicketStatus Status { get; set; }
    public DateTime BookedAtUtc { get; set; }
    public DateTime? CancelledAtUtc { get; set; }
}
