using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace TaskPilot.Client.Services
{
    public class StatsService
    {
        private readonly HttpClient _httpClient;

        public StatsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> CreateStatsDefaultAsync(StatsCreateDto statsCreateDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Stats/CreateStats", statsCreateDto);
            if (!response.IsSuccessStatusCode)
                throw new Exception(await response.Content.ReadAsStringAsync());
            return await response.Content.ReadFromJsonAsync<int>();
        }

    }
}
