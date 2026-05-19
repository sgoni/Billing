namespace Integration.Services.Domain.Enums;

public class EventRelayStatus : Enumeration
{
    public static EventRelayStatus Pending = new(0, nameof(Pending));
    public static EventRelayStatus Published = new(1, nameof(Published));
    public static EventRelayStatus Failed = new(2, nameof(Failed));

    public EventRelayStatus(int id, string name) : base(id, name)
    {
    }
}