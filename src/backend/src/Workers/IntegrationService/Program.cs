var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddScoped<IEventRelayService, EventRelayService>();
builder.Services.AddMessageBroker(builder.Configuration, Assembly.GetExecutingAssembly());

// Add Marten
var connectionString = builder.Configuration.GetValue<string>("ConnectionStrings:Database"!);
builder.Services
    .AddMarten(opts => { opts.Connection(connectionString!); })
    .UseLightweightSessions();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Services
    .AddHealthChecks()
    //.AddRabbitMQ("amqp://localhost", name: "rabbitmq", timeout: TimeSpan.FromSeconds(5))
    .AddApplicationStatus("service_status", tags: new[] { "api" })
    .AddNpgSql(connectionString!,
        name: "pgsql",
        failureStatus: HealthStatus.Degraded,
        tags: new[] { "db", "sql", "pgsql" });

builder.Services.AddHostedService<EventRelayWorker>();
var host = builder.Build();
host.Run();