using AkakceProject.Core.Dto;
using AkakceProject.Core.Entities;
using AkakceProject.Core.Responses;

namespace AkakceProject.Infrastructure.Contracts
{
    public interface IProductRepository
    {
        public Task<IEnumerable<Product>> GetProducts();
        public Task<Product> GetProduct(int id);
        public Task<IEnumerable<Product>> GetUserProducts(int id);
        public Task<IEnumerable<Product>> GetCampaignProducts(int id);
        public Task<ProductResponse> GetProductInfo(int userId, int productId);
        public Task<Product> CreateProduct(ProductForCreationDto product);
        public Task UpdateProduct(int id, ProductForUpdateDto product);
        public Task DeleteProduct(int id);
        public Task DeleteUserProducts(int id);
        public Task DeleteCampaignProducts(int id);
    }
}
