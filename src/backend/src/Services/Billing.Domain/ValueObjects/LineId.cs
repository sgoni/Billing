namespace Billing.Domain.ValueObjects;

public record LineId : GuidValueObject
{
    public LineId(Guid value) : base(value)
    {
    }

    public static LineId Of(Guid value)
    {
        return new LineId(value);
    }
}