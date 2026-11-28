using Microsoft.EntityFrameworkCore;

namespace TrainBooking.Api.Services;

public static class DbExceptionHelpers
{
    public static bool IsUniqueViolation(DbUpdateException ex) =>
        ex.InnerException is Npgsql.PostgresException { SqlState: Npgsql.PostgresErrorCodes.UniqueViolation };
}
