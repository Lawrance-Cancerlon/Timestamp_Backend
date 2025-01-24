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
public class UserLogsController(DatabaseService database, IAuthenticationService authentication) : ControllerBase
{
    private readonly DatabaseService _database = database;
    private readonly IAuthenticationService _authentication = authentication;

    [HttpGet]
    [Authorize]
    public async Task<ActionResult> Get([FromQuery] string? id, [FromQuery] string? userId)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        var filterBuilder = Builders<UserLog>.Filter;
        var filter = filterBuilder.Empty;
        if (id != null) filter &= filterBuilder.Eq(x => x.Id, id);
        if (userId != null) filter &= filterBuilder.Eq(x => x.UserId, userId);
        return Ok(new ReturnListRecord<UserLog>(await _database.GetCollection<UserLog>("userLogs").Find(filter).ToListAsync()));
    }
}
