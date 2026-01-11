using System.Text.Json;
using System.Text.Json.Serialization;

using Milky.Net.ModelGenerator.Models;

namespace Milky.Net.SourceGenerator.Common;

[JsonSourceGenerationOptions(WriteIndented = false)]
[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(JsonElement))]
[JsonSerializable(typeof(MilkyIR))]
public sealed partial class MilkyJsonSerializerContext : JsonSerializerContext
{
}
