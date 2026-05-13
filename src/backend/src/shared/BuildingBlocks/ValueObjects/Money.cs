namespace BuildingBlocks.ValueObjects;

public record Money
{
    public Money(decimal amount, string currencyCode)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.");

        if (string.IsNullOrWhiteSpace(currencyCode))
            throw new ArgumentNullException("Currency code cannot be empty.");

        if (currencyCode.Length != 3)
            throw new ArgumentException("Currency code must have 3 characters.");

        Amount = amount;
        CurrencyCode = currencyCode.ToUpper();
    }

    public decimal Amount { get; }
    public string CurrencyCode { get; }

    public static Money Of(decimal amount, string currencyCode)
    {
        return new Money(amount, currencyCode);
    }

    public Money Add(Money other)
    {
        if (CurrencyCode != other.CurrencyCode)
            throw new ArgumentException("Cannot add Money with different currencies.");

        return new Money(Amount + other.Amount, CurrencyCode);
    }

    public Money Subtract(Money other)
    {
        if (CurrencyCode != other.CurrencyCode)
            throw new ArgumentException("Cannot subtract Money with different currencies.");

        return new Money(Amount - other.Amount, CurrencyCode);
    }
}