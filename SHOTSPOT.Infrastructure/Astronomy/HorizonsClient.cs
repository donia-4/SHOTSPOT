using SHOTSPOT.Application.Common.Interfaces;

namespace SHOTSPOT.Infrastructure.Astronomy;

public class HorizonsClient : IEphemerisProvider
{
    private readonly HttpClient _httpClient;

    public HorizonsClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
}