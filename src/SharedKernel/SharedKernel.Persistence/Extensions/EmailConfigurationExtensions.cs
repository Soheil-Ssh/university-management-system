using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Domain.ValueObjects;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Persistence.Extensions;

public static class EmailConfigurationExtensions
{
    public static EntityTypeBuilder<TEntity> ConfigureEmail<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, Email>> navigation,
        string columnName,
        bool isRequired = false)
        where TEntity : class
    {
        builder.ComplexProperty(navigation!, email =>
        {
            email.Property(x => x.Value)
                .HasColumnName(columnName)
                .HasMaxLength(256)
                .IsRequired(isRequired);
        });

        return builder;
    }

    public static ComplexPropertyBuilder<TComplex> ConfigureEmail<TComplex>(
        this ComplexPropertyBuilder<TComplex> builder,
        Expression<Func<TComplex, Email>> navigation,
        string columnName,
        bool isRequired = false)
        where TComplex : class
    {
        builder.ComplexProperty(navigation!, email =>
        {
            email.Property(x => x.Value)
                .HasColumnName(columnName)
                .HasMaxLength(256)
                .IsRequired(isRequired);
        });

        return builder;
    }

    public static EntityTypeBuilder<TEntity> ConfigureEmailAsConversion<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, Email>> propertyExpression,
        string columnName,
        bool isRequired = false)
        where TEntity : class
    {
        var propertyBuilder = builder.Property(propertyExpression)
            .HasColumnName(columnName)
            .HasMaxLength(256)
            .HasConversion(email => email.Value,
                value => Email.Create(value).Data);

        if (isRequired)
            propertyBuilder.IsRequired();

        return builder;
    }
}