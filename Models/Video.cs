using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Timestamp_Backend.Models;

public class Video
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Url")]
    public string Url { get; set; } = null!;
    [BsonElement("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
