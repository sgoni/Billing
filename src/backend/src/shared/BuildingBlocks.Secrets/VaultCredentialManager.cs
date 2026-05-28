public class VaultCredentialManager
{
    private readonly VaultClient _vaultClient;
    private readonly VaultSettings _settings;

    private readonly SemaphoreSlim _lock = new(1, 1);
    private VaultCredentialLease? _currentLease;

    private const double RenewalThreshold = 0.7; // 70% of the TTL
    private const double ExpirationSafety = 0.9; // 90% → regenerate

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
        if (_currentLease == null)
            return await CreateNewCredentialsAsync();

        var now = DateTime.UtcNow;
        var totalTtl = _currentLease.LeaseDurationSeconds;

        var renewTime = _currentLease.ExpirationUtc.AddSeconds(-totalTtl * (1 - RenewalThreshold));
        var expireTime = _currentLease.ExpirationUtc.AddSeconds(-totalTtl * (1 - ExpirationSafety));

        if (now < renewTime)
        {
            // still valid
            return (_currentLease.Username, _currentLease.Password);
        }

        await _lock.WaitAsync();

        try
        {
            // double-check
            now = DateTime.UtcNow;

            if (_currentLease == null)
                return await CreateNewCredentialsAsync();

            totalTtl = _currentLease.LeaseDurationSeconds;
            renewTime = _currentLease.ExpirationUtc.AddSeconds(-totalTtl * (1 - RenewalThreshold));
            expireTime = _currentLease.ExpirationUtc.AddSeconds(-totalTtl * (1 - ExpirationSafety));

            if (now < renewTime)
                return (_currentLease.Username, _currentLease.Password);

            // try to renew
            if (now < expireTime)
            {
                var renewed = await TryRenewLeaseAsync(3600);

                if (renewed)
                    return (_currentLease!.Username, _currentLease.Password);
            }

            // fallback → new credentials
            return await CreateNewCredentialsAsync();
        }
        finally
        {
            _lock.Release();
        }
    }

    // ---------------------------
    //  RENEW LEASE
    // ---------------------------
    private async Task<bool> TryRenewLeaseAsync(int renewTime)
    {
        try
        {
            var renewal = await _vaultClient.V1.System.RenewLeaseAsync(_currentLease!.LeaseId, renewTime);
            _currentLease.LeaseDurationSeconds = renewal.LeaseDurationSeconds;
            _currentLease.ExpirationUtc = DateTime.UtcNow.AddSeconds(renewal.LeaseDurationSeconds);

            return true;
        }
        catch
        {
            // Vault did not allow renewal
            return false;
        }
    }

    // ---------------------------
    // NEW CREDENTIALS
    // ---------------------------
    private async Task<(string Username, string Password)> CreateNewCredentialsAsync()
    {
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

    private bool IsLeaseValid()
    {
        if (_currentLease == null)
            return false;

        var safeExpiration = _currentLease.ExpirationUtc
            .AddSeconds(-_currentLease.LeaseDurationSeconds * (1 - SafetyFactor));

        return DateTime.UtcNow < safeExpiration;
    }
}