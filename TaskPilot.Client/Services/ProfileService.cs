using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace TaskPilot.Client.Services
{
    public class ProfileService
    {
        private readonly HttpClient _httpClient;

        public ProfileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<StatsSendDto> GetStudentStatsAsync(StatsCalculateDto dto)
        {
            //1. Get response from the API
            var response = await _httpClient.PostAsJsonAsync("api/Stats/GetStats", dto);

            //2. Check if the response is successful
            if(!response.IsSuccessStatusCode)
                throw new Exception(await response.Content.ReadAsStringAsync());

            //3. Read the result from the response
            return await response.Content.ReadFromJsonAsync<StatsSendDto>();
        }
    }
}
