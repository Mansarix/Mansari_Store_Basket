using Mansari.Store.Basket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mansari.Store.Infrastructure.Persistence.Configurations;

public sealed class BasketConfiguration : IEntityTypeConfiguration<Mansari.Store.Basket.Domain.Aggregates.Basket>
{
    public void Configure(EntityTypeBuilder<Mansari.Store.Basket.Domain.Aggregates.Basket> builder)
    {
        //builder.ToTable("Baskets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.LastUpdatedAt);

        builder.HasMany<BasketItem>("_items")
            .WithOne()
            .HasForeignKey("BasketId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation("_items")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => new { x.UserId, x.Status });

        //builder.HasQueryFilter(x => x.Status == BasketStatus.Active);
    }
}