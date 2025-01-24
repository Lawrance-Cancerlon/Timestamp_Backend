using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Timestamp_Backend.Models;

public class Transaction
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;
    [BsonElement("BoothId")]
    public string BoothId { get; set; } = null!;
    [BsonElement("Amount")]
    public int Amount { get; set; }
    [BsonElement("PaymentToken")]
    public string? PaymentToken { get; set; }
    [BsonElement("Timestamp")]
    public string Timestamp { get; set; } = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
}
