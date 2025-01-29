using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Timestamp_Backend.Configurations;
using Timestamp_Backend.Services;

var builder = WebApplication.CreateBuilder(args);

//Add Cors Policy
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder => 
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

//Add Database Connection
builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection(nameof(DatabaseSettings)));
builder.Services.AddSingleton<DatabaseService>(serviceProvider =>
{
    var settings = serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    return new DatabaseService(settings.ConnectionString, settings.DatabaseName);
});

//Add Firebase Authentication
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile("firebase.json")
});
builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
builder.Services.AddHttpClient<IAuthenticationService, AuthenticationService>((serviceProvider, httpClient) =>
{
    var tokenUrl = builder.Configuration["TokenSettings:TokenUrl"];
    if (string.IsNullOrEmpty(tokenUrl))
    {
        throw new InvalidOperationException("Token URL is not configured.");
    }
    httpClient.BaseAddress = new Uri(tokenUrl);
});

//Add Jwt Validator
builder.Services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => AuthenticationService.ConfigureAuthentication(options, builder.Configuration));

builder.Services.AddControllers();
builder.Services.AddControllersWithViews();

//Add MidTrans Payment
builder.Services.AddSingleton<IPaymentService, PaymentService>();
builder.Services.AddHttpClient<IPaymentService, PaymentService>((serviceProvider, httpClient) =>
{
    httpClient.BaseAddress = new Uri("https://app.sandbox.midtrans.com/snap/v1/transactions");
});

//Add Google Cloud Storage Service
builder.Services.AddSingleton<StorageService>(serviceProvider =>
{
    return new StorageService(StorageClient.Create(GoogleCredential.FromFile("storage.json")), UrlSigner.FromCredential(GoogleCredential.FromFile("storage.json")));
});

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCors();
// app.UseHttpsRedirection();

app.Run();