using System.Text;
using System.Text.Json;
using cldv6211proj.Models.ViewModels;
using Shared.Models;

namespace cldv6211proj.Services
{
    public static class DurableWrapperService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly string _durableFunctionBaseUrl = "http://localhost:7071/api";

        public static async Task StartOrderProcessing(int orderID)
        {
            new StringContent(
                JsonSerializer.Serialize(new { orderID }), // Ensure the payload is an object with an orderID property
                Encoding.UTF8,
                "application/json"
            );
            var content = new StringContent(
                JsonSerializer.Serialize(orderID),
                Encoding.UTF8,
                "application/json"
            );
            var response = await _httpClient.PostAsync(
                $"{_durableFunctionBaseUrl}/ProcessOrderOrchestration_HttpStart",
                content
            );
            response.EnsureSuccessStatusCode();
        }
    }
}
