using AxisCRM.Api.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AxisCRM.Api.Data.Mappings
{
    public class AtendimentoMap : IEntityTypeConfiguration<Atendimento>
    {
        public void Configure(EntityTypeBuilder<Atendimento> builder)
        {
            builder.ToTable("atendimento")
                .HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Assunto)
                .HasColumnType("VARCHAR")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.DataCadastro)
                .HasColumnType("timestamp")
                .IsRequired();

            builder.Property(p => p.DataEncerramento)
                .HasColumnType("timestamp");

            builder.Property(p => p.Status)
                .IsRequired();

            builder.Property(p => p.DataUltimaAtualizacao)
                .HasColumnType("timestamp");

            builder.HasOne(p => p.Cliente)
                .WithMany()
                .HasForeignKey(fk => fk.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Usuario)
                .WithMany()
                .HasForeignKey(fk => fk.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}