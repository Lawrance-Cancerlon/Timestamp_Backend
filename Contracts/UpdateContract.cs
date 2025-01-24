using System.Text.Json.Serialization;
using Timestamp_Backend.Models;

namespace Timestamp_Backend.Contracts;

public record class UpdateBoothRecord
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    [JsonPropertyName("clientKey")]
    public string? ClientKey { get; set; }
    [JsonPropertyName("serverKey")]
    public string? ServerKey { get; set; }
    [JsonPropertyName("themeId")]
    public string? ThemeId { get; set; }
    [JsonPropertyName("frameIds")]
    public List<string>? FrameIds { get; set; }

    public Booth UpdateBooth(Booth booth)
    {
        return new Booth
        {
            Id = booth.Id,
            Name = Name ?? booth.Name,
            Description = Description ?? booth.Description,
            ClientKey = ClientKey ?? booth.ClientKey,
            ServerKey = ServerKey?? booth.ServerKey,
            ThemeId = ThemeId ?? booth.ThemeId,
            FrameIds = FrameIds ?? booth.FrameIds
        };
    }
}

public record class UpdateFilterRecord
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("preset")]
    public FilterRecord? Preset { get; set; }

    public Filter UpdateFilter(Filter filter)
    {
        return new Filter
        {
            Id = filter.Id,
            Name = Name ?? filter.Name,
            Preset = Preset ?? filter.Preset
        };
    }
}

public record class UpdateFrameRecord
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("themeId")]
    public string? ThemeId { get; set; }
    [JsonPropertyName("price")]
    public int? Price { get; set; }
    [JsonPropertyName("count")]
    public int? Count { get; set; }
    [JsonPropertyName("layouts")]
    public List<LayoutRecord>? Layouts { get; set; }
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    public Frame UpdateFrame(Frame frame)
    {
        return new Frame
        {
            Id = frame.Id,
            Name = Name?? frame.Name,
            ThemeId = ThemeId ?? frame.ThemeId,
            Price = Price?? frame.Price,
            Count = Count?? frame.Count,
            Layouts = Layouts?? frame.Layouts,
            Url = Url ?? frame.Url
        };
    }
}

public record class UpdateThemeRecord
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    [JsonPropertyName("config")]
    public string? Config { get; set; }

    public Theme UpdateTheme(Theme theme)
    {
        return new Theme
        {
            Id = theme.Id,
            Name = Name ?? theme.Name,
            Config = Config ?? theme.Config
        };
    }
}

public record class UpdateUserRecord
{
    [JsonPropertyName("name")]
    public string? Name { get; set;}
    [JsonPropertyName("email")]
    public string? Email { get; set;}
    [JsonPropertyName("password")]
    public string? Password { get; set; }

    public User UpdateUser(User user)
    {
        return new User
        {
            Id = user.Id,
            Name = Name ?? user.Name,
            IdentityId = user.IdentityId
        };
    }
}