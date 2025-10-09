using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Seeder;

public class MigrateDatabase
{
    public static async Task MigrateAsynce(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var dbContext = services.GetRequiredService<EVSwappingV2Context>();
            await dbContext.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            // Log any errors during migration
            var logger = services.GetRequiredService<ILogger<MigrateDatabase>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }
}
