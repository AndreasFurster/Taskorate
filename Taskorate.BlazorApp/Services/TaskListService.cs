using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Taskorate.Models;

namespace Taskorate.BlazorApp.Services
{
    public class TaskListService
    {
        private readonly HttpClient _httpClient;

        public TaskListService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<QuickTaskList> Add(QuickTaskList taskList)
        {
            var response = await _httpClient.PostAsJsonAsync("task-lists", taskList);
            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<QuickTaskList>(content, new JsonSerializerOptions{ PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}
