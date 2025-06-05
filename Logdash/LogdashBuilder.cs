using Logdash.Constants;
using Logdash.Models;

namespace Logdash;

public class LogdashBuilder
{
    private HttpClient? _httpClient;
    private InitializationParams? _initializationParams;
    
    public LogdashBuilder WithHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        if (_initializationParams != null)
        {
            _httpClient.BaseAddress = new Uri(_initializationParams.Host ?? LogdashConstants.LogdashApiUrl);
            _httpClient.DefaultRequestHeaders.Add("project-api-key", _initializationParams.ApiKey);
        }
        
        return this;
    }

    public LogdashBuilder WithInitializationParams(InitializationParams? initializationParams)
    {
        _initializationParams = initializationParams;
        if (_initializationParams != null)
        {
            if (_httpClient != null)
            {
                _httpClient.BaseAddress = new Uri(_initializationParams.Host ?? LogdashConstants.LogdashApiUrl);
                _httpClient?.DefaultRequestHeaders.Add("project-api-key", _initializationParams.ApiKey);
            }
        }
        return this;
    }

    public (ILogdashLogger logdash, ILogdashMetrics metrics) Build()
    {
        var logdash =  new Logdash(_httpClient!, _initializationParams!);
        return (logdash, logdash);
    }
}