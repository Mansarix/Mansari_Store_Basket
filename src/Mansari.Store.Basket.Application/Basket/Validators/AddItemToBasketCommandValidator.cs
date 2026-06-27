using FluentValidation;

namespace Mansari.Store.Basket.Application.Basket.Validators;

public sealed class AddItemToBasketCommandValidator : AbstractValidator<AddItemToBasketCommand>
{
    public AddItemToBasketCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0);

        RuleFor(x => x.Item.ProductId)
            .GreaterThan(0);

        RuleFor(x => x.Item.Quantity)
            .InclusiveBetween(1, 10);

        RuleFor(x => x.Item.UnitPrice)
            .GreaterThan(0);
    }
}
