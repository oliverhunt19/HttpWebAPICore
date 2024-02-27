using HttpWebAPICore.EngineSerialisers;
using HttpWebAPICore.Interfaces;
using System.Net;
using System.Net.Sockets;
using System.Text;
using JsonException = System.Text.Json.JsonException;

namespace HttpWebAPICore;

/// <summary>
/// Http Engine.
/// </summary>
public class HttpEngine
{
    

    protected readonly HttpClient httpClient;

    protected HttpEngine(HttpClient httpClient)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        ServicePointManager.DefaultConnectionLimit = 10;
    }

    

    public static async Task<bool> IsConnected(Uri host)
    {
        Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            await s.ConnectAsync(host.Host, 80);
            return s.Connected;
        }
        catch (SocketException ex)
        {
            return false;
        }
        finally
        {
            await s.DisconnectAsync(false);
        }
    }

    public  async Task<bool> TestInternetConnection(string? url = null)
    {
        try
        {
            url ??= "http://www.gstatic.com/generate_204";
            var resp = await httpClient.GetAsync(url);
            return resp.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public static Task<bool> TestInternetConnection(TimeSpan? timeout = null, string? url = null)
    {
        HttpClient httpClient = new HttpClient()
        {
            Timeout = timeout ?? TimeSpan.FromSeconds(5),
        };
        HttpEngine httpEngine = new HttpEngine(httpClient);
        return httpEngine.TestInternetConnection(url);
    }
}

/// <summary>
/// Http Engine.
/// Manages the http connections, and is responsible for invoking request and handling responses.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class HttpEngine<TRequest, TResponse> : HttpEngine
    where TRequest : IRequest
    where TResponse : IResponse<TRequest>, new()
{
    protected HttpEngineSerialiser<TResponse> EngineSerialiser { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    protected HttpEngine()
        : this(HttpClientFactory.CreateDefaultHttpClient(), new JsonEngineSerialiser<TResponse>())
    {

    }

    protected HttpEngine(HttpEngineSerialiser<TResponse> engineSerialiser) : this(HttpClientFactory.CreateDefaultHttpClient(), engineSerialiser)
    {

    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="httpClient">The <see cref="httpClient"/>.</param>
    protected HttpEngine(HttpClient httpClient, HttpEngineSerialiser<TResponse> engineSerialiser) : base(httpClient)
    {
        EngineSerialiser = engineSerialiser;
    }

    /// <summary>
    /// Query Async.
    /// </summary>
    /// <param name="request">The request that will be sent.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Task{T}"/>.</returns>
    public async Task<TResponse> QueryAsync(TRequest request, CancellationToken cancellationToken = default)
    {
        if(request == null)
            throw new ArgumentNullException(nameof(request));

        return await QueryAsync(request, new HttpEngineOptions(), cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Query Async.
    /// </summary>
    /// <param name="request">The request that will be sent.</param>
    /// <param name="httpEngineOptions">The <see cref="HttpEngineOptions"/></param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>The <see cref="Task{T}"/>.</returns>
    public async Task<TResponse> QueryAsync(TRequest request, HttpEngineOptions httpEngineOptions, CancellationToken cancellationToken = default)
    {
        
        if(request == null)
            throw new ArgumentNullException(nameof(request));

        httpEngineOptions ??= new HttpEngineOptions();

        try
        {
            using var httpResponseMessage = await ProcessRequestAsync(request, httpEngineOptions, cancellationToken)
                .ConfigureAwait(false);

            var response = await ProcessResponseAsync(httpResponseMessage, request, cancellationToken)
                .ConfigureAwait(false);
            //return response;
            switch(response.Status)
            {
                case Status.Ok:
                case Status.NotFound:
                case Status.ZeroResults:
                case Status.ResourceExhausted:
                    return response;

                case Status.InvalidRequest:
                case Status.InvalidArgument:
                    if(!httpEngineOptions.ThrowOnInvalidRequest)
                    {
                        return response;
                    }

                    throw new InvalidOperationException($"{response.ErrorMessage ?? "No message"}")
                    {
                    };

                default:
                    throw new InvalidOperationException($"{response.ErrorMessage ?? "No message"}")
                    {
                    };
            }
        }
        catch(Exception ex)
        {
            //Uri uri = request
            //.GetUri();
            var baseException = ex
                .GetBaseException();

            throw new InvalidOperationException(baseException.Message, baseException);
        }
    }

    



    private async Task<HttpResponseMessage> ProcessRequestAsync(TRequest request, HttpEngineOptions options, CancellationToken cancellationToken = default)
    {
        if(request == null)
            throw new ArgumentNullException(nameof(request));

        using var httpRequestMessage = request.GetHttpRequestMessage();

        if(!await IsConnected(httpRequestMessage.RequestUri ?? options.DefaultConnectionCheck)
            .ConfigureAwait(false))
        {
            throw new InvalidOperationException("The http client cannot get a connection to the host");
        }

        return await httpClient
                    .SendAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);
    }
    private async Task<TResponse> ProcessResponseAsync(HttpResponseMessage httpResponse, TRequest request, CancellationToken cancellationToken = default)
    {
        if(httpResponse == null)
            throw new ArgumentNullException(nameof(httpResponse));


        TResponse response;
        if(httpResponse.StatusCode == HttpStatusCode.Forbidden)
        {
            response = new TResponse
            {
                RequestUri = httpResponse.RequestMessage?.RequestUri,
                Status = Status.PermissionDenied,
                ErrorMessage = httpResponse.ReasonPhrase ?? "",
            };

            return response;
        }

        Stream rawResponce = await httpResponse.Content
                    .ReadAsStreamAsync(cancellationToken)
                    .ConfigureAwait(false);

        response = await EngineSerialiser.DeserializeAsync(rawResponce,cancellationToken).ConfigureAwait(false) ?? new TResponse();

        response.RawJson = await rawResponce.StreamToString();
        response.Request = request;
        response.RequestUri = httpResponse.RequestMessage?.RequestUri;
        await response.PostProcess();

        return response;
    }


}

public static class HttpEngineExtensions
{
    public static string? GetLine(this string text, int lineNo)
    {
        string[] lines = text.Replace("\r", "").Split('\n');
        return lines.Length >= lineNo ? lines[lineNo - 1] : null;
    }

    public static string GetSnippet(this string text, int lineNo, int eitherSide)
    {
        List<string> snippetLines = new List<string>();
        string[] lines = text.Replace("\r", "").Split('\n');
        for(int i = 0; i < 2 * eitherSide + 1; i++)
        {
            int lineNumber = lineNo - eitherSide + i;
            string line = lines.Length >= lineNumber ? lines[lineNumber - 1] : "";
            snippetLines.Add(line);
        }
        return string.Join("\n", snippetLines);
    }

    public static Task<string> StreamToString(this Stream stream)
    {
        stream.Position = 0;
        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        {
            return reader.ReadToEndAsync();
        }
    }


}