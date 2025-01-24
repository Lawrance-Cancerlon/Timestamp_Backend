using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Timestamp_Backend.Models;

public class BoothLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Message")]
    public string Message { get; set; } = null!;
    [BsonElement("Level")]
    public string Level { get; set; } = null!;
    [BsonElement("BoothId")]
    public string BoothId { get; set; } = null!;
    [BsonElement("Timestamp")]
    public string Timestamp { get; set; } = null!;
    [BsonElement("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
