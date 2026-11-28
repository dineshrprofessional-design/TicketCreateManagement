using Microsoft.EntityFrameworkCore;
using TrainBooking.Api.Data;
using TrainBooking.Api.DTOs;
using TrainBooking.Api.Enums;
using TrainBooking.Api.Models;
using TrainBooking.Api.Services.Exceptions;

namespace TrainBooking.Api.Services;

public class TrainService : ITrainService
{
    private readonly AppDbContext _db;

    public TrainService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<TrainResponse> CreateAsync(CreateTrainRequest request, CancellationToken ct = default)
    {
        var exists = await _db.Trains.AnyAsync(t => t.TrainNumber == request.TrainNumber, ct);
        if (exists)
            throw new ConflictException($"A train with number '{request.TrainNumber}' already exists");

        var train = new Train
        {
            TrainNumber = request.TrainNumber,
            TrainName = request.TrainName,
            Source = request.Source,
            Destination = request.Destination,
            Capacity = request.Capacity
        };

        _db.Trains.Add(train);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbExceptionHelpers.IsUniqueViolation(ex))
        {
            throw new ConflictException($"A train with number '{request.TrainNumber}' already exists");
        }

        return MapToResponse(train);
    }

    public async Task<IReadOnlyList<TrainResponse>> GetAllAsync(CancellationToken ct = default)
    {
        var trains = await _db.Trains.OrderBy(t => t.Id).ToListAsync(ct);
        return trains.Select(MapToResponse).ToList();
    }

    public async Task<TrainResponse> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var train = await _db.Trains.FindAsync(new object?[] { id }, ct)
            ?? throw new NotFoundException($"Train {id} not found");

        return MapToResponse(train);
    }

    public async Task<TrainResponse> UpdateAsync(int id, UpdateTrainRequest request, CancellationToken ct = default)
    {
        var train = await _db.Trains.FindAsync(new object?[] { id }, ct)
            ?? throw new NotFoundException($"Train {id} not found");

        if (train.TrainNumber != request.TrainNumber)
        {
            var collides = await _db.Trains.AnyAsync(t => t.Id != id && t.TrainNumber == request.TrainNumber, ct);
            if (collides)
                throw new ConflictException($"A train with number '{request.TrainNumber}' already exists");
        }

        var maxBookedSeat = await _db.Tickets
            .Where(t => t.TrainId == id && t.Status == TicketStatus.Booked)
            .MaxAsync(t => (int?)t.SeatNumber, ct) ?? 0;

        if (request.Capacity < maxBookedSeat)
            throw new ConflictException($"Cannot reduce capacity below seat {maxBookedSeat}, which is currently booked");

        train.TrainNumber = request.TrainNumber;
        train.TrainName = request.TrainName;
        train.Source = request.Source;
        train.Destination = request.Destination;
        train.Capacity = request.Capacity;

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbExceptionHelpers.IsUniqueViolation(ex))
        {
            throw new ConflictException($"A train with number '{request.TrainNumber}' already exists");
        }

        return MapToResponse(train);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var train = await _db.Trains.FindAsync(new object?[] { id }, ct)
            ?? throw new NotFoundException($"Train {id} not found");

        var hasTicketHistory = await _db.Tickets.AnyAsync(t => t.TrainId == id, ct);
        if (hasTicketHistory)
            throw new ConflictException($"Cannot delete train {id}: it has existing ticket history");

        _db.Trains.Remove(train);
        await _db.SaveChangesAsync(ct);
    }

    private static TrainResponse MapToResponse(Train train) => new()
    {
        Id = train.Id,
        TrainNumber = train.TrainNumber,
        TrainName = train.TrainName,
        Source = train.Source,
        Destination = train.Destination,
        Capacity = train.Capacity
    };
}
