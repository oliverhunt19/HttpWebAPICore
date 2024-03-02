using HttpWebAPICore.Interfaces;
using System.Text.Json.Serialization;

namespace HttpWebAPICore.BaseClasses;

/// <summary>
/// Base abstract class for responses.
/// </summary>
public abstract class BaseResponse<T> : IResponse<T>
    where T : IRequest
{
    /// <inheritdoc />
    public virtual string RawJson { get; set; }

    /// <inheritdoc />
    public virtual Uri? RequestUri { get; set; }

    /// <inheritdoc />
    public virtual Status Status { get; set; }

    /// <inheritdoc />
    [JsonPropertyName("error_message")]
    public virtual string ErrorMessage { get; set; }

    /// <summary>
    /// Info Messages.
    /// </summary>
    [JsonPropertyName("info_messages")]
    public virtual IEnumerable<string> InfoMessages { get; set; }

    /// <summary>
    /// Html Attributions.
    /// </summary>
    [JsonPropertyName("html_attributions")]
    public virtual IEnumerable<string> HtmlAttributions { get; set; }

    public T Request { get; set; }

    public BaseResponse()
    {
        RawJson = "";
        InfoMessages = new List<string>();
        HtmlAttributions = new List<string>();
        ErrorMessage = "";
    }

    public virtual Task PostProcess()
    {
        // Does nothing by default
        return Task.CompletedTask;
    }
}