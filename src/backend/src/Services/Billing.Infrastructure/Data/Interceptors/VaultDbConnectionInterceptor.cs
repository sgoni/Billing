

namespace Billing.Infrastructure.Data.Interceptors;

public class VaultDbConnectionInterceptor : DbConnectionInterceptor
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<VaultDbConnectionInterceptor> _logger;
    private readonly ISecretManager _secretManager;

    public VaultDbConnectionInterceptor(ISecretManager secretManager, IConfiguration configuration,
        ILogger<VaultDbConnectionInterceptor> logger)
    {
        _secretManager = secretManager;
        _configuration = configuration;
        _logger = logger;
    }

    public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result,
        CancellationToken cancellationToken = new())

    {
        _logger.LogInformation("Obtaining dynamic credentials from Vault ...");

        var server = _configuration["DatabaseConfig:server"];
        var port = _configuration["DatabaseConfig:port"];
        var database = _configuration["DatabaseConfig:database"];

        var creds = await _secretManager.GetPostgreSQLCredential<UsernamePasswordCredentials>();

        connection.ConnectionString =
            $"Server={server};Port={port};Database={database};User Id={creds.Username};Password={creds.Password};Include Error Detail=true";

        _logger.LogInformation("BD credentials dynamically updated from Vault for {database}", database);
        Console.WriteLine($"🔐 Vault interceptor injecting user: {creds.Username}");
        return await base.ConnectionOpeningAsync(connection, eventData, result, cancellationToken);
    }
}