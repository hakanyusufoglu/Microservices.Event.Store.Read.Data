using MongoDB.Driver;
namespace Shared.Services.Abstract
{
	public interface IMongoDbService
	{
		//ilgili collection'ı döndüren metot
		IMongoCollection<T> GetCollection<T>(string collectionName);
		IMongoDatabase GetDatabase(string databaseName, string connectionString);
	}
}
