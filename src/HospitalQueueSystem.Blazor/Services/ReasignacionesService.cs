using System.Net.Http.Json;
using HospitalQueueSystem.Models;

namespace HospitalQueueSystem.Blazor.Services
{
    public class ReasignacionesService
    {
        private readonly HttpClient _httpClient;

        public ReasignacionesService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("HospitalAPI");
        }

        public async Task<List<Reasignacion>?> GetReasignacionesAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Reasignacion>>("api/reasignaciones");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo reasignaciones: {ex.Message}");
                return new List<Reasignacion>();
            }
        }

        public async Task<List<Reasignacion>?> GetReasignacionesPorTurnoAsync(int turnoId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Reasignacion>>($"api/reasignaciones/turno/{turnoId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo reasignaciones por turno: {ex.Message}");
                return new List<Reasignacion>();
            }
        }

        public async Task<Reasignacion?> ReasignarTurnoAsync(ReasignacionRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/reasignaciones", request);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Reasignacion>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error en reasignación: {response.StatusCode} - {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al reasignar turno: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Clinica>?> GetClinicasAsync()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Clinica>>("api/clinicas");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error obteniendo clínicas: {ex.Message}");
                return new List<Clinica>();
            }
        }
    }
}