using System.Diagnostics.CodeAnalysis;

namespace Milky.Net.Server;

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
public sealed class ApiEndpointAttribute([StringSyntax("Route")] string pattern) : Attribute
{
    public string Pattern { get; } = pattern;
}
