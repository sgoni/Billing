public class VaultCredentialManager
{
    private readonly VaultClient _vaultClient;
    private readonly VaultSettings _settings;

    private readonly SemaphoreSlim _lock = new(1, 1);
    private VaultCredentialLease? _currentLease;

    private const double SafetyFactor = 0.8;

    public VaultCredentialManager(IOptions<VaultSettings> options)
    {
        _settings = options.Value with
        {
            TokenApi = Extensions.GetTokenFromEnvironmentVariable()
        };


        var authMethod = new TokenAuthMethodInfo(_settings.TokenApi);
        var settings = new VaultClientSettings(_settings.Address, authMethod);

        _vaultClient = new VaultClient(settings);
    }

    public async Task<(string Username, string Password)> GetCredentialsAsync()
    {
        if (IsLeaseValid())
            return (_currentLease!.Username, _currentLease.Password);

        await _lock.WaitAsync();

        try
        {
            // double-check (thread-safe)
            if (IsLeaseValid())
                return (_currentLease!.Username, _currentLease.Password);

            var secret = await _vaultClient.V1.Secrets.Database
                .GetCredentialsAsync(_settings.Role);

            _currentLease = new VaultCredentialLease
            {
                Username = secret.Data.Username,
                Password = secret.Data.Password,
                LeaseId = secret.LeaseId,
                LeaseDurationSeconds = secret.LeaseDurationSeconds,
                ExpirationUtc = DateTime.UtcNow.AddSeconds(secret.LeaseDurationSeconds)
            };

            return (_currentLease.Username, _currentLease.Password);
        }
        finally
        {
            _lock.Release();
        }
    }

    private bool IsLeaseValid()
    {
        if (_currentLease == null)
            return false;

        var safeExpiration = _currentLease.ExpirationUtc
            .AddSeconds(-_currentLease.LeaseDurationSeconds * (1 - SafetyFactor));

        return DateTime.UtcNow < safeExpiration;
    }
}