namespace BuildingBlocks.Secrets;

public class VaultCredentialLease
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string LeaseId { get; set; }
    public int LeaseDurationSeconds { get; set; }
    public DateTime ExpirationUtc { get; set; }
}