namespace Billing.Infrastructure.Data.Interceptors;

public class VaultDbConnectionInterceptor : DbConnectionInterceptor
{
    private readonly VaultCredentialManager _credentialManager;
    private readonly IConfiguration _configuration;
    private readonly ISecretManager _secretManager;
    private readonly IServiceDiscovery _discovery;
    private readonly ILogger<VaultDbConnectionInterceptor> _logger;

    public VaultDbConnectionInterceptor(
        VaultCredentialManager credentialManager,
        ISecretManager secretManager,
        IServiceDiscovery discovery,
        IConfiguration configuration,
        ILogger<VaultDbConnectionInterceptor> logger
    )
    {
        _secretManager = secretManager;
        _discovery = discovery;
        _configuration = configuration;
        _credentialManager = credentialManager;
        _logger = logger;
    }

    public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(
        DbConnection connection,
        ConnectionEventData eventData,
        InterceptionResult result,
        CancellationToken cancellationToken = new())

    {
        _logger.LogInformation("Resolving DB connection via Consul + Vault...");

        var (host, port) = await _discovery.GetServiceAsync(_configuration["DatabaseConfig:serviceName"]);

        /*
         * === Implementacion anterior ====
         * var creds = await _secretManager.GetPostgreSQLCredential<UsernamePasswordCredentials>();
         * connection.ConnectionString =
         * $"Host={host};Port={port};Database=billingdb;Username={creds.Username};Password={creds.Password};Include Error Detail=true";
         * _logger.LogInformation("BD credentials dynamically updated from Vault for {database}",
         * _configuration["DatabaseConfig:serviceName"]);
         */

        var (username, password) = await _credentialManager.GetCredentialsAsync();
        connection.ConnectionString =
            $"Host={host};Port={port};Database=billingdb;Username={username};Password={password};Include Error Detail=true";

        _logger.LogInformation("DB connection resolved dynamically → {host}:{port}", host, port);
        Console.WriteLine($"DB connection resolved dynamically → {host}:{port}");
        Console.WriteLine($"🔐 Vault interceptor injecting user: {username}");
        Console.WriteLine($"🔐 Vault interceptor injecting password: {password}");

        return await base.ConnectionOpeningAsync(connection, eventData, result, cancellationToken);
    }
}