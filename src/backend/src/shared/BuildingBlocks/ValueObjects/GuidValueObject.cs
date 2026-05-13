namespace BuildingBlocks.ValueObjects;

public abstract record GuidValueObject
{
    protected GuidValueObject(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException($"{GetType().Name} cannot be empty");

        Value = value;
    }

    public Guid Value { get; }

    // Override equals para comparar por valor
    public virtual bool Equals(GuidValueObject? other)
    {
        return other is not null && Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}