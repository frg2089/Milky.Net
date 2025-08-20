using System.Text.Json.Serialization;

using Shimakaze.Milky.Model.Common;

namespace Shimakaze.Milky.Model.Api.File;

public sealed record class GetGroupFilesOutput(
    [property: JsonPropertyName("files")] GroupFileEntity[] Files,
    [property: JsonPropertyName("folders")] GroupFolderEntity[] Folders
);
