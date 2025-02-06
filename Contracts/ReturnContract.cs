using System.Text.Json.Serialization;
using Timestamp_Backend.Models;

namespace Timestamp_Backend.Contracts;

public record class ReturnDataRecord<T>
{
    [JsonPropertyName("data")]
    public T Data { get; set; }

    public ReturnDataRecord(T data)
    {
        Data = data;
    }
}

public record class ReturnThemeRecord
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;

    public ReturnThemeRecord(Theme theme, string url)
    {
        Id = theme.Id;
        Name = theme.Name;
        Url = url;
    }
}

public record class ReturnPageRecord
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;
    [JsonPropertyName("images")]
    public List<ReturnImageRecord> Images { get; set; } = null!;
    [JsonPropertyName("video")]
    public ReturnVideoRecord Video { get; set; } = null!;

    public ReturnPageRecord(string pageId, List<ReturnImageRecord> images, ReturnVideoRecord video)
    {
        Id = pageId;
        Images = images;
        Video = video;
    }
}

public record class ReturnFrameRecord
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;
    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;
    [JsonPropertyName("themeId")]
    public string ThemeId { get; set; } = null!;
    [JsonPropertyName("count")]
    public int Count { get; set; }
    [JsonPropertyName("layouts")]
    public List<LayoutRecord> Layouts { get; set; } = null!;
    [JsonPropertyName("price")]
    public int Price { get; set; }
    [JsonPropertyName("split")]
    public bool Split { get; set; }
    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;

    public ReturnFrameRecord(Frame frame, string url)
    {
        Id = frame.Id;
        Name = frame.Name;
        ThemeId = frame.ThemeId;
        Count = frame.Count;
        Layouts = frame.Layouts;
        Price = frame.Price;
        Split = frame.Split;
        Url = url;
    }
}

public record class ReturnImageRecord
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;
    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;

    public ReturnImageRecord(string id, string url)
    {
        Id = id;
        Url = url;
    }
}

public record class ReturnVideoRecord
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;
    [JsonPropertyName("url")]
    public string Url { get; set; } = null!;

    public ReturnVideoRecord(string id, string url)
    {
        Id = id;
        Url = url;
    }
}

public record class ReturnTokenRecord<T>
{
    [JsonPropertyName("data")]
    public T Data { get; set; }
    [JsonPropertyName("token")]
    public string Token { get; set; }

    public ReturnTokenRecord(T data, string token)
    {
        Data = data;
        Token = token;
    }
}

public record ReturnInitRecord
{
    [JsonPropertyName("booth")]
    public Booth Booth { get; set; } = null!;
    [JsonPropertyName("theme")]
    public ReturnThemeRecord Theme { get; set; } = null!;
    [JsonPropertyName("frames")]
    public List<ReturnFrameRecord> Frames { get; set; } = null!;
    [JsonPropertyName("filters")]
    public List<Filter> Filters { get; set; } = null!;
}