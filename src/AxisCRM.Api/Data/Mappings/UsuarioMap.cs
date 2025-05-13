using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AxisCRM.Api.Data.Mappings
{
    public class UsuarioMap : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("usuario")
                .HasKey(p => p.Id);

            builder.Property(p => p.Email)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.HasIndex(p => p.Email)
                .IsUnique();

            builder.Property(p => p.Senha)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(p => p.Perfil)
                .IsRequired()
                .HasColumnType("integer");

            builder.Property(p => p.DataCadastro)
                .HasColumnType("timestamp")
                .IsRequired();

            builder.Property(p => p.DataExclusao)
                .HasColumnType("timestamp");

            builder.Property(p => p.Excluido)
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
    
}