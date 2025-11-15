using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalQueueSystem.Models;
using HospitalQueueSystem.Models.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HospitalQueueSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReasignacionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReasignacionesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/reasignaciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reasignacion>>> GetReasignaciones()
        {
            return await _context.Reasignaciones
                .Include(r => r.Turno)
                    .ThenInclude(t => t.Paciente)
                .Include(r => r.ClinicaAnterior)
                .Include(r => r.ClinicaNueva)
                .Include(r => r.Usuario)
                .OrderByDescending(r => r.FechaReasignacion)
                .ToListAsync();
        }

        // GET: api/reasignaciones/turno/5
        [HttpGet("turno/{turnoId}")]
        public async Task<ActionResult<IEnumerable<Reasignacion>>> GetReasignacionesPorTurno(int turnoId)
        {
            return await _context.Reasignaciones
                .Include(r => r.ClinicaAnterior)
                .Include(r => r.ClinicaNueva)
                .Include(r => r.Usuario)
                .Where(r => r.TurnoId == turnoId)
                .OrderByDescending(r => r.FechaReasignacion)
                .ToListAsync();
        }

        // POST: api/reasignaciones
        [HttpPost]
        public async Task<ActionResult<Reasignacion>> PostReasignacion(ReasignacionRequest request)
        {
            // Obtener el usuario actual del token JWT
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            if (usuarioId == 0)
            {
                return Unauthorized("Usuario no válido");
            }

            // Verificar que el turno existe
            var turno = await _context.Turnos
                .Include(t => t.Clinica)
                .Include(t => t.Paciente)
                .FirstOrDefaultAsync(t => t.Id == request.TurnoId);

            if (turno == null)
            {
                return NotFound("Turno no encontrado");
            }

            // Verificar que la nueva clínica existe
            var clinicaNueva = await _context.Clinicas.FindAsync(request.ClinicaNuevaId);
            if (clinicaNueva == null)
            {
                return NotFound("Clínica nueva no encontrada");
            }

            // Guardar la clínica anterior antes de actualizar
            var clinicaAnteriorId = turno.ClinicaId;

            // Obtener el siguiente número de turno para la nueva clínica
            var ultimoTurno = await _context.Turnos
                .Where(t => t.ClinicaId == request.ClinicaNuevaId)
                .OrderByDescending(t => t.NumeroTurno)
                .FirstOrDefaultAsync();

            var nuevoNumeroTurno = (ultimoTurno?.NumeroTurno ?? 0) + 1;

            // Crear registro de reasignación
            var reasignacion = new Reasignacion
            {
                TurnoId = request.TurnoId,
                ClinicaAnteriorId = clinicaAnteriorId,
                ClinicaNuevaId = request.ClinicaNuevaId,
                Motivo = request.Motivo,
                UsuarioId = usuarioId,
                FechaReasignacion = DateTime.UtcNow
            };

            _context.Reasignaciones.Add(reasignacion);

            // Actualizar el turno con la nueva clínica y número
            turno.ClinicaId = request.ClinicaNuevaId;
            turno.NumeroTurno = nuevoNumeroTurno;
            turno.Estado = "Pendiente"; // Reiniciar estado

            try
            {
                await _context.SaveChangesAsync();
                
                // Cargar datos relacionados para la respuesta
                await _context.Entry(reasignacion)
                    .Reference(r => r.ClinicaAnterior)
                    .LoadAsync();
                    
                await _context.Entry(reasignacion)
                    .Reference(r => r.ClinicaNueva)
                    .LoadAsync();
                    
                await _context.Entry(reasignacion)
                    .Reference(r => r.Usuario)
                    .LoadAsync();

                return CreatedAtAction("GetReasignacion", new { id = reasignacion.Id }, reasignacion);
            }
            catch (DbUpdateException)
            {
                return BadRequest("Error al guardar la reasignación");
            }
        }

        // GET: api/reasignaciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reasignacion>> GetReasignacion(int id)
        {
            var reasignacion = await _context.Reasignaciones
                .Include(r => r.Turno)
                    .ThenInclude(t => t.Paciente)
                .Include(r => r.ClinicaAnterior)
                .Include(r => r.ClinicaNueva)
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reasignacion == null)
            {
                return NotFound();
            }

            return reasignacion;
        }

        // DELETE: api/reasignaciones/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Recepcion")]
        public async Task<IActionResult> DeleteReasignacion(int id)
        {
            var reasignacion = await _context.Reasignaciones.FindAsync(id);
            if (reasignacion == null)
            {
                return NotFound();
            }

            _context.Reasignaciones.Remove(reasignacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}