using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Timestamp_Backend.Models;

public class Page
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("ImageIds")]
    public List<string> ImageIds { get; set; } = null!;
    [BsonElement("VideoId")]
    public string VideoId { get; set; } = null!;
    [BsonElement("CreatedAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
