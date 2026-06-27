using Mansari.Store.Basket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mansari.Store.Basket.Infrastructure.Persistence.Configurations;

public class BasketItemConfiguration : IEntityTypeConfiguration<BasketItem>
{
    public void Configure(EntityTypeBuilder<BasketItem> builder)
    {
        builder.ToTable("BasketItems");

        builder.HasKey(x => x.Id);
		
		builder.Property(x => x.ProductId)
			.IsRequired();

		builder.Property(x => x.UnitPrice)
			.HasPrecision(18, 2);

        builder.OwnsOne(x => x.Quantity, q =>
        {
            q.Property(x => x.Value)
             .HasColumnName("Quantity")
             .IsRequired();
        });
    }
}

