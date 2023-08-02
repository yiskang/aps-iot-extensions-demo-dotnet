using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Newtonsoft.Json.Serialization;
using dotenv.net;
using Autodesk.Das.Models;
using JsonFlatFileDataStore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
});

string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "db.json");
var jsonDataStore = new DataStore(jsonFilePath, keyProperty: "id", reloadBeforeGetCollection: true);

builder.Services.AddSingleton<IDataStore>(jsonDataStore);

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    //options.SerializerSettings.Converters.Add(new EmptyStringToNullJsonConverter());
    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
    // options.SerializerSettings.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ssZ";
});

// READ APS credentials from .dev or app settings
DotEnv.Load();

var configuration = builder.Configuration;
var envConfiguration = DotEnv.Read();
var clientID = configuration["APS_CLIENT_ID"] ?? envConfiguration["APS_CLIENT_ID"];
var clientSecret = configuration["APS_CLIENT_SECRET"] ?? envConfiguration["APS_CLIENT_SECRET"];
if (string.IsNullOrEmpty(clientID) || string.IsNullOrEmpty(clientSecret))
{
    throw new ApplicationException("Missing required environment variables APS_CLIENT_ID or APS_CLIENT_SECRET.");
}
builder.Services.AddSingleton<APS>(new APS(clientID, clientSecret));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();