using System;
using Google.Cloud.Storage.V1;

namespace Timestamp_Backend.Services;

public class StorageService(StorageClient client, UrlSigner urlSigner)
{
    private readonly StorageClient _client = client;
    private readonly UrlSigner _urlSigner = urlSigner;
    private readonly UrlSigner.RequestTemplate _bucket = UrlSigner.RequestTemplate.FromBucket("timestamp-storage");

    public async Task<string> UploadFrameUrl(string id)
    {
        UrlSigner.Options options = UrlSigner.Options.FromDuration(TimeSpan.FromDays(1));
        return await _urlSigner.SignAsync(_bucket.WithHttpMethod(HttpMethod.Put).WithObjectName("frames/" + id + ".png"), options);
    }

    public async Task<string> GetFrameUrl(string id)
    {
        UrlSigner.Options options = UrlSigner.Options.FromDuration(TimeSpan.FromDays(1));
        return await _urlSigner.SignAsync(_bucket.WithHttpMethod(HttpMethod.Get).WithObjectName("frames/" + id + ".png"), options);
    }

    public async void DeleteFrame(string id)
    {
        await _client.DeleteObjectAsync("timestamp-storage", "frames/" + id + ".png");
    }

    public async Task<string> UploadImageUrl(string id)
    {
        UrlSigner.Options options = UrlSigner.Options.FromDuration(TimeSpan.FromDays(1));
        return await _urlSigner.SignAsync(_bucket.WithHttpMethod(HttpMethod.Put).WithObjectName("images/" + id + ".jpg"), options);
    }

    public async Task<string> GetImageUrl(string id)
    {
        UrlSigner.Options options = UrlSigner.Options.FromDuration(TimeSpan.FromDays(1));
        return await _urlSigner.SignAsync(_bucket.WithHttpMethod(HttpMethod.Get).WithObjectName("images/" + id + ".jpg"), options);
    }

    public async Task<string> UploadThemeUrl(string id)
    {
        UrlSigner.Options options = UrlSigner.Options.FromDuration(TimeSpan.FromDays(1));
        return await _urlSigner.SignAsync(_bucket.WithHttpMethod(HttpMethod.Put).WithObjectName("themes/" + id + ".png"), options);
    }

    public async Task<string> GetThemeUrl(string id)
    {
        UrlSigner.Options options = UrlSigner.Options.FromDuration(TimeSpan.FromDays(1));
        return await _urlSigner.SignAsync(_bucket.WithHttpMethod(HttpMethod.Get).WithObjectName("themes/" + id + ".png"), options);
    }

    public async void DeleteTheme(string id)
    {
        await _client.DeleteObjectAsync("timestamp-storage", "themes/" + id + ".png");
    }

    public async Task<string> UploadVideoUrl(string id)
    {
        UrlSigner.Options options = UrlSigner.Options.FromDuration(TimeSpan.FromDays(1));
        return await _urlSigner.SignAsync(_bucket.WithHttpMethod(HttpMethod.Put).WithObjectName("videos/" + id + ".mp4"), options);
    }

    public async Task<string> GetVideoUrl(string id)
    {
        UrlSigner.Options options = UrlSigner.Options.FromDuration(TimeSpan.FromDays(1));
        return await _urlSigner.SignAsync(_bucket.WithHttpMethod(HttpMethod.Get).WithObjectName("videos/" + id + ".mp4"), options);
    }
}
