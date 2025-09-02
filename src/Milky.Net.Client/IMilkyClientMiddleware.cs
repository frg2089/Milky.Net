using System.Text.Json.Serialization.Metadata;

namespace Milky.Net.Client;

public interface IMilkyClientMiddleware
{
    void PreRequest<TRequest, TResponse>(string api, TRequest request, JsonTypeInfo<TRequest> reqTypeInfo, JsonTypeInfo<TResponse> resTypeInfo);
    void PostRequest<TRequest, TResponse>(string api, TRequest request, TResponse response, JsonTypeInfo<TRequest> reqTypeInfo, JsonTypeInfo<TResponse> resTypeInfo);
}
