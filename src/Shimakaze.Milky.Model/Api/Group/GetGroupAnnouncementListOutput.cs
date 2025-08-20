using System.Text.Json.Serialization;

using Shimakaze.Milky.Model.Common;

namespace Shimakaze.Milky.Model.Api.Group;

public sealed record class GetGroupAnnouncementListOutput(
    [property: JsonPropertyName("announcements")] GroupAnnouncementEntity[] Announcements
);
