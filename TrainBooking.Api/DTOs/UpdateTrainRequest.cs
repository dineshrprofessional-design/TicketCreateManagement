using System.ComponentModel.DataAnnotations;

namespace TrainBooking.Api.DTOs;

public class UpdateTrainRequest
{
    [Required, MaxLength(20)]
    public string TrainNumber { get; set; } = null!;

    [Required, MaxLength(100)]
    public string TrainName { get; set; } = null!;

    [Required, MaxLength(100)]
    public string Source { get; set; } = null!;

    [Required, MaxLength(100)]
    public string Destination { get; set; } = null!;

    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1")]
    public int Capacity { get; set; }
}
