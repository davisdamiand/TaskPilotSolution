using Shared.DTOs;
using System.Net.Http.Json;

namespace TaskPilot.Client.Services;

public class StudentService
{
    private readonly HttpClient _httpClient;

    public StudentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<int> CreateStudentAsync(StudentCreateDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Student/CreateStudent", dto);

        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync());

        return await response.Content.ReadFromJsonAsync<int>();
    }
}
