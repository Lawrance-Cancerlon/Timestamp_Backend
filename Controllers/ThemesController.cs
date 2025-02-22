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
public class ThemesController(DatabaseService database, IAuthenticationService authentication, StorageService storage) : ControllerBase
{
    private readonly DatabaseService _database = database;
    private readonly IAuthenticationService _authentication = authentication;
    private readonly StorageService _storage = storage;

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Create([FromHeader(Name = "Authorization")] string token, CreateThemeRecord createTheme)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        Theme theme = new()
        {
            Name = createTheme.Name,
        };
        await _database.GetCollection<Theme>("themes").InsertOneAsync(theme);
        User actor = await _database.GetCollection<User>("users").Find(x => x.IdentityId == _authentication.GetIdentity(token)).FirstOrDefaultAsync();
        await _database.GetCollection<UserLog>("userLogs").InsertOneAsync(new UserLog
        {
            Message = "Created theme " + theme.Name,
            UserId = actor.Id, 
        });
        return CreatedAtRoute(new {id = theme.Id}, new ReturnDataRecord<ReturnThemeRecord>(new ReturnThemeRecord(theme, await _storage.UploadThemeUrl(theme.Id))));
    }

    [HttpGet]
    public async Task<ActionResult> Get([FromQuery] string? id)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        var filterBuilder = Builders<Theme>.Filter;
        var filter = filterBuilder.Empty;
        if (id != null) filter &= filterBuilder.Eq(x => x.Id, id);
        return Ok(new ReturnDataRecord<List<ReturnThemeRecord>>([.. await Task.WhenAll((await _database.GetCollection<Theme>("themes").Find(filter).ToListAsync()).Select(async x => new ReturnThemeRecord(x, await _storage.GetThemeUrl(x.Id))))]));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> Update([FromHeader(Name = "Authorization")] string token, string id, UpdateThemeRecord updateTheme)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        Theme theme = await _database.GetCollection<Theme>("themes").Find(x => x.Id == id).FirstOrDefaultAsync();
        if (theme == null) return NotFound("Theme not found");
        theme = updateTheme.UpdateTheme(theme);
        await _database.GetCollection<Theme>("themes").ReplaceOneAsync(x => x.Id == id, theme);
        User actor = await _database.GetCollection<User>("users").Find(x => x.IdentityId == _authentication.GetIdentity(token)).FirstOrDefaultAsync();
        await _database.GetCollection<UserLog>("userLogs").InsertOneAsync(new UserLog
        {
            Message = "Updated theme " + theme.Name,
            UserId = actor.Id, 
        });
        return Ok(new ReturnDataRecord<ReturnThemeRecord>(new ReturnThemeRecord(theme, await _storage.UploadThemeUrl(theme.Id))));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete([FromHeader(Name = "Authorization")] string token, string id)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        Theme theme = await _database.GetCollection<Theme>("themes").Find(x => x.Id == id).FirstOrDefaultAsync();
        if (theme == null) return NotFound("Theme not found");
        await _database.GetCollection<Theme>("themes").DeleteOneAsync(x => x.Id == id);
        User actor = await _database.GetCollection<User>("users").Find(x => x.IdentityId == _authentication.GetIdentity(token)).FirstOrDefaultAsync();
        await _database.GetCollection<UserLog>("userLogs").InsertOneAsync(new UserLog
        {
            Message = "Deleted theme " + theme.Name,
            UserId = actor.Id, 
        });
        _storage.DeleteTheme(id);
        return Ok(new ReturnDataRecord<Theme>(theme));
    }
}
