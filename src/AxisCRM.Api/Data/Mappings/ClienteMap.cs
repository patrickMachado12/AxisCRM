using AxisCRM.Api.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AxisCRM.Api.Data.Mappings
{
    public class ClienteMap : IEntityTypeConfiguration<Cliente>
    {
        public void Configure(EntityTypeBuilder<Cliente> builder)
        {
            builder.ToTable("cliente")
                .HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Nome)
                .HasColumnType("VARCHAR")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.CpfCnpj)
                .HasColumnType("VARCHAR")
                .IsRequired()
                .HasMaxLength(18);

            builder.HasIndex(p => p.CpfCnpj)
                .IsUnique();

            builder.Property(p => p.Email)
                .HasColumnType("VARCHAR")
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Telefone)
                .HasColumnType("VARCHAR")
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(p => p.Observacao)
                .HasColumnType("VARCHAR")
                .HasMaxLength(1000);

            builder.Property(p => p.DataCadastro)
                .HasColumnType("timestamp")
                .IsRequired();

            builder.Property(p => p.TipoPessoa)
                .IsRequired()
                .HasColumnType("integer");

            builder.Property(p => p.DataExclusao)
                .HasColumnType("timestamp");

            builder.Property(p => p.Excluido)
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
}