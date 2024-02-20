namespace HttpWebAPICore.Interfaces;

/// <summary>
/// Base interface for requests.
/// </summary>
public interface IRequest
{
    /// <summary>
    /// Gets the request message for the http client
    /// </summary>
    /// <returns></returns>
    HttpRequestMessage GetHttpRequestMessage();

    
}