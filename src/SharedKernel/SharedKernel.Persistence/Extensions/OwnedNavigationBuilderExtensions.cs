using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Domain.ValueObjects;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Persistence.Extensions;

public static class OwnedNavigationBuilderExtensions
{
    public static OwnedNavigationBuilder<TEntity, TDependent> ConfigureName<TEntity, TDependent>(
        this OwnedNavigationBuilder<TEntity, TDependent> builder,
        Expression<Func<TDependent, Name>> navigation,
        string columnName,
        bool isRequired = false)
        where TEntity : class
        where TDependent : class
    {
        builder.OwnsOne(navigation!, owned =>
        {
            var property = owned.Property(x => x.Value)
                .HasColumnName(columnName)
                .HasMaxLength(100);

            if (isRequired)
                property.IsRequired();
        });

        return builder;
    }

    public static OwnedNavigationBuilder<TEntity, TDependent> ConfigureNationalCode<TEntity, TDependent>(
        this OwnedNavigationBuilder<TEntity, TDependent> builder,
        Expression<Func<TDependent, NationalCode>> navigation,
        string columnName,
        bool isRequired = false)
        where TEntity : class
        where TDependent : class
    {
        builder.OwnsOne(navigation!, owned =>
        {
            var property = owned.Property(x => x.Value)
                .HasColumnName(columnName)
                .HasMaxLength(10);

            if (isRequired)
                property.IsRequired();
        });

        return builder;
    }

    public static OwnedNavigationBuilder<TEntity, TDependent> ConfigureMobile<TEntity, TDependent>(
        this OwnedNavigationBuilder<TEntity, TDependent> builder,
        Expression<Func<TDependent, MobileNumber>> navigation,
        string columnName,
        bool isRequired = false)
        where TEntity : class
        where TDependent : class
    {
        builder.OwnsOne(navigation!, owned =>
        {
            var property = owned.Property(x => x.Value)
                .HasColumnName(columnName)
                .HasMaxLength(11)
                .IsRequired();

            if (isRequired)
                property.IsRequired();
        });

        return builder;
    }

    public static OwnedNavigationBuilder<TEntity, TDependent> ConfigurePhone<TEntity, TDependent>(
        this OwnedNavigationBuilder<TEntity, TDependent> builder,
        Expression<Func<TDependent, PhoneNumber>> navigation,
        string columnName,
        bool isRequired = false)
        where TEntity : class
        where TDependent : class
    {
        builder.OwnsOne(navigation!, owned =>
        {
            var property = owned.Property(x => x.Value)
                .HasColumnName(columnName)
                .HasMaxLength(20)
                .IsRequired();

            if (isRequired)
                property.IsRequired();
        });

        return builder;
    }

    public static OwnedNavigationBuilder<TEntity, TDependent> ConfigureEmail<TEntity, TDependent>(
        this OwnedNavigationBuilder<TEntity, TDependent> builder,
        Expression<Func<TDependent, Email>> navigation,
        string columnName,
        bool isRequired = false)
        where TEntity : class
        where TDependent : class
    {
        builder.OwnsOne(navigation!, owned =>
        {
            var property = owned.Property(x => x.Value)
                .HasColumnName(columnName)
                .HasMaxLength(320)
                .IsRequired();

            if (isRequired)
                property.IsRequired();
        });

        return builder;
    }

    public static OwnedNavigationBuilder<TEntity, TDependent> ConfigureAddress<TEntity, TDependent>(
        this OwnedNavigationBuilder<TEntity, TDependent> builder,
        Expression<Func<TDependent, Address>> navigation)
        where TEntity : class
        where TDependent : class
    {
        builder.OwnsOne(navigation!, owned =>
        {
            owned.Property(x => x.Province)
                .HasMaxLength(50);

            owned.Property(x => x.City)
                .HasMaxLength(50);

            owned.Property(x => x.Street)
                .HasMaxLength(200);

            owned.Property(x => x.BuildingNumber)
                .HasMaxLength(20);

            owned.OwnsOne(x => x.PostalCode, postalCode =>
            {
                postalCode.Property(x => x.Value)
                    .HasColumnName("PostalCode")
                    .HasMaxLength(10);
            });

            owned.Property(x => x.Unit)
                .HasMaxLength(20);

            owned.Property(x => x.AdditionalInfo)
                .HasMaxLength(500);
        });

        return builder;
    }
}