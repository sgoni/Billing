namespace Billing.Domain.ValueObjects;

public record InvoiceId : GuidValueObject
{
    public InvoiceId(Guid value) : base(value)
    {
    }

    public static InvoiceId Of(Guid value)
    {
        return new InvoiceId(value);
    }
}