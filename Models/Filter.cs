using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Timestamp_Backend.Models;

public record class GrayscaleRecord
{
    [JsonPropertyName("mode")]
    public string? Mode { get; set; } = null!;
}

public record class BlendRecord
{
    [JsonPropertyName("color")]
    public string? Color { get; set; } = null!;
    [JsonPropertyName("mode")]
    public string? Mode { get; set; } = null!;
    [JsonPropertyName("alpha")]
    public double? Alpha { get; set; }
}

public record class BlurRecord
{
    [JsonPropertyName("blur")]
    public double? Blur { get; set; }
    [JsonPropertyName("horizontal")]
    public bool? Horizontal { get; set; }
    [JsonPropertyName("aspectRatio")]
    public double? AspectRatio { get; set; }
}

public record class GammaRecord
{
    [JsonPropertyName("gamma")]
    public List<double>? Gamma { get; set; } = null!;
}

public record class FilterRecord
{
    [JsonPropertyName("grayscale")]
    public GrayscaleRecord? Grayscale { get; set; }
    [JsonPropertyName("sepia")]
    public bool? Sepia { get; set; }
    [JsonPropertyName("polaroid")]
    public bool? Polaroid { get; set; }
    [JsonPropertyName("blackwhite")]
    public bool? Blackwhite { get; set; }
    [JsonPropertyName("brownie")]
    public bool? Brownie { get; set; }
    [JsonPropertyName("kodachrome")]
    public bool? Kodachrome { get; set; }
    [JsonPropertyName("technicolor")]
    public bool? Technicolor { get; set; }
    [JsonPropertyName("vintage")]
    public bool? Vintage { get; set; }
    [JsonPropertyName("blend")]
    public List<BlendRecord>? Blend { get; set; }
    [JsonPropertyName("blur")]
    public BlurRecord? Blur { get; set; }
    [JsonPropertyName("contrast")]
    public double? Contrast { get; set; }
    [JsonPropertyName("gamma")]
    public GammaRecord? Gamma { get; set; }
    [JsonPropertyName("noise")]
    public double? Noise { get; set; }
    [JsonPropertyName("pixelate")]
    public double? Pixelate { get; set; }
    [JsonPropertyName("saturation")]
    public double? Saturation { get; set; }
    [JsonPropertyName("vibrance")]
    public double? Vibrance { get; set; }
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
