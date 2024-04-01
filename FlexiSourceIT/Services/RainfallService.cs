using FlexiSourceIT.Exceptions;
using FlexiSourceIT.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq; // Add this namespace
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

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
                throw new ArgumentNullException(nameof(stationId), "Station ID cannot be null or empty");
            }

            var requestUrl = $"https://environment.data.gov.uk/flood-monitoring/id/stations/{stationId}";

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.GetAsync(requestUrl);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to fetch rainfall readings from external API");
                throw new Exception("Failed to fetch rainfall readings from external API", ex);
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadAsStringAsync();
                var errorDetail = JsonConvert.DeserializeObject<ErrorResponse>(errorResponse);

                if (errorDetail != null)
                {
                    _logger.LogError($"Error response from external API: {errorDetail.Message}");
                    throw new ExternalApiException("Error response from external API", errorDetail);
                }
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var jsonObject = JObject.Parse(responseBody);
            var items = jsonObject["items"];
            var latestReading = items?["measures"]?["latestReading"];

            if (latestReading != null)
            {
                var dateMeasuredString = latestReading?["dateTime"]?.ToObject<string>();
                if (dateMeasuredString == null)
                {
                    throw new Exception("dateMeasuredString is null");
                }

                var dateMeasured = DateTime.Parse(dateMeasuredString);

                var amountMeasured = latestReading?["value"]?.ToObject<double>();
                if (amountMeasured == null)
                {
                    throw new Exception("amountMeasured is null");
                }

                var decimalAmountMeasured = (decimal)amountMeasured;

                var rainfallReading = new RainfallReading
                {
                    DateMeasured = dateMeasured,
                    AmountMeasured = decimalAmountMeasured
                };

                return new List<RainfallReading> { rainfallReading };
            }
            else
            {
                throw new Exception("latestReading is null");
            }

        }
    }
}
