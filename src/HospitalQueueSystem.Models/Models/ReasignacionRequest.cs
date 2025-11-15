namespace HospitalQueueSystem.Models
{
    public class ReasignacionRequest
    {
        public int TurnoId { get; set; }
        public int ClinicaNuevaId { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
    }
}