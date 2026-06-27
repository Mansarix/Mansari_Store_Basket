namespace Mansari.Store.Basket.Domain.ValueObjects;

using Mansari.Store.Basket.Domain.Common;

public class Money : ValueObject
{
    public decimal Value { get; private set; }

    private Money() { }

    public Money(decimal value)
    {
        if (value < 0)
            throw new ArgumentException("مقدار پول نمی‌تواند منفی باشد.", nameof(value));

        Value = value;
    }

    public static Money operator +(Money a, Money b) => new Money(a.Value + b.Value);
    public static Money operator -(Money a, Money b) => new Money(a.Value - b.Value);
    public static Money operator *(Money a, int multiplier) => new Money(a.Value * multiplier);

    public static bool operator >(Money a, Money b) => a.Value > b.Value;
    public static bool operator <(Money a, Money b) => a.Value < b.Value;
    public static bool operator >=(Money a, Money b) => a.Value >= b.Value;
    public static bool operator <=(Money a, Money b) => a.Value <= b.Value;

    public static implicit operator decimal(Money money) => money.Value;
    public static implicit operator Money(decimal value) => new Money(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString("N0");
}
