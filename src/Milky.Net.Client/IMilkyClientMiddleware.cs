using System.Text.Json.Serialization.Metadata;

namespace Milky.Net.Client;

/// <summary>
/// Milky 客户端中间件
/// </summary>
public interface IMilkyClientMiddleware
{
    /// <summary>
    /// 运行中间件
    /// </summary>
    /// <typeparam name="TRequest">请求体类型</typeparam>
    /// <typeparam name="TResponse">响应体类型</typeparam>
    /// <param name="api">请求的 Api 端点</param>
    /// <param name="request">请求对象</param>
    /// <param name="next">继续执行下一个中间件，它将返回响应对象</param>
    /// <param name="cancellationToken">取消令牌</param>
    Task Execute<TRequest, TResponse>(string api, TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken);
}
