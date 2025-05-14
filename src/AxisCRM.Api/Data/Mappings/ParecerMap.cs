using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AxisCRM.Api.Data.Mappings
{
    public class ParecerMap : IEntityTypeConfiguration<Parecer>
    {
        public void Configure(EntityTypeBuilder<Parecer> builder)
        {
            builder.ToTable("parecer")
                .HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.Descricao)
                .HasColumnType("VARCHAR")
                .IsRequired();

            builder.Property(p => p.DataCadastro)
                .HasColumnType("timestamp")
                .IsRequired();

            builder.Property(p => p.PessoaContato)
                .HasColumnType("VARCHAR")
                .IsRequired()
                .HasMaxLength(20);
            
            builder.Property(p => p.DataUltimaAlteracao)
                .HasColumnType("timestamp")
                .IsRequired();

            builder.HasOne(p => p.Atendimento)
                .WithMany()
                .HasForeignKey(fk => fk.IdAtendimento)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Usuario)
                .WithMany()
                .HasForeignKey(fk => fk.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}