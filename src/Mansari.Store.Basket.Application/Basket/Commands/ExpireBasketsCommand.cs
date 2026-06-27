using Mansari.Store.Basket.Application.Basket.Commands;
using Mansari.Store.Basket.Application.Common.Abstractions;

public sealed record ExpireBasketsCommand : ICommand<int>;
