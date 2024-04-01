using FlexiSourceIT.Models;

namespace FlexiSourceIT.Services
{
    public interface IRainfallService
    {
        Task<IEnumerable<RainfallReading>> GetRainfallReadingsAsync(string stationId);
    }
}
