using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Timestamp_Backend.Services;

public interface IAuthenticationService
{
    public Task<string?> SignUp(string email, string password);
    public Task<string?> Login(string email, string password);
    public void Delete(string identityId);
    public string GetIdentityId(string tokenId);
    public string GetIdentity(string token);
    public void ChangeEmail(string identityId, string email);
    public void ChangePassword(string identityId, string newPassword);
}

public class AuthenticationService(HttpClient httpClient) : IAuthenticationService
{
    private readonly HttpClient _httpClient = httpClient;
    public async Task<string?> SignUp(string email, string password)
    {
        UserRecord? user = null;
        try
        {
            user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);
        }
        catch (Exception) { };
        if (user != null) return null;
        var newUser = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
        {
            Email = email,
            Password = password,
        });
        return newUser.Uid;
    }

    public async Task<string?> Login(string email, string password)
    {
        var request = new
        {
            Email = email,
            Password = password,
            ReturnSecureToken = true
        };
        var response = await _httpClient.PostAsJsonAsync("", request);
        var result = await response.Content.ReadFromJsonAsync<TokenId>();
        return result?.IdToken;
    }

    public async void Delete(string identityId)
    {
        await FirebaseAuth.DefaultInstance.DeleteUserAsync(identityId);
    }

    public string GetIdentityId(string tokenId)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(tokenId) as JwtSecurityToken;
        return jsonToken!.Claims.First(claim => claim.Type == "user_id").Value;
    }

    public string GetIdentity(string token)
    {
        var tokenArr = token.Split(" ");
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(tokenArr[1]) as JwtSecurityToken;
        return jsonToken!.Claims.First(claim => claim.Type == "user_id").Value;
    }

    public async void ChangeEmail(string identityId, string email)
    {
        await FirebaseAuth.DefaultInstance.UpdateUserAsync(new UserRecordArgs
        {
            Uid = identityId,
            Email = email
        });
    }

    public async void ChangePassword(string identityId, string newPassword)
    {
        await FirebaseAuth.DefaultInstance.UpdateUserAsync(new UserRecordArgs
        {
            Uid = identityId,
            Password = newPassword,
        });
    }

    public static void ConfigureAuthentication(JwtBearerOptions options, IConfiguration config)
    {
        options.Authority = config["TokenSettings:Issuer"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = config["TokenSettings:Issuer"],
            ValidateAudience = true,
            ValidAudience = config["TokenSettings:Audience"],
            ValidateLifetime = true,
        };
        Console.WriteLine(config["TokenSettings:Issuer"]);
    }
}

public record class Credential
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;
    [JsonPropertyName("password")]
    public string Password { get; set; } = null!;
}

public record class TokenId
{
    [JsonPropertyName("idToken")]
    public string IdToken { get; set; } = null!;
}