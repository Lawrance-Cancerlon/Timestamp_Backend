using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Timestamp_Backend.Contracts;
using Timestamp_Backend.Models;
using Timestamp_Backend.Services;

namespace Timestamp_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FramesController(DatabaseService database, IAuthenticationService authentication, StorageService storage) : ControllerBase
{
    private readonly DatabaseService _database = database;
    private readonly IAuthenticationService _authentication = authentication;
    private readonly StorageService _storage = storage;

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Create([FromHeader(Name = "Authorization")] string token, CreateFrameRecord createFrame)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        Frame frame = new()
        {
            Name = createFrame.Name,
            ThemeId = createFrame.ThemeId,
            Count = createFrame.Count,
            Price = createFrame.Price,
            Layouts = createFrame.Layouts,
            Split = createFrame.Split,
        };
        await _database.GetCollection<Frame>("frames").InsertOneAsync(frame);
        User actor = await _database.GetCollection<User>("users").Find(x => x.IdentityId == _authentication.GetIdentity(token)).FirstOrDefaultAsync();
        await _database.GetCollection<UserLog>("userLogs").InsertOneAsync(new UserLog
        {
            Message = "Created frame " + frame.Name,
            UserId = actor.Id, 
        });
        return CreatedAtRoute(new {id = frame.Id}, new ReturnDataRecord<ReturnFrameRecord>(new ReturnFrameRecord(frame, await _storage.UploadFrameUrl(frame.Id))));
    }

    [HttpGet]
    public async Task<ActionResult> Get([FromQuery] string? id, [FromQuery] string? themeId, [FromQuery] int? count, [FromQuery] bool? split, [FromQuery] string? boothId)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        var filterBuilder = Builders<Frame>.Filter;
        var filter = filterBuilder.Empty;
        if (id!= null) filter &= filterBuilder.Eq(x => x.Id, id);
        if (themeId!= null) filter &= filterBuilder.Eq(x => x.ThemeId, themeId);
        if (count!= null) filter &= filterBuilder.Eq(x => x.Count, count);
        if (split!= null) filter &= filterBuilder.Eq(x => x.Split, split);
        if (boothId!= null){
            Booth booth = await _database.GetCollection<Booth>("booths").Find(x => x.Id == boothId).FirstOrDefaultAsync();
            if (booth == null) return NotFound("Booth not found");
            filter &= filterBuilder.In(x => x.Id, booth.FrameIds);
        }
        return Ok(new ReturnDataRecord<List<ReturnFrameRecord>>([.. await Task.WhenAll((await _database.GetCollection<Frame>("frames").Find(filter).ToListAsync()).Select(async x => new ReturnFrameRecord(x, await _storage.GetFrameUrl(x.Id))))]));
    }

    [HttpGet("theme")]
    public async Task<ActionResult> GetTheme([FromHeader(Name = "Token")] string id)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        Booth booth = await _database.GetCollection<Booth>("booths").Find(x => x.Id == id).FirstOrDefaultAsync();
        if (booth == null) return Unauthorized();
        var frames = await _database.GetCollection<Frame>("frames").Find(Builders<Frame>.Filter.In(x => x.Id, booth.FrameIds)).ToListAsync();
        var uniqueThemes = frames.Select(x => x.ThemeId).Distinct().ToList();
        var themes = await _database.GetCollection<Theme>("themes").Find(Builders<Theme>.Filter.In(x => x.Id, uniqueThemes)).ToListAsync();
        return Ok(new ReturnDataRecord<List<Theme>>(themes));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> Update([FromHeader(Name = "Authorization")] string token, string id, UpdateFrameRecord updateFrame)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        Frame frame = await _database.GetCollection<Frame>("frames").Find(x => x.Id == id).FirstOrDefaultAsync();
        if (frame == null) return NotFound("Frame not found");
        frame = updateFrame.UpdateFrame(frame);
        await _database.GetCollection<Frame>("frames").ReplaceOneAsync(x => x.Id == id, frame);
        User actor = await _database.GetCollection<User>("users").Find(x => x.IdentityId == _authentication.GetIdentity(token)).FirstOrDefaultAsync();
        await _database.GetCollection<UserLog>("userLogs").InsertOneAsync(new UserLog
        {
            Message = "Updated frame " + frame.Name,
            UserId = actor.Id, 
        });
        return Ok(new ReturnDataRecord<ReturnFrameRecord>(new ReturnFrameRecord(frame, await _storage.UploadFrameUrl(frame.Id))));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete([FromHeader(Name = "Authorization")] string token, string id)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        Frame frame = await _database.GetCollection<Frame>("frames").Find(x => x.Id == id).FirstOrDefaultAsync();
        if (frame == null) return NotFound("Frame not found");
        await _database.GetCollection<Frame>("frames").DeleteOneAsync(x => x.Id == id);
        User actor = await _database.GetCollection<User>("users").Find(x => x.IdentityId == _authentication.GetIdentity(token)).FirstOrDefaultAsync();
        await _database.GetCollection<UserLog>("userLogs").InsertOneAsync(new UserLog
        {
            Message = "Deleted frame " + frame.Name,
            UserId = actor.Id, 
        });
        _storage.DeleteFrame(id);
        await _database.GetCollection<Booth>("booths").UpdateManyAsync(
            Builders<Booth>.Filter.AnyEq(x => x.FrameIds, id),
            Builders<Booth>.Update.Pull(x => x.FrameIds, id)
        );
        return Ok(new ReturnDataRecord<Frame>(frame));
    }
}
