using HttpWebAPICore.Interfaces;

namespace HttpWebAPICore
{
    public class APIBase<TRequest,TResponce> : HttpEngine<TRequest,TResponce>, IApi<TRequest,TResponce>
        where TRequest : IRequest
        where TResponce : IResponse<TRequest>, new()
    {
        public APIBase(HttpEngineSerialiser<TResponce> engineSerialiser) : base(engineSerialiser)
        {

        }

        public APIBase() : base()
        {

        }
    }
}
