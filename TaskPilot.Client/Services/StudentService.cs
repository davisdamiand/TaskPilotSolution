using Shared.DTOs;
using System.Net.Http.Json;

namespace TaskPilot.Client.Services
{
    public class StudentService
    {
        private readonly HttpClient _httpClient;

        public StudentService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> RegisterStudentWithDefaultsAsync(StudentCreateDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Student/Register", dto);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }

            return await response.Content.ReadFromJsonAsync<int>();
        }
    }
}



