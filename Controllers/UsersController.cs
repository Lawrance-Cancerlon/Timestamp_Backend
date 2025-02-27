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
public class UsersController(DatabaseService database, IAuthenticationService authentication) : ControllerBase
{
    private readonly DatabaseService _database = database;
    private readonly IAuthenticationService _authentication = authentication;

    [HttpPost]
    [Authorize]
    public async Task<ActionResult> Create([FromHeader(Name = "Authorization")] string token, CreateUserRecord createUser)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        var identityId = await _authentication.SignUp(createUser.Email, createUser.Password);
        if(identityId == null) return BadRequest("Email already exists");
        User user = new()
        {
            Name = createUser.Name,
            IdentityId = identityId,
        };
        await _database.GetCollection<User>("users").InsertOneAsync(user);
        User actor = await _database.GetCollection<User>("users").Find(x => x.IdentityId == _authentication.GetIdentity(token)).FirstOrDefaultAsync();
        await _database.GetCollection<UserLog>("userLogs").InsertOneAsync(new UserLog
        {
            Message = "Created admin " + user.Name,
            UserId = actor.Id, 
        });
        return CreatedAtRoute(new {id = user.Id}, new ReturnDataRecord<User>(user));
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(Credential credential)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        var token = await _authentication.Login(credential.Email, credential.Password);
        if (token == null) return BadRequest("Invalid email or password");
        User actor = await _database.GetCollection<User>("users").Find(x => x.IdentityId == _authentication.GetIdentityId(token)).FirstOrDefaultAsync();
        await _database.GetCollection<UserLog>("userLogs").InsertOneAsync(new UserLog
        {
            Message = "Logged in " + actor.Name,
            UserId = actor.Id, 
        });
        return Ok(new ReturnTokenRecord<User>(actor, token));
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult> Get([FromQuery] string? id)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        var filterBuilder = Builders<User>.Filter;
        var filter = filterBuilder.Empty;
        if(id != null) filter &= filterBuilder.Eq(x => x.Id, id);
        return Ok(new ReturnDataRecord<List<User>>(await _database.GetCollection<User>("users").Find(filter).ToListAsync()));
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult> Update([FromHeader(Name = "Authorization")] string token, UpdateUserRecord updateUser)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        User actor = await _database.GetCollection<User>("users").Find(x => x.IdentityId == _authentication.GetIdentity(token)).FirstOrDefaultAsync();
        actor = updateUser.UpdateUser(actor);
        await _database.GetCollection<User>("users").ReplaceOneAsync(x => x.Id == actor.Id, actor);
        if(updateUser.Email != null) _authentication.ChangeEmail(_authentication.GetIdentity(token), updateUser.Email);
        if(updateUser.Password != null) _authentication.ChangePassword(_authentication.GetIdentity(token), updateUser.Password);
        await _database.GetCollection<UserLog>("userLogs").InsertOneAsync(new UserLog
        {
            Message = "Updated admin " + actor.Name,
            UserId = actor.Id, 
        });
        return Ok(new ReturnDataRecord<User>(actor));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> Delete([FromHeader(Name = "Authorization")] string token, string id)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        User user = await _database.GetCollection<User>("users").Find(x => x.Id == id).FirstOrDefaultAsync();
        if(user == null) return NotFound("User not found");
        _authentication.Delete(user.IdentityId);
        await _database.GetCollection<User>("users").DeleteOneAsync(x => x.Id == user.Id);
        User actor = await _database.GetCollection<User>("users").Find(x => x.IdentityId == _authentication.GetIdentity(token)).FirstOrDefaultAsync();
        await _database.GetCollection<UserLog>("userLogs").InsertOneAsync(new UserLog
        {
            Message = "Deleted admin " + user.Name,
            UserId = actor.Id, 
        });
        return Ok(new ReturnDataRecord<User>(user));
    }
}
