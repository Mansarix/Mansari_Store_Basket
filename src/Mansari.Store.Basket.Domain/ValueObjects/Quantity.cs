using Mansari.Store.Basket.Domain.Common;

namespace Mansari.Store.Basket.Domain.ValueObjects;

public sealed class Quantity : ValueObject
{
    private const int MinValue = 1;
    private const int MaxValue = 10;

    public int Value { get; }

    private Quantity() { }

    private Quantity(int value)
    {
        if (value < MinValue)
            throw new BasketDomainException(BasketErrors.InvalidQuantity);

        if (value > MaxValue)
            throw new BasketDomainException(BasketErrors.InvalidQuantity);

        Value = value;
    }

    public static Quantity Create(int value)
    {
        return new Quantity(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator int(Quantity quantity) => quantity.Value;

    public override string ToString() => Value.ToString();
}
