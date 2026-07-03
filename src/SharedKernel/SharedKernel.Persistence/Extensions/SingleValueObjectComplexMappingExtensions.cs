using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Domain.ValueObjects;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Persistence.Extensions;

public static class SingleValueObjectComplexMappingExtensions
{
    public static EntityTypeBuilder<TEntity> ConfigureName<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        Expression<Func<TEntity, Name>> navigation,
        string columnName,
        bool isRequired = false)
        where TEntity : class
    {
        builder.ComplexProperty(navigation!, name =>
        {
            name.Property(x => x.Value)
                .HasColumnName(columnName)
                .HasMaxLength(100)
                .IsRequired(isRequired);
        });

        return builder;
    }

    public static ComplexPropertyBuilder<TComplex> ConfigureName<TComplex>(
        this ComplexPropertyBuilder<TComplex> builder,
        Expression<Func<TComplex, Name>> navigation,
        string columnName,
        bool isRequired = false)
        where TComplex : class
    {
        builder.ComplexProperty(navigation!, name =>
        {
            name.Property(x => x.Value)
                .HasColumnName(columnName)
                .HasMaxLength(100)
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
}