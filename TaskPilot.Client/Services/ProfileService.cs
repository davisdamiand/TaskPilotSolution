using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

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
            try
            {
                //1. Get response from the API
                var response = await _httpClient.PostAsJsonAsync("api/Stats/GetStats", dto);

                //2. Check if the response is successful
                if (!response.IsSuccessStatusCode)
                {
                    // return a default object when server returns an error to avoid crashing the client
                    return new StatsSendDto();
                }

                //3. Read the result from the response
                var result = await response.Content.ReadFromJsonAsync<StatsSendDto>();
                return result ?? new StatsSendDto();
            }
            catch (Exception)
            {
                // network/server error - return default stats to keep the UI responsive
                return new StatsSendDto();
            }
        }

        public async Task<List<StudentLeague>> GetLeagueStudentsAsync(int currentStudentID)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Stats/GetLeague", currentStudentID);
            if (!response.IsSuccessStatusCode)
                throw new Exception(await response.Content.ReadAsStringAsync());
            return await response.Content.ReadFromJsonAsync<List<StudentLeague>>();
        }
    }
}
