using System;
using System.Text.Json.Serialization;
using Timestamp_Backend.Models;

namespace Timestamp_Backend.Services;

public interface IPaymentService
{
    public Task<string?> GeneratePaymentToken(string id, string paymentKey, Frame frame, int quantity);
}

public class PaymentService(HttpClient httpClient) : IPaymentService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<string?> GeneratePaymentToken(string id, string paymentKey, Frame frame, int quantity)
    {
        var request = new
        {
            transaction_details = new
            {
                order_id = id,
                gross_amount = frame.Price*quantity,
            },
            enabled_payments = new string[] { "other_qris" },
        };
        var paymentKeyBytes = System.Text.Encoding.UTF8.GetBytes(paymentKey + ':');
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(paymentKeyBytes));
        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("", content);
        var result = await response.Content.ReadFromJsonAsync<PaymentToken>();
        return result?.Token;
    }
}

public record class PaymentToken
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = null!;
    [JsonPropertyName("redirect_url")]
    public string RedirectUrl { get; set; } = null!;
}