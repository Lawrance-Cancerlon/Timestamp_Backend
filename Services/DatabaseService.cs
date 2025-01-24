using System;
using MongoDB.Driver;

namespace Timestamp_Backend.Services;

public class DatabaseService(string connectionString, string databaseName)
{
    private readonly IMongoDatabase _database = new MongoClient(connectionString).GetDatabase(databaseName);

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }
}
