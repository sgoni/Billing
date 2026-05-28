namespace BuildingBlocks.Secrets;

public record VaultSettings
{
    [Required] public string? Address { get; set; }
    [Required] public string? Role { get; set; }
    public string? MountPath { get; set; }
    public string? SecretType { get; set; }
    public string? TokenApi { get; set; }
}