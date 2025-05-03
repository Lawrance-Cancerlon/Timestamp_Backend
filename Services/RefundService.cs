using System;

namespace Timestamp_Backend.Services;

public interface IRefundService
{
    public Task<HttpResponseMessage> Refund(string id, string paymentKey);
}

public class RefundService(HttpClient httpClient) : IRefundService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<HttpResponseMessage> Refund(string id, string paymentKey)
    {
        var request = new
        {
            refund_key = "refund_"+id,
        };
        var paymentKeyBytes = System.Text.Encoding.UTF8.GetBytes(paymentKey + ':');
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + Convert.ToBase64String(paymentKeyBytes));
        var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(id+"/refund", content);
        return response;
    }
}
