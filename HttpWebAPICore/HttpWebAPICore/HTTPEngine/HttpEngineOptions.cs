namespace HttpWebAPICore.HTTPEngine;

/// <summary>
/// Http Engine Options.
/// </summary>
public class HttpEngineOptions
{
    /// <summary>
    /// Throw On Bad Request.
    /// </summary>
    public virtual bool ThrowOnInvalidRequest { get; set; } = true;

    public Uri DefaultConnectionCheck { get; set; } = new Uri(@"http://www.google.com");
}