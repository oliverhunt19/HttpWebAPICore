using HttpWebAPICore.Interfaces;
using System.Text.Json.Serialization;

namespace HttpWebAPICore.BaseClasses;

/// <summary>
/// Base abstract class for requests.
/// </summary>
public abstract class BaseRequest : IRequest
{
    /// <summary>
    /// Base Url (abstract).
    /// </summary>
    [JsonIgnore]
    protected internal abstract string BaseUrl { get; }


    /// <summary>
    /// See <see cref="IRequest.GetUri()"/>.
    /// </summary>
    /// <returns>The <see cref="Uri"/>.</returns>
    public virtual Uri GetUri()
    {
        const string SCHEME = "https://";

        var queryStringParameters = GetQueryStringParameters()
            .Select(x =>
                x.Value == null
                    ? Uri.EscapeDataString(x.Key)
                    : Uri.EscapeDataString(x.Key) + "=" + Uri.EscapeDataString(x.Value));
        var queryString = string.Join("&", queryStringParameters);

        if (!string.IsNullOrEmpty(queryString))
        {
            queryString = $"?{queryString}";
        }

        var uri = new Uri($"{SCHEME}{BaseUrl}{queryString}");
        return uri;
    }

    /// <summary>
    /// See <see cref="IRequest.GetQueryStringParameters()"/>.
    /// </summary>
    /// <returns>The <see cref="IList{KeyValuePair}"/> collection.</returns>
    public virtual IList<KeyValuePair<string, string?>> GetQueryStringParameters()
    {
        var parameters = new List<KeyValuePair<string, string?>>();
        return parameters;
    }

    public virtual HttpRequestMessage GetHttpRequestMessage()
    {
        return new HttpRequestMessage(HttpMethod.Get, GetUri());
    }
}