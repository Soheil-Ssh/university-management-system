using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Domain.ValueObjects;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Persistence.Extensions;

public static class PhoneConfigurationExtensions
{
    public static EntityTypeBuilder<TEntity> ConfigurePhone<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, PhoneNumber>> navigation,
        string columnName,
        bool isRequired = false)
        where TEntity : class
    {
        builder.ComplexProperty(navigation!, phone =>
        {
            phone.Property(x => x.Value)
                .HasColumnName(columnName)
                .HasMaxLength(20)
                .IsRequired(isRequired);
        });

        return builder;
    }

    public static ComplexPropertyBuilder<TComplex> ConfigurePhone<TComplex>(
        this ComplexPropertyBuilder<TComplex> builder,
        Expression<Func<TComplex, PhoneNumber>> navigation,
        string columnName,
        bool isRequired = false)
        where TComplex : class
    {
        builder.ComplexProperty(navigation!, phone =>
        {
            phone.Property(x => x.Value)
                .HasColumnName(columnName)
                .HasMaxLength(20)
                .IsRequired(isRequired);
        });

        return builder;
    }

    public static EntityTypeBuilder<TEntity> ConfigurePhoneAsConversion<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, PhoneNumber>> propertyExpression,
        string columnName,
        bool isRequired = false)
        where TEntity : class
    {
        var propertyBuilder = builder.Property(propertyExpression)
            .HasColumnName(columnName)
            .HasMaxLength(20)
            .HasConversion(
                phoneNumber => phoneNumber.Value,
                value => PhoneNumber.Create(value).Data);

        if (isRequired)
            propertyBuilder.IsRequired();

        return builder;
    }
}