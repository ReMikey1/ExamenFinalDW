using Microsoft.EntityFrameworkCore;

namespace HospitalQueueSystem.Models.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Paciente> Pacientes { get; set; }
        public DbSet<Clinica> Clinicas { get; set; }
        public DbSet<Turno> Turnos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuraciones adicionales si son necesarias
            modelBuilder.Entity<Turno>()
                .HasIndex(t => new { t.ClinicaId, t.Estado });
                
            modelBuilder.Entity<Turno>()
                .HasIndex(t => t.NumeroTurno);
        }
    }
}