using HttpWebAPICore.HTTPEngine;
using HttpWebAPICore.Interfaces;

namespace HttpWebAPICore.BaseClasses
{
    public abstract class APIBase<TRequest, TResponce> : HttpEngine<TRequest, TResponce>, IApi<TRequest, TResponce>
        where TRequest : class, IRequest
        where TResponce : class, IResponse<TRequest>, new()
    {
        public APIBase(HttpEngineSerialiser<TResponce> engineSerialiser) : base(engineSerialiser)
        {

        }

        public APIBase() : base()
        {

        }
    }
}
