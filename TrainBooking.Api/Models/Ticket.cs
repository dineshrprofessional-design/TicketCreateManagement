using TrainBooking.Api.Enums;

namespace TrainBooking.Api.Models;

public class Ticket
{
    public int Id { get; set; }
    public int TrainId { get; set; }
    public Train Train { get; set; } = null!;
    public string PassengerName { get; set; } = null!;
    public int SeatNumber { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Booked;
    public DateTime BookedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CancelledAtUtc { get; set; }
}
