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
public class FramesController(DatabaseService database, IAuthenticationService authentication) : ControllerBase
{
    private readonly DatabaseService _database = database;
    private readonly IAuthenticationService _authentication = authentication;

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
            Url = createFrame.Url,
        };
        await _database.GetCollection<Frame>("frames").InsertOneAsync(frame);
        User actor = await _database.GetCollection<User>("users").Find(x => x.IdentityId == _authentication.GetIdentity(token)).FirstOrDefaultAsync();
        await _database.GetCollection<UserLog>("userLogs").InsertOneAsync(new UserLog
        {
            Message = "Created frame " + frame.Name,
            UserId = actor.Id, 
        });
        return CreatedAtRoute(new {id = frame.Id}, new ReturnDataRecord<Frame>(frame));
    }

    [HttpGet]
    public async Task<ActionResult> Get([FromQuery] string? id, [FromQuery] string? themeId, [FromQuery] int? count)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        var filterBuilder = Builders<Frame>.Filter;
        var filter = filterBuilder.Empty;
        if (id!= null) filter &= filterBuilder.Eq(x => x.Id, id);
        if (themeId!= null) filter &= filterBuilder.Eq(x => x.ThemeId, themeId);
        if (count!= null) filter &= filterBuilder.Eq(x => x.Count, count);
        return Ok(new ReturnListRecord<Frame>(await _database.GetCollection<Frame>("frames").Find(filter).ToListAsync()));
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
        return Ok(new ReturnDataRecord<Frame>(frame));
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
        return Ok(new ReturnDataRecord<Frame>(frame));
    }
}
