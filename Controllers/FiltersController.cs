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
public class FiltersController(DatabaseService database, IAuthenticationService authentication) : ControllerBase
{
    private readonly DatabaseService _database = database;
    private readonly IAuthenticationService _authentication = authentication;

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Create([FromHeader(Name = "Authorization")] string token, CreateFilterRecord createFilter)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        Filter filter = new()
        {
            Name = createFilter.Name,
            Preset = createFilter.Preset,
        };
        await _database.GetCollection<Filter>("filters").InsertOneAsync(filter);
        User actor = await _database.GetCollection<User>("users").Find(x => x.IdentityId == _authentication.GetIdentity(token)).FirstOrDefaultAsync();
        await _database.GetCollection<UserLog>("userLogs").InsertOneAsync(new UserLog
        {
            Message = "Created filter " + filter.Name,
            UserId = actor.Id, 
        });
        return CreatedAtRoute(new {id = filter.Id}, new ReturnDataRecord<Filter>(filter));
    }

    [HttpGet]
    public async Task<ActionResult> Get([FromQuery] string? id)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        var filterBuilder = Builders<Filter>.Filter;
        var filter = filterBuilder.Empty;
        if (id != null) filter &= filterBuilder.Eq(x => x.Id, id);
        return Ok(new ReturnListRecord<Filter>(await _database.GetCollection<Filter>("filters").Find(filter).ToListAsync()));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult> Update([FromHeader(Name = "Authorization")] string token, string id, UpdateFilterRecord updateFilter)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        Filter filter = await _database.GetCollection<Filter>("filters").Find(x => x.Id == id).FirstOrDefaultAsync();
        if (filter == null) return NotFound("Filter not found");
        filter = updateFilter.UpdateFilter(filter);
        await _database.GetCollection<Filter>("filters").ReplaceOneAsync(x => x.Id == id, filter);
        User actor = await _database.GetCollection<User>("users").Find(x => x.IdentityId == _authentication.GetIdentity(token)).FirstOrDefaultAsync();
        await _database.GetCollection<UserLog>("userLogs").InsertOneAsync(new UserLog
        {
            Message = "Updated filter " + filter.Name,
            UserId = actor.Id, 
        });
        return Ok(new ReturnDataRecord<Filter>(filter));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete([FromHeader(Name = "Authorization")] string token, string id)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        Filter filter = await _database.GetCollection<Filter>("filters").Find(x => x.Id == id).FirstOrDefaultAsync();
        if (filter == null) return NotFound("Filter not found");
        await _database.GetCollection<Filter>("filters").DeleteOneAsync(x => x.Id == id);
        User actor = await _database.GetCollection<User>("users").Find(x => x.IdentityId == _authentication.GetIdentity(token)).FirstOrDefaultAsync();
        await _database.GetCollection<UserLog>("userLogs").InsertOneAsync(new UserLog
        {
            Message = "Deleted filter " + filter.Name,
            UserId = actor.Id, 
        });
        return Ok(new ReturnDataRecord<Filter>(filter));
    }
}
