using MachinesDataAnalyzingSystem.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace MachinesDataAnalyzingSystem.WebAPI.Extensions;

public static class MigrationsExtension
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }
}