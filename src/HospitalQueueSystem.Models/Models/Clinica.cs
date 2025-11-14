using System.ComponentModel.DataAnnotations;

namespace HospitalQueueSystem.Models
{
    public class Clinica
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Descripcion { get; set; } = string.Empty;
        
        public bool Activa { get; set; } = true;
    }
}