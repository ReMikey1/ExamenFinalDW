using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalQueueSystem.Models
{
    public class Reasignacion
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int TurnoId { get; set; }
        
        [Required]
        public int ClinicaAnteriorId { get; set; }
        
        [Required]
        public int ClinicaNuevaId { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Motivo { get; set; } = string.Empty;
        
        public DateTime FechaReasignacion { get; set; } = DateTime.UtcNow;
        
        [Required]
        public int UsuarioId { get; set; }
        
        // Navegación
        [ForeignKey("TurnoId")]
        public Turno? Turno { get; set; }
        
        [ForeignKey("ClinicaAnteriorId")]
        public Clinica? ClinicaAnterior { get; set; }
        
        [ForeignKey("ClinicaNuevaId")]
        public Clinica? ClinicaNueva { get; set; }
        
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }
    }
}