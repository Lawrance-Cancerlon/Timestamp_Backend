using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Timestamp_Backend.Models;

public class Booth
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Name")]
    public string Name { get; set; } = null!;
    [BsonElement("Description")]
    public string Description { get; set; } = null!;
    [BsonElement("Status")]
    public int Status { get; set; }
    [BsonElement("ClientKey")]
    public string ClientKey { get; set; } = null!;
    [BsonElement("ServerKey")]
    public string ServerKey { get; set; } = null!;
    [BsonElement("ThemeId")]
    public string ThemeId { get; set; } = null!;
    [BsonElement("FrameIds")]
    public List<string> FrameIds { get; set; } = null!;
}
