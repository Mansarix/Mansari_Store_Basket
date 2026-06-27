namespace Mansari.Store.Basket.Domain.Common;

public sealed class BasketDomainException : DomainException
{
    public BasketDomainException(string message) : base(message)
    {
    }
}