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
public class TransactionsController(DatabaseService database, IAuthenticationService authentication, IPaymentService paymentService) : ControllerBase
{
    private readonly DatabaseService _database = database;
    private readonly IAuthenticationService _authentication = authentication;
    private readonly IPaymentService _paymentService = paymentService;

    [HttpPost]
    public async Task<ActionResult> Create([FromHeader(Name = "Token")] string id, CreateTransactionRecord createTransaction)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        Booth booth = await _database.GetCollection<Booth>("booths").Find(x => x.Id == id).FirstOrDefaultAsync();
        if (booth == null) return Unauthorized();
        Frame frame = await _database.GetCollection<Frame>("frames").Find(x => x.Id == createTransaction.FrameId).FirstOrDefaultAsync();
        Transaction transaction = new()
        {
            BoothId = id,
            Amount = frame.Price,
        };
        await _database.GetCollection<Transaction>("transactions").InsertOneAsync(transaction);
        string? token = await _paymentService.GeneratePaymentToken(transaction.Id, booth.ServerKey, frame);
        if (token == null)
        {
            await _database.GetCollection<Transaction>("transactions").DeleteOneAsync(x => x.Id == transaction.Id);
            return StatusCode(StatusCodes.Status500InternalServerError, "Failed to generate payment token.");
        }
        return CreatedAtRoute(new {id = transaction.Id}, new ReturnTokenRecord<Transaction>(transaction, token));
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult> Get([FromQuery] string? id, [FromQuery] string? paymentType, [FromQuery] string? maxTime, [FromQuery] string? minTime)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        var filterBuilder = Builders<Transaction>.Filter;
        var filter = filterBuilder.Empty;
        if (id != null) filter &= filterBuilder.Eq(x => x.Id, id);
        if (maxTime != null) filter &= filterBuilder.Lte(x => x.Timestamp, maxTime);
        if (minTime != null) filter &= filterBuilder.Gte(x => x.Timestamp, minTime);
        return Ok(new ReturnDataRecord<List<Transaction>>(await _database.GetCollection<Transaction>("transactions").Find(filter).ToListAsync()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetStatus([FromHeader(Name = "Token")] string boothId, string id)
    {
        Response.Headers.Append("Access-Control-Allow-Origin", "*");
        Booth booth = await _database.GetCollection<Booth>("booths").Find(x => x.Id == boothId).FirstOrDefaultAsync();
        if (booth == null) return Unauthorized();
        HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(booth.ServerKey + ':')));
        HttpResponseMessage response = await client.GetAsync("https://api.sandbox.midtrans.com/v2/" + id + "/status");
        var result = await response.Content.ReadFromJsonAsync<object>();
        return Ok(result);
    }
}