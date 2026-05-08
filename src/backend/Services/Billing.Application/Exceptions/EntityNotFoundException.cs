namespace Billing.Application.Exceptions;

public class EntityNotFoundException : NotFoundException
{
    public EntityNotFoundException(string entityName, object id)
        : base($"{entityName} with id {id} not found")
    {
    }

    public static EntityNotFoundException For<T>(object id)
    {
        return new EntityNotFoundException(typeof(T).Name, id);
    }
}