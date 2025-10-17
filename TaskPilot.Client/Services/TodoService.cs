using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace TaskPilot.Client.Services
{
    public class TodoService
    {
        private readonly HttpClient _httpClient;

        public TodoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> CreateTodoAsync(TodoCreateDto todoCreateDto)
        {
            // 1. Get response from the API
            var response = await _httpClient.PostAsJsonAsync("api/Todo/CreateTodo", todoCreateDto);

            //2. Check if the response is successful
            if (!response.IsSuccessStatusCode)
                throw new Exception(await response.Content.ReadAsStringAsync());

            //3. Read the result from the response
            return await response.Content.ReadFromJsonAsync<int>();
        }
    }
}
