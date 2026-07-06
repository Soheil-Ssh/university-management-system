using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Domain.ValueObjects;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Persistence.Extensions;

public static class NationalCodeConfigurationExtensions
{
    public static ComplexPropertyBuilder<TComplex> ConfigureNationalCode<TComplex>(
        this ComplexPropertyBuilder<TComplex> builder,
        Expression<Func<TComplex, NationalCode>> navigation,
        string columnName,
        bool isRequired = false)
        where TComplex : class
    {
        builder.ComplexProperty(navigation!, nationalCode =>
        {
            nationalCode.Property(x => x.Value)
                .HasColumnName(columnName)
                .HasMaxLength(10)
                .IsRequired(isRequired);
        });

        return builder;
    }

    public static EntityTypeBuilder<TEntity> ConfigureNationalCode<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, NationalCode>> navigation,
        string columnName,
        bool isRequired = false)
        where TEntity : class
    {
        builder.ComplexProperty(navigation!, nationalCode =>
        {
            nationalCode.Property(x => x.Value)
                .HasColumnName(columnName)
                .HasMaxLength(10)
                .IsRequired(isRequired);
        });

        return builder;
    }

    public static EntityTypeBuilder<TEntity> ConfigureNationalCodeAsConversion<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, NationalCode>> propertyExpression,
        string columnName,
        bool isRequired = false)
        where TEntity : class
    {
        var propertyBuilder = builder.Property(propertyExpression)
            .HasColumnName(columnName)
            .HasMaxLength(10)
            .HasConversion(
                nationalCode => nationalCode.Value,
                value => NationalCode.Create(value).Data);

        if (isRequired)
            propertyBuilder.IsRequired();

        return builder;
    }
}