internal class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services
            .AddApplicationServices(builder.Configuration)
            .AddInfrastructureServices(builder.Configuration)
            .AddApiServices(builder.Configuration, Assembly.GetExecutingAssembly());

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Host.UseSerilog();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseApiServices();

        if (app.Environment.IsDevelopment())
        {
            await app.InitialiseDatabaseAsync();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.Run();
    }
}