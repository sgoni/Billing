namespace BuildingBlocks.Secrets;

public interface ISecretManager
{
    Task<T> GetCredential<T>(string path) where T : new();
    Task<UsernamePasswordCredentials> GetPostgreSQLCredential<T>() where T : new();
    Task<UsernamePasswordCredentials> GetRabbitMQCredential<T>(string path) where T : new();
}