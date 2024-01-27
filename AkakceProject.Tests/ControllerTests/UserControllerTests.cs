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
    public class UserControllerTests
    {
        private UserController _controller;
        private Mock<IMediator> _mediatorMock;

        public List<User> dbUsers;
        public List<Campaign> dbCampaigns;
        public List<Product> dbProducts;

        [SetUp]
        public void Setup()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new UserController(_mediatorMock.Object);

            dbUsers = new List<User>
            {
                new User { UserId = 1, UserName = "asli", UserPassword = "userasli", EmailAddress = "asli@gmail.com", PhoneNumber = "", IsAdmin = false, IsUserActive = true },
                new User { UserId = 2, UserName = "Samsung", UserPassword = "samsung.admin", EmailAddress = "info@samsung.com", PhoneNumber = "08502256060", IsAdmin = true, IsUserActive = true },
                new User { UserId = 3, UserName = "Apple", UserPassword = "apple.admin", EmailAddress = "info@apple.com", PhoneNumber = "08502257979", IsAdmin = true, IsUserActive = true }
            };

            dbCampaigns = new List<Campaign>
            {
                new Campaign { CampaignId = 1, CampaignTitle = "%20 �ndirim", IsActive = true, StartsAt = "2023-09-06", EndsAt = "2023-09-15", UserId = 2},
                new Campaign { CampaignId = 2, CampaignTitle = "Sigorta Hediye", IsActive = true, StartsAt = "2023-09-06", EndsAt = "2023-09-15", UserId = 2},
                new Campaign { CampaignId = 3, CampaignTitle = "%10 �ndirim", IsActive = false, StartsAt = "2023-09-08", EndsAt = "2023-09-20", UserId = 3}
            };

            dbProducts = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Galaxy Z Flip 4 256 GB", ProductDescription = "G�zel telefon", ProductPrice = 40000, UserId = 2, CampaignId = 1 },
                new Product { ProductId = 2, ProductName = "Galaxy Z Flip 3 128 GB", ProductDescription = "Sa�lam telefon", ProductPrice = 30000, UserId = 2, CampaignId = 2 },
                new Product { ProductId = 3, ProductName = "Iphone 12 128 GB", ProductDescription = "�ok iyi telefon", ProductPrice = 42000, UserId = 3, CampaignId = 3 },
                new Product { ProductId = 4, ProductName = "Galaxy A3 64 GB", ProductDescription = "�darelik telefon", ProductPrice = 6000, UserId = 2, CampaignId = null },
            };
        }

        [Test]
        public async Task GetUsers_ReturnsOkWithUsers()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<UserMediator.GetAllUsersQuery>(), default))
                         .ReturnsAsync(dbUsers);

            // Act
            var result = await _controller.GetUsers();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var users = (result.Result as OkObjectResult)!.Value as IEnumerable<User>;
            Assert.That(users, Is.Not.Null);
            Assert.That(dbUsers, Has.Count.EqualTo(users.Count()));
        }

        [Test]
        public async Task GetUsers_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<UserMediator.GetAllUsersQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.GetUsers();

            // Assert
            Assert.That(result, Is.Not.Null);

            var statusCodeResult = (result.Result as ObjectResult)!.StatusCode;
            Assert.That(statusCodeResult, Is.EqualTo(500));
        }

        [Test]
        public async Task GetUser_ReturnsOkWithUser()
        {
            // Arrange
            var expectedUser = dbUsers[1];

            _mediatorMock.Setup(m => m.Send(It.IsAny<UserMediator.GetUserQuery>(), default))
                         .ReturnsAsync(expectedUser);

            // Act
            var result = await _controller.GetUser(2);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var user = (result.Result as OkObjectResult)!.Value as User;
            Assert.That(user, Is.Not.Null);
            Assert.That(user, Is.EqualTo(expectedUser));
        }

        [Test]
        public async Task GetUser_ReturnsNotFoundWithNonExistingUserId()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<UserMediator.GetUserQuery>(), default))
                         .ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetUser(5);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<NotFoundResult>(result.Result);

            var statusCodeResult = (result.Result as NotFoundResult)!.StatusCode;
            Assert.That(statusCodeResult, Is.EqualTo(404));
        }

        [Test]
        public async Task GetUser_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<UserMediator.GetUserQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.GetUser(500);

            // Assert
            Assert.That(result, Is.Not.Null);

            var statusCodeResult = (result.Result as ObjectResult)!.StatusCode;
            Assert.That(statusCodeResult, Is.EqualTo(500));
        }

        [Test]
        public async Task GetUserInfo_ReturnsOkWithNormalUserResponse()
        {
            // Arrange
            UserResponse user = new UserResponse
            {
                UserName = "asli",
                EmailAddress = "asli@gmail.com",
                PhoneNumber = "",
                UserCampaigns = new List<CampaignResponse>
                {
                    new CampaignResponse { AuthUserName = "Samsung", CampaignTitle = "%20 �ndirim", IsActive = true, StartsAt = "2023-09-06", EndsAt = "2023-09-15" },
                    new CampaignResponse { AuthUserName = "Samsung", CampaignTitle = "Sigorta Hediye", IsActive = true, StartsAt = "2023-09-06", EndsAt = "2023-09-15" },
                    new CampaignResponse { AuthUserName = "Apple", CampaignTitle = "%10 �ndirim", IsActive = false, StartsAt = "2023-09-08", EndsAt = "2023-09-20" }
                },
                UserProducts = new List<ProductResponse>
                {
                    new ProductResponse { BrandName = "Samsung", CampaignName = "%20 �ndirim", ProductName = "Galaxy Z Flip 4 256 GB", ProductDescription = "G�zel telefon", ProductPrice = 40000 },
                    new ProductResponse { BrandName = "Samsung", CampaignName = "%Sigorta Hediye", ProductName = "Galaxy Z Flip 3 128 GB", ProductDescription = "Sa�lam telefon", ProductPrice = 30000 },
                    new ProductResponse { BrandName = "Apple", CampaignName = "%10 �ndirim", ProductName = "Iphone 12 128 GB", ProductDescription = "�ok iyi telefon", ProductPrice = 42000 },
                    new ProductResponse { BrandName = "Samsung", CampaignName = null, ProductName = "Galaxy A3 64 GB", ProductDescription = "�darelik telefon", ProductPrice = 6000 },
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UserMediator.GetUserInfoQuery>(), default))
                         .ReturnsAsync(user);

            // Act
            var result = await _controller.GetUserInfo(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var returnedUser = (result.Result as OkObjectResult)!.Value as UserResponse;
            Assert.That(returnedUser, Is.Not.Null);
            Assert.That(returnedUser.UserCampaigns, Has.Count.EqualTo(user.UserCampaigns.Count));
            Assert.That(returnedUser.UserProducts, Has.Count.EqualTo(user.UserProducts.Count));
        }

        [Test]
        public async Task GetUserInfo_ReturnsOkWithAdminUserResponse()
        {
            // Arrange
            UserResponse user = new UserResponse
            {
                UserName = "Samsung",
                EmailAddress = "info@samsung.com",
                PhoneNumber = "08502256060",
                UserCampaigns = new List<CampaignResponse>
                {
                    new CampaignResponse { AuthUserName = "Samsung", CampaignTitle = "%20 �ndirim", IsActive = true, StartsAt = "2023-09-06", EndsAt = "2023-09-15" },
                    new CampaignResponse { AuthUserName = "Samsung", CampaignTitle = "Sigorta Hediye", IsActive = true, StartsAt = "2023-09-06", EndsAt = "2023-09-15" }
                },
                UserProducts = new List<ProductResponse>
                {
                    new ProductResponse { BrandName = "Samsung", CampaignName = "%20 �ndirim", ProductName = "Galaxy Z Flip 4 256 GB", ProductDescription = "G�zel telefon", ProductPrice = 40000 },
                    new ProductResponse { BrandName = "Samsung", CampaignName = "%Sigorta Hediye", ProductName = "Galaxy Z Flip 3 128 GB", ProductDescription = "Sa�lam telefon", ProductPrice = 30000 },
                    new ProductResponse { BrandName = "Samsung", CampaignName = null, ProductName = "Galaxy A3 64 GB", ProductDescription = "�darelik telefon", ProductPrice = 6000 },
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UserMediator.GetUserInfoQuery>(), default))
                         .ReturnsAsync(user);

            // Act
            var result = await _controller.GetUserInfo(2);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var returnedUser = (result.Result as OkObjectResult)!.Value as UserResponse;
            Assert.That(returnedUser, Is.Not.Null);
            Assert.That(returnedUser.UserCampaigns, Has.Count.EqualTo(user.UserCampaigns.Count));
            Assert.That(returnedUser.UserProducts, Has.Count.EqualTo(user.UserProducts.Count));
        }

        [Test]
        public async Task GetUserInfo_ReturnsNotFoundWithNonExistingUserId()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<UserMediator.GetUserInfoQuery>(), default))
                         .ReturnsAsync((UserResponse)null);

            // Act
            var result = await _controller.GetUserInfo(4);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<NotFoundResult>(result.Result);

            var statusCodeResult = (result.Result as NotFoundResult)!.StatusCode;
            Assert.That(statusCodeResult, Is.EqualTo(404));
        }

        [Test]
        public async Task GetUserInfo_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<UserMediator.GetUserInfoQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.GetUserInfo(500);

            // Assert
            Assert.That(result, Is.Not.Null);

            var statusCodeResult = (result.Result as ObjectResult)!.StatusCode;
            Assert.That(statusCodeResult, Is.EqualTo(500));
        }

        [Test]
        public async Task CreateUser_ReturnsOkWithCreatedUser()
        {
            // Arrange
            UserForCreationDto user = new UserForCreationDto { UserName = "taha", UserPassword = "ace.of_spades", EmailAddress = "taha@gmail.com", PhoneNumber = "", IsAdmin = false };
            User expectedCreatedUser = new User { UserId = 4, UserName = "taha", UserPassword = "ace.of_spades", EmailAddress = "taha@gmail.com", PhoneNumber = "", IsAdmin = false, IsUserActive = true };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UserMediator.CreateUserQuery>(), default))
                         .ReturnsAsync(expectedCreatedUser);

            // Act
            var result = await _controller.CreateUser(user);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);

            var returnedUser = (result.Result as CreatedAtActionResult)!.Value as User;
            Assert.That(returnedUser, Is.Not.Null);
            Assert.That(returnedUser, Is.SameAs(expectedCreatedUser));
        }

        [Test]
        public async Task CreateUser_DoesNotCreateUserWithSameUserName()
        {
            // Arrange
            UserForCreationDto user = new UserForCreationDto { UserName = "asli", UserPassword = "ace.of_spades", EmailAddress = "taha@gmail.com", PhoneNumber = "", IsAdmin = false };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UserMediator.CreateUserQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.CreateUser(user);

            // Assert
            Assert.That(result, Is.Not.Null);

            var statusCodeResult = (result.Result as ObjectResult)!.StatusCode;
            Assert.That(statusCodeResult, Is.EqualTo(500));
        }

        [Test]
        public async Task UpdateUser_UpdatesUserSuccessfully()
        {
            // Arrange
            UserForUpdateDto user = new UserForUpdateDto { UserName = "asli", UserPassword = "userasli", EmailAddress = "asli@gmail.com", PhoneNumber = "05323964657" };
            User expectedUpdatedUser = new User { UserId = 1, UserName = "asli", UserPassword = "userasli", EmailAddress = "asli@gmail.com", PhoneNumber = "05323964657", IsAdmin = false, IsUserActive = true };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UserMediator.GetUserQuery>(), default))
                         .ReturnsAsync(expectedUpdatedUser);

            // Act
            var result = await _controller.UpdateUser(1, user) as NoContentResult;
            var updatedUser = await _controller.GetUser(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));

            var returnedUser = (updatedUser.Result as OkObjectResult)!.Value as User;
            Assert.That(returnedUser, Is.Not.Null);
            Assert.That(returnedUser, Is.SameAs(expectedUpdatedUser));
        }

        [Test]
        public async Task UpdateUser_ReturnsNotFoundWithNonExistingUserId()
        {
            // Arrange
            UserForUpdateDto user = new UserForUpdateDto { UserName = "taha", UserPassword = "usertaha", EmailAddress = "taha@gmail.com", PhoneNumber = "" };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UserMediator.GetUserQuery>(), default))
                         .ReturnsAsync((User)null);

            // Act
            var result = await _controller.UpdateUser(4, user) as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task UpdateUser_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            UserForUpdateDto user = new UserForUpdateDto { UserName = "asli", UserPassword = "userasli", EmailAddress = "asli@gmail.com", PhoneNumber = "05323964657" };
            User expectedUpdatedUser = new User { UserId = 1, UserName = "asli", UserPassword = "userasli", EmailAddress = "asli@gmail.com", PhoneNumber = "05323964657", IsAdmin = false, IsUserActive = true };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UserMediator.GetUserQuery>(), default))
                         .ReturnsAsync(expectedUpdatedUser);

            _mediatorMock.Setup(m => m.Send(It.IsAny<UserMediator.UpdateUserQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.UpdateUser(1, user) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.EqualTo("Something went wrong"));
        }

        [Test]
        public async Task DeactivateUser_DeactivatesUserSuccessfully()
        {
            // Arrange
            User expectedDeactivatedUser = new User { UserId = 1, UserName = "asli", UserPassword = "userasli", EmailAddress = "asli@gmail.com", PhoneNumber = "", IsAdmin = false, IsUserActive = false };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UserMediator.GetUserQuery>(), default))
                         .ReturnsAsync(expectedDeactivatedUser);

            // Act
            var result = await _controller.DeactivateUser(1) as NoContentResult;
            var updatedUser = await _controller.GetUser(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));

            var returnedUser = (updatedUser.Result as OkObjectResult)!.Value as User;
            Assert.That(returnedUser, Is.Not.Null);
            Assert.That(returnedUser.IsUserActive, Is.False);
        }

        [Test]
        public async Task DeactivateUser_ReturnsNotFoundWithNonExistingUserId()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<UserMediator.GetUserQuery>(), default))
                         .ReturnsAsync((User)null);

            // Act
            var result = await _controller.DeactivateUser(4) as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task DeactivateUser_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            User expectedUpdatedUser = new User { UserId = 1, UserName = "asli", UserPassword = "userasli", EmailAddress = "asli@gmail.com", PhoneNumber = "", IsAdmin = false, IsUserActive = false };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UserMediator.GetUserQuery>(), default))
                         .ReturnsAsync(expectedUpdatedUser);

            _mediatorMock.Setup(m => m.Send(It.IsAny<UserMediator.DeactivateUserQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.DeactivateUser(4) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.EqualTo("Something went wrong"));
        }
    }
}