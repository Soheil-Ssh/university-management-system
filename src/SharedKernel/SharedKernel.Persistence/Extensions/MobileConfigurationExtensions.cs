using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Domain.ValueObjects;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Persistence.Extensions;

public static class MobileConfigurationExtensions
{
    public static EntityTypeBuilder<TEntity> ConfigureMobile<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, MobileNumber>> navigation,
        string columnName,
        bool isRequired = false)
        where TEntity : class
    {
        builder.ComplexProperty(navigation!, mobile =>
        {
            mobile.Property(x => x.Value)
                .HasColumnName(columnName)
                .HasMaxLength(11)
                .IsRequired(isRequired);
        });

        return builder;
    }

    public static ComplexPropertyBuilder<TComplex> ConfigureMobile<TComplex>(
        this ComplexPropertyBuilder<TComplex> builder,
        Expression<Func<TComplex, MobileNumber>> navigation,
        string columnName,
        bool isRequired = false)
        where TComplex : class
    {
        builder.ComplexProperty(navigation!, mobile =>
        {
            mobile.Property(x => x.Value)
                .HasColumnName(columnName)
                .HasMaxLength(11)
                .IsRequired(isRequired);
        });

        return builder;
    }

    public static EntityTypeBuilder<TEntity> ConfigureMobileAsConversion<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, MobileNumber>> propertyExpression,
        string columnName,
        bool isRequired = false)
        where TEntity : class
    {
        var propertyBuilder = builder.Property(propertyExpression)
            .HasColumnName(columnName)
            .HasMaxLength(11)
            .HasConversion(
                mobileNumber => mobileNumber.Value,
                value => MobileNumber.Create(value).Data);

        if (isRequired)
            propertyBuilder.IsRequired();

        return builder;
    }
}