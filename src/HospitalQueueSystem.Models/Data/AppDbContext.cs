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
        public DbSet<Reasignacion> Reasignaciones { get; set; } // NUEVO

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuraciones existentes
            modelBuilder.Entity<Turno>()
                .HasIndex(t => new { t.ClinicaId, t.Estado });
                
            modelBuilder.Entity<Turno>()
                .HasIndex(t => t.NumeroTurno);
                
            // Nueva configuración para Reasignaciones
            modelBuilder.Entity<Reasignacion>()
                .HasIndex(r => r.TurnoId);
                
            modelBuilder.Entity<Reasignacion>()
                .HasIndex(r => r.FechaReasignacion);
        }
    }
}
