using AkakceProject.Core.Dto;
using AkakceProject.Core.Entities;
using AkakceProject.Core.Responses;
using AkakceProject.Infrastructure.Contracts;
using Dapper;

namespace AkakceProject.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDapperContext _context;

        public ProductRepository(IDapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            var query = "SELECT * FROM AkakceProduct WITH (NOLOCK)";
            return await _context.QueryAsync<Product>(query);
        }

        public async Task<Product> GetProduct(int id)
        {
            var query = "SELECT * FROM AkakceProduct WITH (NOLOCK) WHERE ProductId = @ProductId";
            return await _context.QuerySingleOrDefaultAsync<Product>(query, new { ProductId = id });
        }

        public async Task<IEnumerable<Product>> GetUserProducts(int id)
        {
            var query = "SELECT * FROM AkakceProduct WITH (NOLOCK) WHERE UserId = @UserId";
            return await _context.QueryAsync<Product>(query, new { UserId = id });
        }

        public async Task<IEnumerable<Product>> GetCampaignProducts(int id)
        {
            var query = "SELECT * FROM AkakceProduct WITH (NOLOCK) WHERE CampaignId = @CampaignId";
            return await _context.QueryAsync<Product>(query, new { CampaignId = id });
        }

        public async Task<ProductResponse> GetProductInfo(int userId, int productId)
        {
            var userRepo = new UserRepository(_context);

            var isAdmin = await userRepo.IsUserAdmin(userId);
            if (!isAdmin.HasValue) return null;

            var query = "SELECT U.UserName AS BrandName, C.CampaignTitle AS CampaignName, " +
                        "P.ProductName, P.ProductDescription, P.ProductPrice " +
                        "FROM AkakceUser U WITH (NOLOCK) " +
                        "LEFT JOIN AkakceCampaign C ON U.UserId = C.UserId " +
                        "LEFT JOIN AkakceProduct P ON U.UserId = P.UserId AND C.CampaignId = P.CampaignId " +
                        "WHERE P.ProductId = " + productId;

            query += isAdmin.Value ? " AND U.UserId = " + userId : "";

            return await _context.QuerySingleOrDefaultAsync<ProductResponse>(query);
        }

        public async Task<Product> CreateProduct(ProductForCreationDto product)
        {
            string query = "INSERT INTO AkakceProduct (ProductName, ProductDescription, ProductPrice, UserId, CampaignId) " +
                           "VALUES(@ProductName, @ProductDescription, @ProductPrice, @UserId, @CampaignId);" +
                           "SELECT @@IDENTITY";

            var id = await _context.QuerySingleOrDefaultAsync<int>(query, product);

            var createdProduct = new Product
            {
                ProductId = id,
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                ProductPrice = product.ProductPrice,
                UserId = product.UserId,
                CampaignId = product.CampaignId,
            };

            return createdProduct;
        }

        public async Task UpdateProduct(int id, ProductForUpdateDto product)
        {
            string query = "UPDATE AkakceProduct WITH (ROWLOCK) SET ProductDescription = @ProductDescription, ProductPrice = @ProductPrice, CampaignId = @CampaignId " +
                           "WHERE ProductId = @ProductId";

            await _context.ExecuteAsync(query, new { product.ProductDescription, product.ProductPrice, product.CampaignId, ProductId = id });
        }

        public async Task DeleteProduct(int id)
        {
            string query = "DELETE FROM AkakceProduct WHERE ProductId = @ProductId";

            await _context.ExecuteAsync(query, new { ProductId = id });
        }

        public async Task DeleteUserProducts(int id)
        {
            string query = "DELETE FROM AkakceProduct WHERE UserId = @UserId";

            await _context.ExecuteAsync(query, new { UserId = id });
        }

        public async Task DeleteCampaignProducts(int id)
        {
            string query = "DELETE FROM AkakceProduct WHERE CampaignId = @CampaignId";

            await _context.ExecuteAsync(query, new { CampaignId = id });
        }
    }
}
