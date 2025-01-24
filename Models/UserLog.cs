using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Timestamp_Backend.Models;

public class UserLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Message")]
    public string Message { get; set; } = null!;
    [BsonElement("UserId")]
    public string UserId { get; set; } = null!;
    [BsonElement("Timestamp")]
    public string Timestamp { get; set; } = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
    [BsonElement("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
