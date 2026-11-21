namespace TrainBooking.Api.Models;

public class Train
{
    public int Id { get; set; }
    public string TrainNumber { get; set; } = null!;
    public string TrainName { get; set; } = null!;
    public string Source { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public int Capacity { get; set; }

    public List<Ticket> Tickets { get; set; } = new();
}
