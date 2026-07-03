using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel.Domain.ValueObjects;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace SharedKernel.Persistence.Extensions;

public static class AddressComplexMappingExtensions
{
    public static EntityTypeBuilder<TEntity> ConfigureAddress<TEntity>(
       this EntityTypeBuilder<TEntity> builder,
       Expression<Func<TEntity, Address>> navigation,
       string columnPrefix = "Address_",
       bool isRequired = false)
       where TEntity : class
    {
        builder.ComplexProperty(navigation!, address =>
        {
            ConfigureAddressColumns(address, columnPrefix, isRequired);
        });

        return builder;
    }

    public static ComplexPropertyBuilder<TComplex> ConfigureAddress<TComplex>(
        this ComplexPropertyBuilder<TComplex> builder,
        Expression<Func<TComplex, Address>> navigation,
        string columnPrefix = "Address_",
        bool isRequired = false)
        where TComplex : class
    {
        builder.ComplexProperty(navigation!, address =>
        {
            ConfigureAddressColumns(address, columnPrefix, isRequired);
        });

        return builder;
    }

    private static void ConfigureAddressColumns(
        ComplexPropertyBuilder<Address> address,
        string columnPrefix,
        bool isRequired)
    {
        address.Property(x => x.Province)
            .HasColumnName(GenerateColumnName(columnPrefix, "Province"))
            .HasMaxLength(50)
            .IsRequired(isRequired);

        address.Property(x => x.City)
            .HasColumnName(GenerateColumnName(columnPrefix, "City"))
            .HasMaxLength(50)
            .IsRequired(isRequired);

        address.Property(x => x.Street)
            .HasColumnName(GenerateColumnName(columnPrefix, "Street"))
            .HasMaxLength(200)
            .IsRequired(isRequired);

        address.Property(x => x.BuildingNumber)
            .HasColumnName(GenerateColumnName(columnPrefix, "BuildingNumber"))
            .HasMaxLength(20)
            .IsRequired(false);

        address.ComplexProperty(x => x.PostalCode, postalCode =>
        {
            postalCode.Property(x => x.Value)
                .HasColumnName(GenerateColumnName(columnPrefix, "PostalCode"))
                .HasMaxLength(10)
                .IsRequired(isRequired);
        });

        address.Property(x => x.Unit)
            .HasColumnName(GenerateColumnName(columnPrefix, "Unit"))
            .HasMaxLength(20)
            .IsRequired(false);

        address.Property(x => x.AdditionalInfo)
            .HasColumnName(GenerateColumnName(columnPrefix, "AdditionalInfo"))
            .HasMaxLength(500)
            .IsRequired(false);
    }

    public static string GenerateColumnName(string? prefix, string name)
        => string.IsNullOrWhiteSpace(prefix) ? name
            : $"{prefix}{name}";
}