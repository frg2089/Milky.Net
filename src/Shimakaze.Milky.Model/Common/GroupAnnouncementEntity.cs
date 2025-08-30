using System.Text.Json.Serialization;

namespace Shimakaze.Milky.Model.Common;

/// <summary>
/// 群公告实体
/// </summary>
/// <param name="GroupId">群号</param>
/// <param name="AnnouncementId">公告 ID</param>
/// <param name="UserId">发送者 QQ 号</param>
/// <param name="Time">Unix 时间戳（秒）</param>
/// <param name="Content">公告内容</param>
/// <param name="ImageUrl">公告图片 URL</param>
public sealed record class GroupAnnouncementEntity(
    [property: JsonPropertyName("group_id")] long GroupId,
    [property: JsonPropertyName("announcement_id")] string AnnouncementId,
    [property: JsonPropertyName("user_id")] long UserId,
    [property: JsonPropertyName("time")][property: JsonConverter(typeof(SecondTimestampDateTimeOffsetJsonConverter))] DateTimeOffset Time,
    [property: JsonPropertyName("content")] string Content,
    [property: JsonPropertyName("image_url")] string? ImageUrl);
