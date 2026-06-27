using FluentValidation;

namespace Mansari.Store.Basket.Application.Basket.Validators;

public sealed class UpdateBasketItemQuantityCommandValidator
    : AbstractValidator<UpdateBasketItemQuantityCommand>
{
    public UpdateBasketItemQuantityCommandValidator()
    {
        RuleFor(x => x.NewQuantity)
            .InclusiveBetween(1, 10);
    }
}
