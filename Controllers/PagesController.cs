using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Timestamp_Backend.Contracts;
using Timestamp_Backend.Models;
using Timestamp_Backend.Services;

namespace Timestamp_Backend.Controllers;

[ApiController]
public class PageController(DatabaseService database, IAuthenticationService authentication, StorageService storage) : Controller
{
    private readonly DatabaseService _database = database;
    private readonly IAuthenticationService _authentication = authentication;
    private readonly StorageService _storage = storage;

    [HttpPost("api/pages")]
    public async Task<ActionResult> Create([FromHeader(Name = "Token")] string id, CreatePageRecord createPage)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        Booth booth = await _database.GetCollection<Booth>("booths").Find(x => x.Id == id).FirstOrDefaultAsync();
        if (booth == null) return Unauthorized();
        List<string> imageIds = [];
        for(int i = 0; i < createPage.ImageCount; i++)
        {
            Image image = new();
            await _database.GetCollection<Image>("images").InsertOneAsync(image);
            imageIds.Add(image.Id);
        }
        Video video = new();
        await _database.GetCollection<Video>("videos").InsertOneAsync(video);
        Page page = new()
        {
            ImageIds = imageIds,
            VideoId = video.Id,
        };
        await _database.GetCollection<Page>("pages").InsertOneAsync(page);
        return CreatedAtRoute(new {id = page.Id}, new ReturnDataRecord<ReturnPageRecord>(new ReturnPageRecord(page.Id, [.. await Task.WhenAll(imageIds.Select(async x => new ReturnImageRecord(x, await _storage.UploadImageUrl(x))))], new ReturnVideoRecord(video.Id, await _storage.UploadVideoUrl(video.Id)))));
    }

    [HttpGet("views/{id:length(24)}")]
    public async Task<ViewResult> Index(string id)
    {
        Page page = await _database.GetCollection<Page>("pages").Find(x => x.Id == id).FirstOrDefaultAsync();
        if(page == null) return View("Views/Pages/invalid.cshtml");
        List<string> images = [.. await Task.WhenAll(page.ImageIds.Select(async x => await _storage.GetImageUrl(x)))];
        string video = await _storage.GetVideoUrl(page.VideoId);
        ViewBag.images = images;
        ViewBag.video = video;
        return View("Views/Pages/index.cshtml");
    }
}
