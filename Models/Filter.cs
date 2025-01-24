using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Timestamp_Backend.Models;

public record class ThresholdRecord
{
    [JsonPropertyName("Value")]
    public double Value { get; set; }
    [JsonPropertyName("Grayscale")]
    public bool Grayscale { get; set; }
}

public record class TintRecord
{
    [JsonPropertyName("R")]
    public int R { get; set; }
    [JsonPropertyName("G")]
    public int G { get; set; }
    [JsonPropertyName("B")]
    public int B { get; set; }
}

public record class FilterRecord
{
    [JsonPropertyName("Grayscale")]
    public bool Grayscale { get; set; }
    [JsonPropertyName("Invert")]
    public bool Invert { get; set; }
    [JsonPropertyName("Brightness")]
    public double Brightness { get; set; }
    [JsonPropertyName("Saturation")]
    public double Saturation { get; set; }
    [JsonPropertyName("Hue")]
    public int Hue { get; set; }
    [JsonPropertyName("Lightness")]
    public double Lightness { get; set; }
    [JsonPropertyName("Contrast")]
    public double Contrast { get; set; }
    [JsonPropertyName("Exposure")]
    public double Exposure { get; set; }
    [JsonPropertyName("Threshold")]
    public ThresholdRecord Threshold { get; set; } = null!;
    [JsonPropertyName("Tint")]
    public TintRecord Tint { get; set; } = null!;
}

public class Filter
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("Name")]
    public string Name { get; set; } = null!;
    [BsonElement("Preset")]
    public FilterRecord Preset { get; set; } = null!;
}
