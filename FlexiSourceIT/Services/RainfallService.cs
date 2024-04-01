using FlexiSourceIT.Exceptions;
using FlexiSourceIT.Models;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace FlexiSourceIT.Services
{
    public class RainfallService : IRainfallService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        public RainfallService(HttpClient httpClient, ILogger<RainfallService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task<IEnumerable<RainfallReading>> GetRainfallReadingsAsync(string stationId)
        {
            if (string.IsNullOrEmpty(stationId))
            {
                throw new ArgumentNullException("Station ID cannot be null or empty", nameof(stationId));
            }

            var requestUrl = $"https://environment.data.gov.uk/flood-monitoring/id/stations/{stationId}";

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.GetAsync(requestUrl);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to fetch rainfall reeadings from external API");
                throw new Exception("Failed to fetch rainfall readings from external API", ex);
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                var errorDetail = JsonConvert.DeserializeObject<ErrorResponse>(errorResponse);

                if (errorDetail != null) 
                {
                    _logger.LogError($"Error response from external API: {errorDetail.Message}");
                    throw new ExternalApiException("Error response response from external API", errorDetail);
                }
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var readings = JsonConvert.DeserializeObject<List<RainfallReading>>(responseBody);

            if(readings == null || readings.Count == 0)
            {
                throw new Exception("Failed to desirialized rainfall readings from the API response");
            }
        }
    }
}
