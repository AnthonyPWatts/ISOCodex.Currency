using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ISOCodex.Currency.EntityFrameworkCore;

/// <summary>
/// Entity Framework Core mapping helpers for currency value objects.
/// </summary>
public static class CurrencyPropertyBuilderExtensions
{
    /// <summary>
    /// Configures a property to store <see cref="CurrencyCode"/> as a non-Unicode fixed-length alpha-3 code.
    /// </summary>
    public static PropertyBuilder<CurrencyCode> HasCurrencyCodeConversion(this PropertyBuilder<CurrencyCode> builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.HasConversion(CurrencyCodeValueConverter.Instance);
        builder.HasMaxLength(3);
        builder.IsUnicode(false);
        builder.IsFixedLength();

        return builder;
    }

    /// <summary>
    /// Configures a complex-type property to store <see cref="CurrencyCode"/> as a non-Unicode fixed-length alpha-3 code.
    /// </summary>
    public static ComplexTypePropertyBuilder<CurrencyCode> HasCurrencyCodeConversion(
        this ComplexTypePropertyBuilder<CurrencyCode> builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.HasConversion(CurrencyCodeValueConverter.Instance);
        builder.HasMaxLength(3);
        builder.IsUnicode(false);
        builder.IsFixedLength();

        return builder;
    }

    /// <summary>
    /// Configures a <see cref="Money"/> complex property using separate amount and currency-code columns.
    /// </summary>
    public static ComplexPropertyBuilder<Money> ComplexMoney<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, Money>> propertyExpression,
        string amountColumn,
        string currencyColumn,
        string amountColumnType = "decimal(19,4)")
        where TEntity : class
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (propertyExpression == null)
        {
            throw new ArgumentNullException(nameof(propertyExpression));
        }

        if (string.IsNullOrWhiteSpace(amountColumn))
        {
            throw new ArgumentException("Amount column name must be supplied.", nameof(amountColumn));
        }

        if (string.IsNullOrWhiteSpace(currencyColumn))
        {
            throw new ArgumentException("Currency column name must be supplied.", nameof(currencyColumn));
        }

        var complexProperty = builder.ComplexProperty(propertyExpression);

        var amountProperty = complexProperty.Property(money => money.Amount)
            .HasColumnName(amountColumn);

        if (!string.IsNullOrWhiteSpace(amountColumnType))
        {
            amountProperty.HasColumnType(amountColumnType);
        }

        complexProperty.Property(money => money.Currency)
            .HasColumnName(currencyColumn)
            .HasCurrencyCodeConversion();

        return complexProperty;
    }
}
