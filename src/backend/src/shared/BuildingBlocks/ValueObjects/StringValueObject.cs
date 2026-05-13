namespace BuildingBlocks.ValueObjects;

public abstract record StringValueObject
{
    protected StringValueObject(string value, int maxLength = int.MaxValue)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentNullException($"{GetType().Name} cannot be null or empty");

        if (value.Length > maxLength)
            throw new ArgumentException($"{GetType().Name} cannot exceed {maxLength} characters");

        Value = value;
    }

    public string Value { get; }

    // Comparación por valor
    public virtual bool Equals(StringValueObject? other)
    {
        return other is not null && Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}