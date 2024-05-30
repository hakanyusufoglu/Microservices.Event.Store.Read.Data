using MongoDB.Driver;
using Shared.Services.Abstract;

namespace Shared.Services.Concrete
{
	public class MongoDbService : IMongoDbService
	{
		public IMongoCollection<T> GetCollection<T>(string collectionName)
		{
			IMongoDatabase database = GetDatabase();
			return database.GetCollection<T>(collectionName);
		}

		public IMongoDatabase GetDatabase(string databaseName = "ProductDb", string connectionString = "mongodb://localhost:27017")
		{
			MongoClient mongoClient = new(connectionString);
			return mongoClient.GetDatabase(databaseName);
		}
	}
}
