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
            // Call the API to register the student
            var response = await _httpClient.PostAsJsonAsync("api/Student/Register", dto);

            // If the server returns an error, throw it so the UI can display it.
            if (!response.IsSuccessStatusCode)
            {
                // Log the error
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
            // Return the newly created student's ID
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



