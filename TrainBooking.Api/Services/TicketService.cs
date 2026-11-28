using Microsoft.EntityFrameworkCore;
using TrainBooking.Api.Data;
using TrainBooking.Api.DTOs;
using TrainBooking.Api.Enums;
using TrainBooking.Api.Models;
using TrainBooking.Api.Services.Exceptions;

namespace TrainBooking.Api.Services;

public class TicketService : ITicketService
{
    private readonly AppDbContext _db;

    public TicketService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<TicketResponse> BookAsync(BookTicketRequest request, CancellationToken ct = default)
    {
        var train = await _db.Trains.FindAsync(new object?[] { request.TrainId }, ct)
            ?? throw new NotFoundException($"Train {request.TrainId} not found");

        int seatNumber;

        if (request.SeatNumber is { } requested)
        {
            if (requested < 1 || requested > train.Capacity)
                throw new BadRequestException($"Seat number must be between 1 and {train.Capacity}");

            var taken = await _db.Tickets.AnyAsync(
                t => t.TrainId == train.Id && t.SeatNumber == requested && t.Status == TicketStatus.Booked, ct);
            if (taken)
                throw new ConflictException($"Seat {requested} on train {train.Id} is already booked");

            seatNumber = requested;
        }
        else
        {
            var occupied = (await _db.Tickets
                .Where(t => t.TrainId == train.Id && t.Status == TicketStatus.Booked)
                .Select(t => t.SeatNumber)
                .ToListAsync(ct)).ToHashSet();

            seatNumber = Enumerable.Range(1, train.Capacity).FirstOrDefault(s => !occupied.Contains(s));
            if (seatNumber == 0)
                throw new ConflictException($"Train {train.Id} has no available seats");
        }

        var ticket = new Ticket
        {
            TrainId = train.Id,
            PassengerName = request.PassengerName,
            SeatNumber = seatNumber,
            Status = TicketStatus.Booked,
            BookedAtUtc = DateTime.UtcNow
        };

        _db.Tickets.Add(ticket);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbExceptionHelpers.IsUniqueViolation(ex))
        {
            throw new ConflictException($"Seat {seatNumber} on train {train.Id} was just booked by another request");
        }

        return MapToResponse(ticket, train.TrainNumber);
    }

    public async Task<TicketResponse> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var ticket = await _db.Tickets.Include(t => t.Train).FirstOrDefaultAsync(t => t.Id == id, ct)
            ?? throw new NotFoundException($"Ticket {id} not found");

        return MapToResponse(ticket, ticket.Train.TrainNumber);
    }

    public async Task<IReadOnlyList<TicketResponse>> GetByTrainAsync(int trainId, CancellationToken ct = default)
    {
        var train = await _db.Trains.FindAsync(new object?[] { trainId }, ct)
            ?? throw new NotFoundException($"Train {trainId} not found");

        var tickets = await _db.Tickets
            .Where(t => t.TrainId == trainId)
            .OrderBy(t => t.Id)
            .ToListAsync(ct);

        return tickets.Select(t => MapToResponse(t, train.TrainNumber)).ToList();
    }

    public async Task<TicketResponse> CancelAsync(int id, CancellationToken ct = default)
    {
        var ticket = await _db.Tickets.Include(t => t.Train).FirstOrDefaultAsync(t => t.Id == id, ct)
            ?? throw new NotFoundException($"Ticket {id} not found");

        if (ticket.Status == TicketStatus.Cancelled)
            throw new ConflictException($"Ticket {id} is already cancelled");

        ticket.Status = TicketStatus.Cancelled;
        ticket.CancelledAtUtc = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);

        return MapToResponse(ticket, ticket.Train.TrainNumber);
    }

    private static TicketResponse MapToResponse(Ticket ticket, string trainNumber) => new()
    {
        Id = ticket.Id,
        TrainId = ticket.TrainId,
        TrainNumber = trainNumber,
        PassengerName = ticket.PassengerName,
        SeatNumber = ticket.SeatNumber,
        Status = ticket.Status,
        BookedAtUtc = ticket.BookedAtUtc,
        CancelledAtUtc = ticket.CancelledAtUtc
    };
}
