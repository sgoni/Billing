namespace Billing.Domain.ValueObjects;

public record CustomerId : GuidValueObject
{
    public CustomerId(Guid value) : base(value)
    {
    }

    public static CustomerId Of(Guid value)
    {
        return new CustomerId(value);
    }

    public static CustomerId? FromNullable(Guid? value)
    {
        if (!value.HasValue || value == Guid.Empty) return null;
        return new CustomerId(value.Value);
    }
}