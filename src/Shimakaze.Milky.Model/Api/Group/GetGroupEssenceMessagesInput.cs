using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Api.Group;

public sealed record class GetGroupEssenceMessagesInput(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("page_index")] int PageIndex,
    [property: JsonPropertyName("page_size")] int PageSize
);
