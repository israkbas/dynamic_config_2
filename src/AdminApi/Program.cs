using ConfigReader;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

var mongoConnectionString = builder.Configuration["MongoConnectionString"] ?? "mongodb://localhost:27018";
var mongoClient = new MongoClient(mongoConnectionString);
var mongoDatabase = mongoClient.GetDatabase("DynamicConfigDb");
var collection = mongoDatabase.GetCollection<ConfigRecord>("ConfigRecords");

builder.Services.AddSingleton(collection);

var app = builder.Build();

app.MapGet("/api/configs", async (IMongoCollection<ConfigRecord> collection) =>
{
    var records = await collection.Find(FilterDefinition<ConfigRecord>.Empty).ToListAsync();
    return Results.Ok(records);
});

app.MapPost("/api/configs", async (IMongoCollection<ConfigRecord> collection, ConfigRecord record) =>
{
    await collection.InsertOneAsync(record);
    return Results.Created($"/api/configs/{record.Id}", record);
});

app.MapPut("/api/configs/{id}", async (string id, IMongoCollection<ConfigRecord> collection, ConfigRecord updated) =>
{
    updated.Id = id;
    var filter = Builders<ConfigRecord>.Filter.Eq(x => x.Id, id);
    var result = await collection.ReplaceOneAsync(filter, updated);

    return result.MatchedCount == 0 ? Results.NotFound() : Results.Ok(updated);
});

app.Run();
