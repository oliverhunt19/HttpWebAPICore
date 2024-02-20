namespace HttpWebAPICore.Interfaces;

/// <summary>
/// Base interface for responses.
/// </summary>
public interface IResponse
{
    /// <summary>
    /// Raw json of the response.
    /// </summary>
    string RawJson { get; set; }

    /// <summary>
    /// Raw querystring of the request.
    /// </summary>
    Uri? RequestUri { get; set; }

    /// <summary>
    /// The status returned with the response.
    /// </summary>
    Status Status { get; set; }

    /// <summary>
    /// When the status code is other than 'Ok', there may be an additional error_message field within the response object.
    /// This field contains more detailed information about the reasons behind the given status code.
    /// Note: This field is not guaranteed to be always present, and its content is subject to change.
    /// </summary>
    string ErrorMessage { get; set; }

    Task PostProcess();
}

public interface IResponse<TRequest> : IResponse
    where TRequest : IRequest
{
    TRequest Request { get; set; }
}