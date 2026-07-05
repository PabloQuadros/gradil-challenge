using GradilChallenge.Domain.Entities;
using GradilChallenge.Domain.Enums;
using GradilChallenge.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore;

namespace GradilChallenge.Infrastructure.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(order => order.Id);

        builder.Property(order => order.Id)
            .ValueGeneratedNever();

        builder.Property(order => order.ConfirmedAt)
            .HasConversion(
                toDb => toDb,
                fromDb => DateTime.SpecifyKind(fromDb, DateTimeKind.Utc))
            .IsRequired();

        builder.OwnsOne(order => order.Quote, quoteBuilder =>
        {
            quoteBuilder.Property(quote => quote.DesiredLength)
                .HasConversion(LengthConverter())
                .HasColumnName("DesiredLengthInMeters")
                .IsRequired();

            quoteBuilder.Property(quote => quote.SoldLength)
                .HasConversion(LengthConverter())
                .HasColumnName("SoldLengthInMeters")
                .IsRequired();

            quoteBuilder.Property(quote => quote.Height)
                .HasConversion(HeightConverter())
                .HasColumnName("HeightId")
                .IsRequired();

            quoteBuilder.Property(quote => quote.Color)
                .HasConversion(ColorConverter())
                .HasColumnName("ColorId")
                .IsRequired();

            quoteBuilder.Property(quote => quote.Panel)
                .HasConversion(PanelConverter())
                .HasColumnName("PanelId")
                .IsRequired();

            quoteBuilder.Property(quote => quote.PanelCount).HasColumnName("PanelCount");
            quoteBuilder.Property(quote => quote.PostCount).HasColumnName("PostCount");
            quoteBuilder.Property(quote => quote.FastenerCount).HasColumnName("FastenerCount");
            quoteBuilder.Property(quote => quote.ScrewCount).HasColumnName("ScrewCount");
        });

        builder.Navigation(order => order.Quote).IsRequired();
    }

    private static ValueConverter<Length, double> LengthConverter() => new(
        length => length.Meters,
        meters => Length.FromMeters(meters));

    private static ValueConverter<FenceHeight, int> HeightConverter() => new(
        height => height.Id,
        id => FenceHeight.FromId(id));

    private static ValueConverter<FenceColor, int> ColorConverter() => new(
        color => color.Id,
        id => FenceColor.FromId(id));

    private static ValueConverter<FencePanel, int> PanelConverter() => new(
        panel => panel.Id,
        id => FencePanel.FromId(id));
}
