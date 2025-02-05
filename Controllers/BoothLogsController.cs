using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Timestamp_Backend.Contracts;
using Timestamp_Backend.Models;
using Timestamp_Backend.Services;

namespace Timestamp_Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BoothLogsController(DatabaseService database, IAuthenticationService authentication) : ControllerBase
{
    private readonly DatabaseService _database = database;
    private readonly IAuthenticationService _authentication = authentication;

    [HttpPost]
    public async Task<ActionResult> Create([FromHeader(Name = "Token")] string id, CreateBoothLogRecord createBoothLog)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        Booth booth = await _database.GetCollection<Booth>("booths").Find(x => x.Id == id).FirstOrDefaultAsync();
        if (booth == null) return Unauthorized();
        BoothLog boothLog = new()
        {
            Message = createBoothLog.Message,
            Level = createBoothLog.Level,
            BoothId = id,
            Timestamp = createBoothLog.Timestamp,
        };
        booth.Status = boothLog.Level;
        await _database.GetCollection<Booth>("booths").ReplaceOneAsync(x => x.Id == id, booth);
        await _database.GetCollection<BoothLog>("boothLogs").InsertOneAsync(boothLog);
        return CreatedAtRoute(new {id = boothLog.Id}, new ReturnDataRecord<BoothLog>(boothLog));
    }

    [HttpGet]
    public async Task<ActionResult> Get([FromQuery] string? id, [FromQuery] string? boothId, [FromQuery] int? level)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        var filterBuilder = Builders<BoothLog>.Filter;
        var filter = filterBuilder.Empty;
        if (id != null) filter &= filterBuilder.Eq(x => x.Id, id);
        if (boothId != null) filter &= filterBuilder.Eq(x => x.BoothId, boothId);
        if (level != null) filter &= filterBuilder.Eq(x => x.Level, level);
        return Ok(new ReturnDataRecord<List<BoothLog>>(await _database.GetCollection<BoothLog>("boothLogs").Find(filter).ToListAsync()));
    }
}
