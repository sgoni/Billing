namespace Billing.Infrastructure.Data.Extensions;

public static class DatabaseExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
        await SeedAsync(context);
    }

    private static async Task SeedAsync(ApplicationDbContext context)
    {
        await SeedInvoicesAsync(context);
    }

    private static async Task SeedInvoicesAsync(ApplicationDbContext context)
    {
        if (!await context.Invoices.AnyAsync())
        {
            await context.Invoices.AddRangeAsync(InitialData.InvoicesWithLines);
            await context.SaveChangesAsync();
        }
    }
}