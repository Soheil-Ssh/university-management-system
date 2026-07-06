using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Domain.ValueObjects;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Persistence.Extensions;

public static class PostalCodeConfigurationExtensions
{

    public static EntityTypeBuilder<TEntity> ConfigurePostalCode<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, PostalCode>> navigation,
        string columnName,
        bool isRequired = false)
        where TEntity : class
    {
        builder.ComplexProperty(navigation!, postalCode =>
        {
            postalCode.Property(x => x.Value)
                .HasColumnName(columnName)
                .HasMaxLength(10)
                .IsRequired(isRequired);
        });

        return builder;
    }

    public static ComplexPropertyBuilder<TComplex> ConfigurePostalCode<TComplex>(
        this ComplexPropertyBuilder<TComplex> builder,
        Expression<Func<TComplex, PostalCode>> navigation,
        string columnName,
        bool isRequired = false)
        where TComplex : class
    {
        builder.ComplexProperty(navigation!, postalCode =>
        {
            postalCode.Property(x => x.Value)
                .HasColumnName(columnName)
                .HasMaxLength(10)
                .IsRequired(isRequired);
        });

        return builder;
    }

    public static EntityTypeBuilder<TEntity> ConfigurePostalCodeAsConversion<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, PostalCode>> propertyExpression,
        string columnName,
        bool isRequired = false)
        where TEntity : class
    {
        var propertyBuilder = builder.Property(propertyExpression)
            .HasColumnName(columnName)
            .HasMaxLength(10)
            .HasConversion(
                postalCode => postalCode.Value,
                value => PostalCode.Create(value).Data);

        if (isRequired)
            propertyBuilder.IsRequired();

        return builder;
    }
}