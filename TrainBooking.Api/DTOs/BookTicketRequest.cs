using System.ComponentModel.DataAnnotations;

namespace TrainBooking.Api.DTOs;

public class BookTicketRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "TrainId must reference a valid train")]
    public int TrainId { get; set; }

    [Required, MaxLength(100)]
    public string PassengerName { get; set; } = null!;

    [Range(1, int.MaxValue, ErrorMessage = "SeatNumber must be a positive integer when specified")]
    public int? SeatNumber { get; set; }
}
