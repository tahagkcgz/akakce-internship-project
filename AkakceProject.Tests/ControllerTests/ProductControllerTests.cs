using AkakceProject.API.Controllers;
using AkakceProject.Application.Mediators;
using AkakceProject.Core.Dto;
using AkakceProject.Core.Entities;
using AkakceProject.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace AkakceProject.Tests.ControllerTests
{
    [TestFixture]
    public class ProductControllerTests
    {
        private ProductController _controller;
        private Mock<IMediator> _mediatorMock;

        public List<User> dbUsers;
        public List<Campaign> dbCampaigns;
        public List<Product> dbProducts;

        [SetUp]
        public void Setup()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new ProductController(_mediatorMock.Object);

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
        public async Task GetProducts_ReturnsOkWithProducts()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.GetAllProductsQuery>(), default))
                         .ReturnsAsync(dbProducts);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var products = (result.Result as OkObjectResult)!.Value as IEnumerable<Product>;
            Assert.That(products, Is.Not.Null);
            Assert.That(dbProducts, Has.Count.EqualTo(products.Count()));
        }

        [Test]
        public async Task GetProducts_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.GetAllProductsQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong!"));

            // Act
            var result = await _controller.GetProducts();

            // Assert
            Assert.That(result, Is.Not.Null);

            var statusCodeResult = (result.Result as ObjectResult)!.StatusCode;
            Assert.That(statusCodeResult, Is.EqualTo(500));
        }

        [Test]
        public async Task GetProduct_ReturnsOkWithProduct()
        {
            // Arrange
            var expectedProduct = dbProducts[0];

            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.GetProductQuery>(), default))
                         .ReturnsAsync(expectedProduct);

            // Act
            var result = await _controller.GetProduct(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var product = (result.Result as OkObjectResult)!.Value as Product;
            Assert.That(product, Is.Not.Null);
            Assert.That(product, Is.SameAs(expectedProduct));
        }

        [Test]
        public async Task GetProduct_ReturnsNotFoundWithNonExistingProductId()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.GetProductQuery>(), default))
                         .ReturnsAsync((Product)null);

            // Act
            var result = await _controller.GetProduct(5);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<NotFoundResult>(result.Result);

            var statusCodeResult = (result.Result as NotFoundResult)!.StatusCode;
            Assert.That(statusCodeResult, Is.EqualTo(404));
        }

        [Test]
        public async Task GetProduct_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.GetProductQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong!"));

            // Act
            var result = await _controller.GetProduct(500);

            // Assert
            Assert.That(result, Is.Not.Null);

            var statusCodeResult = (result.Result as ObjectResult)!.StatusCode;
            Assert.That(statusCodeResult, Is.EqualTo(500));
        }

        [Test]
        public async Task GetUserProducts_ReturnsOkWithUserProducts()
        {
            // Arrange
            var expectedProducts = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Galaxy Z Flip 4 256 GB", ProductDescription = "Güzel telefon", ProductPrice = 40000, UserId = 2, CampaignId = 1 },
                new Product { ProductId = 2, ProductName = "Galaxy Z Flip 3 128 GB", ProductDescription = "Sağlam telefon", ProductPrice = 30000, UserId = 2, CampaignId = 2 },
                new Product { ProductId = 4, ProductName = "Galaxy A3 64 GB", ProductDescription = "İdarelik telefon", ProductPrice = 6000, UserId = 2, CampaignId = null }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.GetUserProductsQuery>(), default))
                         .ReturnsAsync(expectedProducts);

            // Act
            var result = await _controller.GetUserProducts(2);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var products = (result.Result as OkObjectResult)!.Value as IEnumerable<Product>;
            Assert.That(products, Is.Not.Null);
            Assert.That(expectedProducts, Has.Count.EqualTo(products.Count()));
        }

        [Test]
        public async Task GetUserProducts_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.GetUserProductsQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong!"));

            // Act
            var result = await _controller.GetUserProducts(500);

            // Assert
            Assert.That(result, Is.Not.Null);

            var statusCodeResult = (result.Result as ObjectResult)!.StatusCode;
            Assert.That(statusCodeResult, Is.EqualTo(500));
        }

        [Test]
        public async Task GetCampaignProducts_ReturnsOkWithCampaignProducts()
        {
            // Arrange
            var expectedProducts = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Galaxy Z Flip 4 256 GB", ProductDescription = "Güzel telefon", ProductPrice = 40000, UserId = 2, CampaignId = 1 }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.GetCampaignProductsQuery>(), default))
                         .ReturnsAsync(expectedProducts);

            // Act
            var result = await _controller.GetCampaignProducts(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var products = (result.Result as OkObjectResult)!.Value as IEnumerable<Product>;
            Assert.That(products, Is.Not.Null);
            Assert.That(expectedProducts, Has.Count.EqualTo(products.Count()));
        }

        [Test]
        public async Task GetCampaignProducts_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.GetCampaignProductsQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong!"));

            // Act
            var result = await _controller.GetCampaignProducts(500);

            // Assert
            Assert.That(result, Is.Not.Null);

            var statusCodeResult = (result.Result as ObjectResult)!.StatusCode;
            Assert.That(statusCodeResult, Is.EqualTo(500));
        }

        [Test]
        public async Task GetProductInfo_ReturnsOkWithProductInfo_WithNormalUser()
        {
            // Arrange
            var expectedProductInfo = new ProductResponse { BrandName = "Samsung", CampaignName = "%20 İndirim", ProductName = "Galaxy Z Flip 4 256 GB", ProductDescription = "Güzel telefon", ProductPrice = 40000 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.GetProductInfoQuery>(), default))
                         .ReturnsAsync(expectedProductInfo);

            // Act
            var result = await _controller.GetProductInfo(1, 1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var product = (result.Result as OkObjectResult)!.Value as ProductResponse;
            Assert.That(product, Is.Not.Null);
            Assert.That(product, Is.SameAs(expectedProductInfo));
        }

        [Test]
        public async Task GetProductInfo_ReturnsOkWithProductInfo_WithAdminUser()
        {
            // Arrange
            var expectedProductInfo = new ProductResponse { BrandName = "Samsung", CampaignName = "%20 İndirim", ProductName = "Galaxy Z Flip 4 256 GB", ProductDescription = "Güzel telefon", ProductPrice = 40000 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.GetProductInfoQuery>(), default))
                         .ReturnsAsync(expectedProductInfo);

            // Act
            var result = await _controller.GetProductInfo(2, 1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var product = (result.Result as OkObjectResult)!.Value as ProductResponse;
            Assert.That(product, Is.Not.Null);
            Assert.That(product, Is.SameAs(expectedProductInfo));
        }

        [Test]
        public async Task GetProductInfo_ReturnsNotFoundWithNonExistingProductId()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.GetProductInfoQuery>(), default))
                         .ReturnsAsync((ProductResponse)null);

            // Act
            var result = await _controller.GetProductInfo(1, 6);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<NotFoundResult>(result.Result);

            var statusCodeResult = (result.Result as NotFoundResult)!.StatusCode;
            Assert.That(statusCodeResult, Is.EqualTo(404));
        }

        [Test]
        public async Task GetProductInfo_ReturnsNotFoundProductOfAnotherAdmin_WithAdminUser()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.GetProductInfoQuery>(), default))
                         .ReturnsAsync((ProductResponse)null);

            // Act
            var result = await _controller.GetProductInfo(2, 3);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<NotFoundResult>(result.Result);

            var statusCodeResult = (result.Result as NotFoundResult)!.StatusCode;
            Assert.That(statusCodeResult, Is.EqualTo(404));
        }

        [Test]
        public async Task GetProductInfo_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.GetProductInfoQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong!"));

            // Act
            var result = await _controller.GetProductInfo(500, 500);

            // Assert
            Assert.That(result, Is.Not.Null);

            var statusCodeResult = (result.Result as ObjectResult)!.StatusCode;
            Assert.That(statusCodeResult, Is.EqualTo(500));
        }

        [Test]
        public async Task CreateProduct_ReturnsOkWithCreatedProduct()
        {
            // Arrange
            ProductForCreationDto product = new ProductForCreationDto { ProductName = "Iphone 11 64 GB", ProductDescription = "Şık telefon", ProductPrice = 22500, UserId = 3, CampaignId = 3 };
            Product expectedCreatedProduct = new Product { ProductId = 5, ProductName = "Iphone 11 64 GB", ProductDescription = "Şık telefon", ProductPrice = 22500, UserId = 3, CampaignId = 3 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.CreateProductQuery>(), default))
                         .ReturnsAsync(expectedCreatedProduct);
            // Act
            var result = await _controller.CreateProduct(product);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);

            var returnedProduct = (result.Result as CreatedAtActionResult)!.Value as Product;
            Assert.That(returnedProduct, Is.Not.Null);
            Assert.That(returnedProduct, Is.SameAs(expectedCreatedProduct));
        }

        [Test]
        public async Task CreateProduct_DoesNotCreateProductWithNormalUserId()
        {
            // Arrange
            ProductForCreationDto product = new ProductForCreationDto { ProductName = "Iphone 11 64 GB", ProductDescription = "Şık telefon", ProductPrice = 22500, UserId = 3, CampaignId = 3 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.CreateProductQuery>(), default))
                         .ThrowsAsync(new Exception("Only admin users can add products."));

            // Act
            var result = await _controller.CreateProduct(product);

            // Assert
            Assert.That(result, Is.Not.Null);

            var returnedResult = result.Result as ObjectResult;
            Assert.That(returnedResult.StatusCode, Is.EqualTo(500));
            Assert.That(returnedResult.Value, Is.EqualTo("Only admin users can add products."));
        }

        [Test]
        public async Task CreateCampaign_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            ProductForCreationDto product = new ProductForCreationDto { ProductName = "Iphone 11 64 GB", ProductDescription = "Şık telefon", ProductPrice = 22500, UserId = 3, CampaignId = 3 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.CreateProductQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.CreateProduct(product);

            // Assert
            Assert.That(result, Is.Not.Null);

            var statusCodeResult = (result.Result as ObjectResult)!.StatusCode;
            Assert.That(statusCodeResult, Is.EqualTo(500));
        }

        [Test]
        public async Task UpdateProduct_UpdatesProductSuccessfully()
        {
            // Arrange
            ProductForUpdateDto product = new ProductForUpdateDto { ProductDescription = "İyi telefon", ProductPrice = 40000, CampaignId = null };
            Product expectedUpdatedProduct = new Product { ProductId = 3, ProductName = "Iphone 12 128 GB", ProductDescription = "İyi telefon", ProductPrice = 40000, UserId = 3, CampaignId = null };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.GetProductQuery>(), default))
                         .ReturnsAsync(expectedUpdatedProduct);

            // Act
            var result = await _controller.UpdateProduct(3, product) as NoContentResult;
            var updatedProduct = await _controller.GetProduct(3);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));

            var returnedProduct = (updatedProduct.Result as OkObjectResult)!.Value as Product;
            Assert.That(returnedProduct, Is.Not.Null);
            Assert.That(returnedProduct, Is.SameAs(expectedUpdatedProduct));
        }

        [Test]
        public async Task UpdateProduct_ReturnsNotFoundWithNonExistingProductId()
        {
            // Arrange
            ProductForUpdateDto product = new ProductForUpdateDto { ProductDescription = "İyi telefon", ProductPrice = 40000, CampaignId = null };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.GetProductQuery>(), default))
                         .ReturnsAsync((Product)null);

            // Act
            var result = await _controller.UpdateProduct(6, product) as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task UpdateProduct_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            ProductForUpdateDto product = new ProductForUpdateDto { ProductDescription = "İyi telefon", ProductPrice = 40000, CampaignId = null };
            Product expectedUpdatedProduct = new Product { ProductId = 3, ProductName = "Iphone 12 128 GB", ProductDescription = "İyi telefon", ProductPrice = 40000, UserId = 3, CampaignId = null };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.GetProductQuery>(), default))
                         .ReturnsAsync(expectedUpdatedProduct);

            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.UpdateProductQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.UpdateProduct(1, product) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.EqualTo("Something went wrong"));
        }

        [Test]
        public async Task DeleteProduct_DeletesProductSuccessfully()
        {
            // Arrange
            Product existingProduct = new Product { ProductId = 3, ProductName = "Iphone 12 128 GB", ProductDescription = "Çok iyi telefon", ProductPrice = 42000, UserId = 3, CampaignId = 3 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.GetProductQuery>(), default))
                         .ReturnsAsync(existingProduct);

            // Act
            var result = await _controller.DeleteProduct(1) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));
        }

        [Test]
        public async Task DeleteProduct_ReturnsNotFoundWithNonExistingProductId()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.GetProductQuery>(), default))
                         .ReturnsAsync((Product)null);

            // Act
            var result = await _controller.DeleteProduct(5) as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task DeleteProduct_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            Product existingProduct = new Product { ProductId = 3, ProductName = "Iphone 12 128 GB", ProductDescription = "Çok iyi telefon", ProductPrice = 42000, UserId = 3, CampaignId = 3 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.GetProductQuery>(), default))
                         .ReturnsAsync(existingProduct);

            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.DeleteProductQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong!"));

            // Act
            var result = await _controller.DeleteProduct(500) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task DeleteUserProducts_DeletesUserProductsSuccessfully()
        {
            // Arrange
            // Act
            var result = await _controller.DeleteUserProducts(2) as NoContentResult;
            var deletedProducts = await _controller.GetUserProducts(2);
            var deletedProductsList = (deletedProducts.Result as OkObjectResult)!.Value as IEnumerable<Product>;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));

            Assert.That(deletedProducts, Is.Not.Null);
            Assert.That(deletedProductsList.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteUserProducts_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.DeleteUserProductsQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong!"));

            // Act
            var result = await _controller.DeleteUserProducts(500) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task DeleteCampaignProducts_DeletesCampaignProductsSuccessfully()
        {
            // Arrange
            // Act
            var result = await _controller.DeleteCampaignProducts(1) as NoContentResult;
            var deletedProducts = await _controller.GetCampaignProducts(1);
            var deletedProductsList = (deletedProducts.Result as OkObjectResult)!.Value as IEnumerable<Product>;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));

            Assert.That(deletedProducts, Is.Not.Null);
            Assert.That(deletedProductsList.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteCampaignProducts_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<ProductMediator.DeleteCampaignProductsQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong!"));

            // Act
            var result = await _controller.DeleteCampaignProducts(500) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
        }
    }
}
