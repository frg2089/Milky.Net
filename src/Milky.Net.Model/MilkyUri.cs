using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Milky.Net.Model;

[JsonConverter(typeof(MilkyUriJsonConverter))]
public sealed record class MilkyUri(string OriginalUri)
{
    public bool IsFile => OriginalUri.StartsWith("file://", StringComparison.OrdinalIgnoreCase);
    public bool IsHttp => OriginalUri.StartsWith("http://", StringComparison.OrdinalIgnoreCase);
    public bool IsHttps => OriginalUri.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
    public bool IsBase64 => OriginalUri.StartsWith("base64://", StringComparison.OrdinalIgnoreCase);

    public string Scheme => OriginalUri[..OriginalUri.IndexOf(':')];

    public bool TryGetUri([NotNullWhen(true)] out Uri? uri) => Uri.TryCreate(OriginalUri, UriKind.Absolute, out uri);

}
