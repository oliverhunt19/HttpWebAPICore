namespace HttpWebAPICore
{
    public abstract class HttpEngineSerialiser<TResponse>
        where TResponse : class
    {
        public abstract ValueTask<TResponse?> DeserializeAsync(Stream rawResponce, CancellationToken cancellationToken);
    }
}
