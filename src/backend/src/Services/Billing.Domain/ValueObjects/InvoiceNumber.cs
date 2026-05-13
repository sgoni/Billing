namespace Billing.Domain.ValueObjects;

public record InvoiceNumber : StringValueObject
{
    public InvoiceNumber(string value) : base(value, 25)
    {
    }

    public static InvoiceNumber Of(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (value.Length > 15)
            throw new DomainException("AccountTypeDesc cannot exceed 15 characters.");

        // Example: validate that it is alphanumeric with scripts or bars
        if (!Regex.IsMatch(value, @"^[A-Za-z0-9\-\/]+$"))
            throw new DomainException("AccountTypeDesc must be alphanumeric, allowing '-' and '/'.");

        return new InvoiceNumber(value);
    }

    public override string ToString()
    {
        return Value;
    }
}