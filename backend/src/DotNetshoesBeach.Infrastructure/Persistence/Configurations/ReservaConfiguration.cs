using DotNetshoesBeach.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DotNetshoesBeach.Infrastructure.Persistence.Configurations;

public class ReservaConfiguration : IEntityTypeConfiguration<Reserva>
{
    public void Configure(EntityTypeBuilder<Reserva> builder)
    {
        builder.ToTable("reservas");

        builder.HasKey(r => r.Id);

        builder.HasOne(r => r.Cliente)
            .WithMany()
            .HasForeignKey(r => r.ClienteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Quadra)
            .WithMany()
            .HasForeignKey(r => r.QuadraId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(r => r.DataHoraInicio).IsRequired();
        builder.Property(r => r.DataHoraFim).IsRequired();

        builder.Property(r => r.Valor)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(r => r.CriadoEm)
            .IsRequired();

        // Índices compostos: "reservas de uma quadra/cliente ordenadas ou filtradas por data".
        builder.HasIndex(r => new { r.QuadraId, r.DataHoraInicio });
        builder.HasIndex(r => new { r.ClienteId, r.DataHoraInicio });

        // A constraint de exclusão que impede reservas sobrepostas na mesma quadra
        // não é expressável no EF Core: ela é criada pelo DbInitializer (ver README do banco).
    }
}
