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
    public class CampaignControllerTests
    {
        private CampaignController _controller;
        private Mock<IMediator> _mediatorMock;

        public List<User> dbUsers;
        public List<Campaign> dbCampaigns;
        public List<Product> dbProducts;

        [SetUp]
        public void Setup()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new CampaignController(_mediatorMock.Object);

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
        public async Task GetCampaigns_ReturnsOkWithCampaigns()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.GetAllCampaignsQuery>(), default))
                         .ReturnsAsync(dbCampaigns);

            // Act
            var result = await _controller.GetCampaigns();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var campaigns = (result.Result as OkObjectResult)!.Value as IEnumerable<Campaign>;
            Assert.That(campaigns, Is.Not.Null);
            Assert.That(dbCampaigns, Has.Count.EqualTo(campaigns.Count()));
        }

        [Test]
        public async Task GetCampaigns_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.GetAllCampaignsQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.GetCampaigns();

            // Assert
            Assert.That(result, Is.Not.Null);

            var statusCodeResult = (result.Result as ObjectResult)!.StatusCode;
            Assert.That(statusCodeResult, Is.EqualTo(500));
        }

        [Test]
        public async Task GetCampaign_ReturnsOkWithCampaign()
        {
            // Arrange
            var expectedCampaign = dbCampaigns[0];

            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.GetCampaignQuery>(), default))
                         .ReturnsAsync(expectedCampaign);

            // Act
            var result = await _controller.GetCampaign(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var campaign = (result.Result as OkObjectResult)!.Value as Campaign;
            Assert.That(campaign, Is.Not.Null);
            Assert.That(campaign, Is.SameAs(expectedCampaign));
        }

        [Test]
        public async Task GetCampaign_ReturnsNotFoundWithNonExistingCampaignId()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.GetCampaignQuery>(), default))
                         .ReturnsAsync((Campaign)null);

            // Act
            var result = await _controller.GetCampaign(5);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<NotFoundResult>(result.Result);

            var statusCodeResult = (result.Result as NotFoundResult)!.StatusCode;
            Assert.That(statusCodeResult, Is.EqualTo(404));
        }

        [Test]
        public async Task GetCampaign_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.GetCampaignQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.GetCampaign(500);

            // Assert
            Assert.That(result, Is.Not.Null);

            var statusCodeResult = (result.Result as ObjectResult)!.StatusCode;
            Assert.That(statusCodeResult, Is.EqualTo(500));
        }

        [Test]
        public async Task GetUserCampaigns_ReturnsOkWithUsers()
        {
            // Arrange
            var expectedCampaigns = new List<Campaign>
            {
                new Campaign { CampaignId = 1, CampaignTitle = "%20 İndirim", IsActive = true, StartsAt = "2023-09-06", EndsAt = "2023-09-15", UserId = 2},
                new Campaign { CampaignId = 2, CampaignTitle = "Sigorta Hediye", IsActive = true, StartsAt = "2023-09-06", EndsAt = "2023-09-15", UserId = 2}
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.GetUserCampaignsQuery>(), default))
                         .ReturnsAsync(expectedCampaigns);

            // Act
            var result = await _controller.GetUserCampaigns(2);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var campaigns = (result.Result as OkObjectResult)!.Value as IEnumerable<Campaign>;
            Assert.That(campaigns, Is.Not.Null);
            Assert.That(expectedCampaigns, Has.Count.EqualTo(campaigns.Count()));
        }

        [Test]
        public async Task GetUserCampaigns_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.GetUserCampaignsQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.GetUserCampaigns(500);

            // Assert
            Assert.That(result, Is.Not.Null);

            var statusCodeResult = (result.Result as ObjectResult)!.StatusCode;
            Assert.That(statusCodeResult, Is.EqualTo(500));
        }

        [Test]
        public async Task GetCampaignInfo_ReturnsOkWithCampaignResponse_WithNormalUser()
        {
            // Arrange
            var expectedCampaignInfo = new CampaignResponse { AuthUserName = "Samsung", CampaignTitle = "%20 İndirim", IsActive = true, StartsAt = "2023-09-06", EndsAt = "2023-09-15" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.GetCampaignInfoQuery>(), default))
                         .ReturnsAsync(expectedCampaignInfo);

            // Act
            var result = await _controller.GetCampaignInfo(1, 1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var campaign = (result.Result as OkObjectResult)!.Value as CampaignResponse;
            Assert.That(campaign, Is.Not.Null);
            Assert.That(campaign, Is.SameAs(expectedCampaignInfo));
        }

        [Test]
        public async Task GetCampaignInfo_ReturnsOkWithCampaignResponse_WithAdminUser()
        {
            // Arrange
            var expectedCampaignInfo = new CampaignResponse { AuthUserName = "Samsung", CampaignTitle = "%20 İndirim", IsActive = true, StartsAt = "2023-09-06", EndsAt = "2023-09-15" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.GetCampaignInfoQuery>(), default))
                         .ReturnsAsync(expectedCampaignInfo);

            // Act
            var result = await _controller.GetCampaignInfo(2, 1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<OkObjectResult>(result.Result);

            var campaign = (result.Result as OkObjectResult)!.Value as CampaignResponse;
            Assert.That(campaign, Is.Not.Null);
            Assert.That(campaign, Is.SameAs(expectedCampaignInfo));
        }

        [Test]
        public async Task GetCampaignInfo_ReturnsNotFoundCampaignOfAnotherAdmin_WithAdminUser()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.GetCampaignInfoQuery>(), default))
                         .ReturnsAsync((CampaignResponse)null);

            // Act
            var result = await _controller.GetCampaignInfo(2, 3);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task GetCampaignInfo_ReturnsNotFoundWithNonExistingCampaignId()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.GetCampaignInfoQuery>(), default))
                         .ReturnsAsync((CampaignResponse)null);

            // Act
            var result = await _controller.GetCampaignInfo(1, 50);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task GetCampaignInfo_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.GetCampaignInfoQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.GetCampaignInfo(500, 500);

            // Assert
            Assert.That(result, Is.Not.Null);

            var statusCodeResult = (result.Result as ObjectResult)!.StatusCode;
            Assert.That(statusCodeResult, Is.EqualTo(500));
        }

        [Test]
        public async Task CreateCampaign_ReturnsOkWithCreatedCampaign()
        {
            // Arrange
            CampaignForCreationDto campaign = new CampaignForCreationDto { CampaignTitle = "3 Al 2 Öde", IsActive = false, StartsAt = "2023-10-15", EndsAt = "2023-10-22", UserId = 2 };
            Campaign expectedCreatedCampaign = new Campaign { CampaignId = 4, CampaignTitle = "3 Al 2 Öde", IsActive = false, StartsAt = "2023-10-15", EndsAt = "2023-10-22", UserId = 2 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.CreateCampaignQuery>(), default))
                         .ReturnsAsync(expectedCreatedCampaign);

            // Act
            var result = await _controller.CreateCampaign(campaign);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.IsInstanceOf<CreatedAtActionResult>(result.Result);

            var returnedCampaign = (result.Result as CreatedAtActionResult)!.Value as Campaign;
            Assert.That(returnedCampaign, Is.Not.Null);
            Assert.That(returnedCampaign, Is.SameAs(expectedCreatedCampaign));
        }

        [Test]
        public async Task CreateCampaign_DoesNotCreateCampaignWithNormalUserId()
        {
            // Arrange
            CampaignForCreationDto campaign = new CampaignForCreationDto { CampaignTitle = "3 Al 2 Öde", IsActive = false, StartsAt = "2023-10-15", EndsAt = "2023-10-22", UserId = 1 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.CreateCampaignQuery>(), default))
                         .ThrowsAsync(new Exception("Only admin users can create campaigns."));

            // Act
            var result = await _controller.CreateCampaign(campaign);

            // Assert
            Assert.That(result, Is.Not.Null);

            var returnedResult = result.Result as ObjectResult;
            Assert.That(returnedResult.StatusCode, Is.EqualTo(500));
            Assert.That(returnedResult.Value, Is.EqualTo("Only admin users can create campaigns."));
        }

        [Test]
        public async Task CreateCampaign_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            CampaignForCreationDto campaign = new CampaignForCreationDto { CampaignTitle = "3 Al 2 Öde", IsActive = false, StartsAt = "2023-10-15", EndsAt = "2023-10-22", UserId = 1 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.CreateCampaignQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.CreateCampaign(campaign);

            // Assert
            Assert.That(result, Is.Not.Null);

            var statusCodeResult = (result.Result as ObjectResult)!.StatusCode;
            Assert.That(statusCodeResult, Is.EqualTo(500));
        }

        [Test]
        public async Task UpdateCampaign_UpdatesCampaignSuccessfully()
        {
            // Arrange
            CampaignForUpdateDto campaign = new CampaignForUpdateDto { IsActive = false, StartsAt = "2023-09-15", EndsAt = "2023-09-22" };
            Campaign expectedUpdatedCampaign = new Campaign { CampaignId = 1, CampaignTitle = "%20 İndirim", IsActive = false, StartsAt = "2023-09-15", EndsAt = "2023-09-22", UserId = 2 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.GetCampaignQuery>(), default))
                         .ReturnsAsync(expectedUpdatedCampaign);

            // Act
            var result = await _controller.UpdateCampaign(1, campaign) as NoContentResult;
            var updatedCampaign = await _controller.GetCampaign(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));

            var returnedCampaign = (updatedCampaign.Result as OkObjectResult)!.Value as Campaign;
            Assert.That(returnedCampaign, Is.Not.Null);
            Assert.That(returnedCampaign, Is.SameAs(expectedUpdatedCampaign));
        }

        [Test]
        public async Task UpdateCampaign_ReturnsNotFoundWithNonExistingCampaignId()
        {
            // Arrange
            CampaignForUpdateDto campaign = new CampaignForUpdateDto { IsActive = false, StartsAt = "2023-09-15", EndsAt = "2023-09-22" };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.GetCampaignQuery>(), default))
                         .ReturnsAsync((Campaign)null);

            // Act
            var result = await _controller.UpdateCampaign(4, campaign) as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task UpdateCampaign_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            CampaignForUpdateDto campaign = new CampaignForUpdateDto { IsActive = false, StartsAt = "2023-09-15", EndsAt = "2023-09-22" };
            Campaign expectedUpdatedCampaign = new Campaign { CampaignId = 1, CampaignTitle = "%20 İndirim", IsActive = false, StartsAt = "2023-09-15", EndsAt = "2023-09-22", UserId = 2 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.GetCampaignQuery>(), default))
                         .ReturnsAsync(expectedUpdatedCampaign);

            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.UpdateCampaignQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong"));

            // Act
            var result = await _controller.UpdateCampaign(1, campaign) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
            Assert.That(result.Value, Is.EqualTo("Something went wrong"));
        }

        [Test]
        public async Task DeleteCampaign_DeletesCampaignSuccessfully()
        {
            // Arrange
            Campaign existingCampaign = new Campaign { CampaignId = 1, CampaignTitle = "%20 İndirim", IsActive = true, StartsAt = "2023-09-06", EndsAt = "2023-09-15", UserId = 2 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.GetCampaignQuery>(), default))
                         .ReturnsAsync(existingCampaign);

            // Act
            var result = await _controller.DeleteCampaign(1) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));
        }

        [Test]
        public async Task DeleteCampaign_ReturnsNotFoundWithNonExistingCampaignId()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.GetCampaignQuery>(), default))
                         .ReturnsAsync((Campaign)null);

            // Act
            var result = await _controller.DeleteCampaign(5) as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task DeleteCampaign_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            Campaign existingCampaign = new Campaign { CampaignId = 1, CampaignTitle = "%20 İndirim", IsActive = true, StartsAt = "2023-09-06", EndsAt = "2023-09-15", UserId = 2 };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.GetCampaignQuery>(), default))
                         .ReturnsAsync(existingCampaign);

            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.DeleteCampaignQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong!"));

            // Act
            var result = await _controller.DeleteCampaign(500) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task DeleteUserCampaigns_DeletesUserCampaignsSuccessfully()
        {
            // Arrange
            // Act
            var result = await _controller.DeleteUserCampaigns(2) as NoContentResult;
            var deletedCampaigns = await _controller.GetUserCampaigns(2);
            var deletedCampaignsList = (deletedCampaigns.Result as OkObjectResult)!.Value as IEnumerable<Campaign>;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(204));

            Assert.That(deletedCampaigns, Is.Not.Null);
            Assert.That(deletedCampaignsList.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task DeleteUserCampaigns_ReturnsInternalServerErrorOnError()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<CampaignMediator.DeleteUserCampaignsQuery>(), default))
                         .ThrowsAsync(new Exception("Something went wrong!"));

            // Act
            var result = await _controller.DeleteUserCampaigns(500) as ObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
        }
    }
}
