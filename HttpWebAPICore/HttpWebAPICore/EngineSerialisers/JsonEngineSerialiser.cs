using System.Text.Json;
using System.Text.Json.Serialization;

namespace HttpWebAPICore.EngineSerialisers
{
    public class JsonEngineSerialiser<TResponse> : HttpEngineSerialiser<TResponse>
    {
        /// <summary>
        /// Json Serializer Options.
        /// </summary>
        internal static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
        {
            new EnumJsonConverterFactory(JsonNamingPolicy.CamelCase, true),
            //new StringObjectConverterFactory(),
        },
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,

        };

        public override ValueTask<TResponse> DeserializeAsync(Stream rawResponce, CancellationToken cancellationToken)
        {
            return JsonSerializer.DeserializeAsync<TResponse>(rawResponce, jsonSerializerOptions, cancellationToken);
        }
    }
}
