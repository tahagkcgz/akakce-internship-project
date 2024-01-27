using AkakceProject.Core.Dto;
using AkakceProject.Core.Entities;
using AkakceProject.Core.Responses;
using AkakceProject.Infrastructure.Contracts;
using AkakceProject.Infrastructure.Repositories;
using Moq;
using NUnit.Framework;
using System.Data;

namespace AkakceProject.Tests.RepositoryTests
{
    [TestFixture]
    public class ProductRepositoryTests
    {
        private Mock<IDapperContext> _contextMock;
        private ProductRepository _productRepository;

        public List<User> dbUsers;
        public List<Campaign> dbCampaigns;
        public List<Product> dbProducts;

        [SetUp]
        public void Setup()
        {
            _contextMock = new Mock<IDapperContext>();
            _productRepository = new ProductRepository(_contextMock.Object);

            dbUsers = new List<User>
            {
                new User { UserId = 1, UserName = "asli", UserPassword = "userasli", EmailAddress = "asli@gmail.com", PhoneNumber = "", IsAdmin = false, IsUserActive = true },
                new User { UserId = 2, UserName = "Samsung", UserPassword = "samsung.admin", EmailAddress = "info@samsung.com", PhoneNumber = "08502256060", IsAdmin = true, IsUserActive = true },
                new User { UserId = 3, UserName = "Apple", UserPassword = "apple.admin", EmailAddress = "info@apple.com", PhoneNumber = "08502257979", IsAdmin = true, IsUserActive = true }
            };

            dbCampaigns = new List<Campaign>
            {
                new Campaign { CampaignId = 1, CampaignTitle = "%20 İndirim", IsActive = true, StartsAt = "2023-09-06", EndsAt = "2023-09-15", UserId = 2},
                new Campaign { CampaignId = 2, CampaignTitle = "Sigorta Hediye", IsActive = true, StartsAt = "2023-09-06", EndsAt = "2023-09-15", UserId = 2},
                new Campaign { CampaignId = 3, CampaignTitle = "%10 İndirim", IsActive = false, StartsAt = "2023-09-08", EndsAt = "2023-09-20", UserId = 3}
            };

            dbProducts = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Galaxy Z Flip 4 256 GB", ProductDescription = "Güzel telefon", ProductPrice = 40000, UserId = 2, CampaignId = 1 },
                new Product { ProductId = 2, ProductName = "Galaxy Z Flip 3 128 GB", ProductDescription = "Sağlam telefon", ProductPrice = 30000, UserId = 2, CampaignId = 2 },
                new Product { ProductId = 3, ProductName = "Iphone 12 128 GB", ProductDescription = "Çok iyi telefon", ProductPrice = 42000, UserId = 3, CampaignId = 3 },
                new Product { ProductId = 4, ProductName = "Galaxy A3 64 GB", ProductDescription = "İdarelik telefon", ProductPrice = 6000, UserId = 2, CampaignId = null },
            };
        }

        [Test]
        public async Task GetProducts_ReturnsListOfProducts()
        {
            // Arrange
            _contextMock.Setup(c => c.QueryAsync<Product>(It.IsAny<string>(), null))
                                     .ReturnsAsync(dbProducts);

            // Act
            var result = await _productRepository.GetProducts();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<IEnumerable<Product>>());
            Assert.That(dbProducts, Has.Count.EqualTo(result.Count()));
        }

        [Test]
        public async Task GetProduct_ReturnsProduct()
        {
            // Arrange
            var expectedProduct = dbProducts[0];

            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<Product>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync(expectedProduct);

            // Act
            var result = await _productRepository.GetProduct(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<Product>());
            Assert.That(result, Is.SameAs(expectedProduct));
        }

        [Test]
        public async Task GetProduct_ReturnsNullWithNonExistingProductId()
        {
            // Arrange
            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<Product>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync((Product)null);

            // Act
            var result = await _productRepository.GetProduct(5);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetUserProducts_ReturnsUserProducts()
        {
            // Arrange
            var expectedProducts = new List<Product> { dbProducts[0], dbProducts[1], dbProducts[3] };

            _contextMock.Setup(c => c.QueryAsync<Product>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync(expectedProducts);

            // Act
            var result = await _productRepository.GetUserProducts(2);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<IEnumerable<Product>>());
            Assert.That(expectedProducts, Has.Count.EqualTo(result.Count()));
        }

        [Test]
        public async Task GetCampaignProducts_ReturnsCampaignProducts()
        {
            // Arrange
            var expectedProducts = new List<Product> { dbProducts[0] };

            _contextMock.Setup(c => c.QueryAsync<Product>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync(expectedProducts);

            // Act
            var result = await _productRepository.GetCampaignProducts(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<IEnumerable<Product>>());
            Assert.That(expectedProducts, Has.Count.EqualTo(result.Count()));
        }

        [Test]
        public async Task GetProductInfo_ReturnsProductInfo()
        {
            // Arrange
            var expectedProductInfo = new ProductResponse
            {
                BrandName = "Samsung",
                CampaignName = "%20 İndirim",
                ProductName = "Galaxy Z Flip 4 256 GB",
                ProductDescription = "Güzel telefon",
                ProductPrice = 40000
            };

            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<bool?>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync(false);

            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<ProductResponse>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync(expectedProductInfo);

            // Act
            var result = await _productRepository.GetProductInfo(1, 1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.SameAs(expectedProductInfo));
        }

        [Test]
        public async Task GetProductInfo_ReturnsNullOnAnotherAdminProductInfo()
        {
            // Arrange
            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<bool?>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync(true);

            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<ProductResponse>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync((ProductResponse)null);

            // Act
            var result = await _productRepository.GetProductInfo(2, 3);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetProductInfo_ReturnsNullWithNonExistingUserId()
        {
            // Arrange
            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<bool?>(It.IsAny<string>(), It.IsAny<object>()))
                                     .Returns(Task.FromResult<bool?>(null));

            // Act
            var result = await _productRepository.GetProductInfo(5, 1);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetProductInfo_ReturnsNullWithNonExistingProductId()
        {
            // Arrange
            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<bool?>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync(true);

            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<ProductResponse>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync((ProductResponse)null);

            // Act
            var result = await _productRepository.GetProductInfo(2, 300);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateProduct_ReturnsCreatedProduct()
        {
            // Arrange
            var product = new ProductForCreationDto { ProductName = "Iphone 13 128 GB", ProductDescription = "Baya iyi telefon", ProductPrice = 45000, UserId = 3, CampaignId = null };
            var expectedCreatedProduct = new Product
            {
                ProductId = 5,
                ProductName = "Iphone 13 128 GB",
                ProductDescription = "Baya iyi telefon",
                ProductPrice = 45000,
                UserId = 3,
                CampaignId = null
            };

            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<int>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync(5);

            // Act
            var result = await _productRepository.CreateProduct(product);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.ProductId, Is.EqualTo(expectedCreatedProduct.ProductId));
                Assert.That(result.ProductName, Is.EqualTo(expectedCreatedProduct.ProductName));
                Assert.That(result.ProductDescription, Is.EqualTo(expectedCreatedProduct.ProductDescription));
                Assert.That(result.ProductPrice, Is.EqualTo(expectedCreatedProduct.ProductPrice));
                Assert.That(result.CampaignId, Is.EqualTo(expectedCreatedProduct.CampaignId));
                Assert.That(result.UserId, Is.EqualTo(expectedCreatedProduct.UserId));
            });
        }

        [Test]
        public async Task UpdateProduct_UpdatesProductSuccessfully()
        {
            // Arrange
            var product = new ProductForUpdateDto { ProductDescription = "Çok sağlam telefon", ProductPrice = 42500, CampaignId = 1 };
            var expectedUpdatedProduct = new Product
            {
                ProductId = 1,
                ProductName = "Galaxy Z Flip 4 256 GB",
                ProductDescription = "Güzel telefon",
                ProductPrice = 40000,
                UserId = 2,
                CampaignId = 1
            };

            _contextMock.Setup(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()))
                                     .Returns(Task.CompletedTask);

            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<Product>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(expectedUpdatedProduct);

            // Act
            await _productRepository.UpdateProduct(1, product);

            // Assert
            _contextMock.Verify(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
            var updatedProduct = await _productRepository.GetProduct(1);

            Assert.Multiple(() =>
            {
                Assert.That(updatedProduct.ProductDescription, Is.EqualTo(expectedUpdatedProduct.ProductDescription));
                Assert.That(updatedProduct.ProductPrice, Is.EqualTo(expectedUpdatedProduct.ProductPrice));
                Assert.That(updatedProduct.CampaignId, Is.EqualTo(expectedUpdatedProduct.CampaignId));
            });
        }

        [Test]
        public async Task DeleteProduct_DeletesProductSuccessfully()
        {
            // Arrange
            _contextMock.Setup(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()))
                                     .Returns(Task.CompletedTask);

            // Act
            await _productRepository.DeleteProduct(1);

            // Assert
            _contextMock.Verify(c => c.ExecuteAsync("DELETE FROM AkakceProduct WHERE ProductId = @ProductId", It.IsAny<object>()), Times.Once);
        }

        [Test]
        public async Task DeleteUserProducts_DeletesUserProductsSuccessfully()
        {
            // Arrange
            _contextMock.Setup(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()))
                                     .Returns(Task.CompletedTask);

            // Act
            await _productRepository.DeleteUserProducts(2);

            // Assert
            _contextMock.Verify(c => c.ExecuteAsync("DELETE FROM AkakceProduct WHERE UserId = @UserId", It.IsAny<object>()), Times.Once);
        }

        [Test]
        public async Task DeleteCampaignProducts_DeletesCampaignProductsSuccessfully()
        {
            // Arrange
            _contextMock.Setup(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()))
                                     .Returns(Task.CompletedTask);

            // Act
            await _productRepository.DeleteCampaignProducts(1);

            // Assert
            _contextMock.Verify(c => c.ExecuteAsync("DELETE FROM AkakceProduct WHERE CampaignId = @CampaignId", It.IsAny<object>()), Times.Once);
        }
    }
}