using System.Text.Json.Serialization;
using Timestamp_Backend.Models;

namespace Timestamp_Backend.Contracts;

public record class CreateBoothRecord
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("description")]
    public string Description { get; set; } = null!;
    [JsonPropertyName("clientKey")]
    public string ClientKey { get; set; } = null!;
    [JsonPropertyName("serverKey")]
    public string ServerKey { get; set; } = null!;
    [JsonPropertyName("themeId")]
    public string ThemeId { get; set; } = null!;
    [JsonPropertyName("frameIds")]
    public List<string> FrameIds { get; set; } = null!;
}

public record class CreateBoothLogRecord
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = null!;
    [JsonPropertyName("level")]
    public int Level { get; set; }
    [JsonPropertyName("timestamp")]
    public string Timestamp { get; set; } = null!;
}

public record class CreateFilterRecord
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("preset")]
    public FilterRecord Preset { get; set; } = null!;
}

public record class CreateFrameRecord
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("themeId")]
    public string ThemeId { get; set; } = null!;
    [JsonPropertyName("count")]
    public int Count { get; set; }
    [JsonPropertyName("price")]
    public int Price { get; set; }
    [JsonPropertyName("layouts")]
    public List<LayoutRecord> Layouts { get; set; } = null!;
    [JsonPropertyName("split")]
    public bool Split { get; set; }
}

public record class CreatePageRecord
{
    [JsonPropertyName("imageCount")]
    public int ImageCount { get; set; }
}

public record class CreateThemeRecord
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
}

public record class CreateTransactionRecord
{
    [JsonPropertyName("frameId")]
    public string FrameId { get; set; } = null!;
}

public record class CreateUserRecord
{
    [JsonPropertyName("name")]
    public string Name { get; set;} = null!;
    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;
    [JsonPropertyName("password")]
    public string Password { get; set; } = null!;
}
