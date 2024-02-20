using System.Xml;
using System.Xml.Serialization;

namespace HttpWebAPICore.EngineSerialisers
{
    public class XMLEngineSerialiser<T> : HttpEngineSerialiser<T>
    {
        public override ValueTask<T> DeserializeAsync(Stream rawResponce, CancellationToken cancellationToken)
        {
            T result;
            using(XmlReader xmlReader = new XmlTextReader(rawResponce))
            {
                result = (T)new XmlSerializer(typeof(T)).Deserialize(xmlReader);
            }

            return ValueTask.FromResult(result);
        }
    }
}
