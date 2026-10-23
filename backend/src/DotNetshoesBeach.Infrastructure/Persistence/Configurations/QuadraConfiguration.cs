using DotNetshoesBeach.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotNetshoesBeach.Infrastructure.Persistence.Configurations;

public class QuadraConfiguration : IEntityTypeConfiguration<Quadra>
{
    public void Configure(EntityTypeBuilder<Quadra> builder)
    {
        builder.ToTable("quadras");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(q => q.Nome)
            .IsUnique();

        builder.Property(q => q.Descricao)
            .HasMaxLength(500);

        builder.Property(q => q.ValorHora)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(q => q.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(q => q.MotivoManutencao)
            .HasMaxLength(500);

        builder.Property(q => q.CriadoEm)
            .IsRequired();

        builder.Ignore(q => q.EstaDisponivel);
    }
}
