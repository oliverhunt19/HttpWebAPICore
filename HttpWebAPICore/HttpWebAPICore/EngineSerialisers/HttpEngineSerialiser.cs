namespace HttpWebAPICore
{
    public abstract class HttpEngineSerialiser<TResponse>
    {
        public abstract ValueTask<TResponse> DeserializeAsync(Stream rawResponce, CancellationToken cancellationToken);
    }
}
