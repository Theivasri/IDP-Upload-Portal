using MongoDB.Driver;
using IDPUpload_Portal.Models;

namespace IDPUpload_Portal.Services
{
    public class CosmosMongoService
    {
        private readonly IMongoCollection<Category> _categories;
        private readonly IMongoCollection<CategoryMetadata> _categoryMetadata;

        public CosmosMongoService(IConfiguration config)
        {
            var client = new MongoClient(config["CosmosDb:ConnectionString"]);
            var database = client.GetDatabase(config["CosmosDb:DatabaseName"]);
            _categories = database.GetCollection<Category>("Category");
            _categoryMetadata = database.GetCollection<CategoryMetadata>("CategoryMetadata");
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _categories.Find(_ => true).ToListAsync();
        }

        public async Task<Category> GetCategoryByIdAsync(string categoryId)
        {
            return await _categories.Find(c => c.Category_ID == categoryId).FirstOrDefaultAsync();
        }

        public async Task<List<CategoryMetadata>> GetMetadataByCategoryIdAsync(string categoryId)
        {
            var metadatalist = await _categoryMetadata.Find(x => x.CategoryId == categoryId).ToListAsync();
            System.Console.WriteLine($"Metadata count : { metadatalist.Count}");
            return metadatalist;
        }
    }
}