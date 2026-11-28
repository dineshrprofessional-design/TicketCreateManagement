using Microsoft.EntityFrameworkCore;
using TrainBooking.Api.Enums;
using TrainBooking.Api.Models;

namespace TrainBooking.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Train> Trains => Set<Train>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Train>(entity =>
        {
            entity.Property(t => t.TrainNumber).IsRequired().HasMaxLength(20);
            entity.Property(t => t.TrainName).IsRequired().HasMaxLength(100);
            entity.Property(t => t.Source).IsRequired().HasMaxLength(100);
            entity.Property(t => t.Destination).IsRequired().HasMaxLength(100);

            entity.HasIndex(t => t.TrainNumber).IsUnique();
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.Property(t => t.PassengerName).IsRequired().HasMaxLength(100);
            entity.Property(t => t.Status).HasConversion<int>();

            entity.HasOne(t => t.Train)
                  .WithMany(tr => tr.Tickets)
                  .HasForeignKey(t => t.TrainId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(t => new { t.TrainId, t.SeatNumber })
                  .IsUnique()
                  .HasFilter($"\"{nameof(Ticket.Status)}\" = {(int)TicketStatus.Booked}")
                  .HasDatabaseName("IX_Tickets_TrainId_SeatNumber_ActiveOnly");
        });
    }
}
