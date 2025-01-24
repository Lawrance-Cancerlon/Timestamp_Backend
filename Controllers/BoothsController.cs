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
public class BoothsController(DatabaseService database, IAuthenticationService authentication) : ControllerBase
{
    private readonly DatabaseService _database = database;
    private readonly IAuthenticationService _authentication = authentication;

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Create([FromHeader(Name = "Authorization")] string token, CreateBoothRecord createBooth)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        Booth booth = new()
        {
            Name = createBooth.Name,
            Description = createBooth.Description,
            ClientKey = createBooth.ClientKey,
            ServerKey = createBooth.ServerKey,
            ThemeId = createBooth.ThemeId,
            FrameIds = createBooth.FrameIds,
        };
        await _database.GetCollection<Booth>("booths").InsertOneAsync(booth);
        User actor = await _database.GetCollection<User>("users").Find(x => x.IdentityId == _authentication.GetIdentity(token)).FirstOrDefaultAsync();
        await _database.GetCollection<UserLog>("userLogs").InsertOneAsync(new UserLog
        {
            Message = "Created booth " + booth.Name,
            UserId = actor.Id, 
        });
        return CreatedAtRoute(new {id = booth.Id}, new ReturnDataRecord<Booth>(booth));
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult> Get([FromQuery] string? id, [FromQuery] string? themeId, [FromQuery] string? frameId)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        var filterBuilder = Builders<Booth>.Filter;
        var filter = filterBuilder.Empty;
        if (id != null) filter &= filterBuilder.Eq(x => x.Id, id);
        if (themeId != null) filter &= filterBuilder.Eq(x => x.ThemeId, themeId);
        if (frameId != null) filter &= filterBuilder.AnyEq(x => x.FrameIds, frameId);
        return Ok(new ReturnListRecord<Booth>(await _database.GetCollection<Booth>("booths").Find(filter).ToListAsync()));
    }

    [HttpGet("init")]
    public async Task<ActionResult> Initialize([FromHeader(Name = "Token")] string id)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        Booth booth = await _database.GetCollection<Booth>("booths").Find(x => x.Id == id).FirstOrDefaultAsync();
        if (booth == null) return Unauthorized();
        Theme theme = await _database.GetCollection<Theme>("themes").Find(x => x.Id == booth.ThemeId).FirstOrDefaultAsync();
        List<Frame> frames = await _database.GetCollection<Frame>("frames").Find(x => booth.FrameIds.Contains(x.Id)).ToListAsync();
        List<Filter> filters = await _database.GetCollection<Filter>("filters").Find(_ => true).ToListAsync();
        return Ok(new ReturnInitRecord
        {
            Booth = booth,
            Theme = theme,
            Frames = frames,
            Filters = filters,
        });
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> Update([FromHeader(Name = "Authorization")] string token, string id, UpdateBoothRecord updateBooth)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        Booth booth = await _database.GetCollection<Booth>("booths").Find(x => x.Id == id).FirstOrDefaultAsync();
        if (booth == null) return NotFound("Booth not found");
        booth = updateBooth.UpdateBooth(booth);
        await _database.GetCollection<Booth>("booths").ReplaceOneAsync(x => x.Id == id, booth);
        User actor = await _database.GetCollection<User>("users").Find(x => x.IdentityId == _authentication.GetIdentity(token)).FirstOrDefaultAsync();
        await _database.GetCollection<UserLog>("userLogs").InsertOneAsync(new UserLog
        {
            Message = "Updated booth " + booth.Name,
            UserId = actor.Id, 
        });
        return Ok(new ReturnDataRecord<Booth>(booth));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete([FromHeader(Name = "Authorization")] string token, string id)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        Booth booth = await _database.GetCollection<Booth>("booths").Find(x => x.Id == id).FirstOrDefaultAsync();
        if (booth == null) return NotFound("Booth not found");
        await _database.GetCollection<Booth>("booths").DeleteOneAsync(x => x.Id == id);
        User actor = await _database.GetCollection<User>("users").Find(x => x.IdentityId == _authentication.GetIdentity(token)).FirstOrDefaultAsync();
        await _database.GetCollection<UserLog>("userLogs").InsertOneAsync(new UserLog
        {
            Message = "Deleted booth " + booth.Name,
            UserId = actor.Id, 
        });
        return Ok(new ReturnDataRecord<Booth>(booth));
    }
}
