using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxisCRM.Api.Data.Mappings;
using AxisCRM.Api.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AxisCRM.Api.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Atendimento> Atendimento { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {   
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UsuarioMap());
            modelBuilder.ApplyConfiguration(new ClienteMap());
            modelBuilder.ApplyConfiguration(new AtendimentoMap());
        }
    }
}