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

public record class ReturnListRecord<T>
{
    [JsonPropertyName("data")]
    public List<T> Data { get; set; }

    public ReturnListRecord(List<T> data)
    {
        Data = data;
    }
}

public record class ReturnTokenRecord
{
    [JsonPropertyName("data")]
    public User Data { get; set; }
    [JsonPropertyName("token")]
    public string Token { get; set; }

    public ReturnTokenRecord(User data, string token)
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
    public Theme Theme { get; set; } = null!;
    [JsonPropertyName("frames")]
    public List<Frame> Frames { get; set; } = null!;
    [JsonPropertyName("filters")]
    public List<Filter> Filters { get; set; } = null!;
}