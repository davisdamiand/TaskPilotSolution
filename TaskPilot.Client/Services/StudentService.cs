using Shared.DTOs;
using Shared.Security;
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

        public async Task<bool> ResetPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Student/ResetPassword", forgotPasswordDto);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            // If the server returns an error, throw it so the UI can display it.
            var errorContent = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            throw new Exception(errorContent?.Message ?? "An unknown error occurred.");
        }
    }
}



