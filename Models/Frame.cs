using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Timestamp_Backend.Models;

public record class LayoutRecord
{
    [JsonPropertyName("X")]
    public double X { get; set; }
    [JsonPropertyName("Y")]
    public double Y { get; set; }
    [JsonPropertyName("Width")]
    public double Width { get; set; }
    [JsonPropertyName("Height")]
    public double Height { get; set; }
}

public class Frame
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Name")]
    public string Name { get; set; } = null!;
    [BsonElement("ThemeId")]
    public string ThemeId { get; set; } = null!;
    [BsonElement("Count")]
    public int Count { get; set; }
    [BsonElement("Layouts")]
    public List<LayoutRecord> Layouts { get; set; } = null!;
    [BsonElement("Price")]
    public int Price { get; set; }
}
