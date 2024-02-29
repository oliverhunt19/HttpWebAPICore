using System.Xml;
using System.Xml.Serialization;

namespace HttpWebAPICore.EngineSerialisers
{
    public class XMLEngineSerialiser<T> : HttpEngineSerialiser<T>
        where T : class
    {
        public override ValueTask<T?> DeserializeAsync(Stream rawResponce, CancellationToken cancellationToken)
        {
            XmlReader xmlReader = new XmlTextReader(rawResponce);
            object? res = new XmlSerializer(typeof(T)).Deserialize(xmlReader);
            T? result = res as T;

            return ValueTask.FromResult(result);
        }
    }
}
