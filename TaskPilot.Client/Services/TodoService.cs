using Shared.DTOs;
using Shared.Security;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

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

        public async Task<List<TodoGetDto>> GetTodosAsync(int studentID)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Todo/GetTodos", studentID);

            if (!response.IsSuccessStatusCode)
                throw new Exception(await response.Content.ReadAsStringAsync());
            //order by prioritySelection
            var todos = await response.Content.ReadFromJsonAsync<List<TodoGetDto>>();
            return todos.OrderBy(t => t.PrioritySelection).ToList();
        }
        
        // Proper error handeling 
        public async Task<bool> UpdateTodo(TodoUpdateDto todoUpdateDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Todo/UpdateTodo", todoUpdateDto);

            if (!response.IsSuccessStatusCode)
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                var error = JsonSerializer.Deserialize<ErrorResponse>(errorJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // You can now surface structured error info
                throw new ApplicationException(error?.Message ?? "Unknown error");
            }

            return true;
        }

        // true the toggle changed
        public async Task<bool> ToggleCompletion(int todoID)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Todo/ToggleCompletion", todoID);

            if (!response.IsSuccessStatusCode)
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                var error = JsonSerializer.Deserialize<ErrorResponse>(errorJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                throw new ApplicationException(error?.Message ?? "Unknown error");
            }

            return true;
        }

        public async Task<bool> DeleteTodoAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Todo/TodoDelete?id={id}");

            if (response.IsSuccessStatusCode)
            {
                // Optionally read the success message
                var successMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine(successMessage);
                return true;
            }

            // Handle structured error
            var errorJson = await response.Content.ReadAsStringAsync();
            try
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(
                    errorJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (error != null)
                {
                    // You can surface this in the UI
                    throw new ApplicationException(error.Message);
                }
            }
            catch (JsonException)
            {
                // Fallback if server didn't return ErrorResponse
                throw new ApplicationException($"Unexpected error: {errorJson}");
            }

            return false;
        }

        public async Task<bool> UpdateTimeSpentAsync(int id, int minutes)
        {
            // Build the request URL with query parameters
            var requestUrl = $"api/Todo/UpdateTime?id={id}&minutes={minutes}";

            // Send PATCH request (no body, just query params)
            var response = await _httpClient.PatchAsync(requestUrl, content: null);

            if (response.IsSuccessStatusCode)
            {
                // Optionally read the success message
                var successMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine(successMessage);
                return true;
            }

            // Handle structured error
            var errorJson = await response.Content.ReadAsStringAsync();
            try
            {
                var error = JsonSerializer.Deserialize<ErrorResponse>(
                    errorJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (error != null)
                {
                    // Surface structured error message
                    throw new ApplicationException(error.Message);
                }
            }
            catch (JsonException)
            {
                // Fallback if server didn't return ErrorResponse
                throw new ApplicationException($"Unexpected error: {errorJson}");
            }

            return false;
        }

    }
}
