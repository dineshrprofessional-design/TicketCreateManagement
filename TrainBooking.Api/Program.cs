using Microsoft.EntityFrameworkCore;
using TrainBooking.Api.Data;
using TrainBooking.Api.Middleware;
using TrainBooking.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("TrainBookingDb")));

builder.Services.AddScoped<ITrainService, TrainService>();
builder.Services.AddScoped<ITicketService, TicketService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseMiddleware<ApiExceptionMiddleware>();

// Configure Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
