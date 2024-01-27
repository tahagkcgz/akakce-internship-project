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
    public class CampaignRepositoryTests
    {
        private Mock<IDapperContext> _contextMock;
        private CampaignRepository _campaignRepository;

        public List<User> dbUsers;
        public List<Campaign> dbCampaigns;
        public List<Product> dbProducts;

        [SetUp]
        public void Setup()
        {
            _contextMock = new Mock<IDapperContext>();
            _campaignRepository = new CampaignRepository(_contextMock.Object);

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
        public async Task GetCampaigns_ReturnsListOfCampaigns()
        {
            // Arrange
            _contextMock.Setup(c => c.QueryAsync<Campaign>(It.IsAny<string>(), null))
                                     .ReturnsAsync(dbCampaigns);

            // Act
            var result = await _campaignRepository.GetCampaigns();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<IEnumerable<Campaign>>());
            Assert.That(dbCampaigns, Has.Count.EqualTo(result.Count()));
        }

        [Test]
        public async Task GetCampaign_ReturnsCampaign()
        {
            // Arrange
            var expectedCampaign = dbCampaigns[0];

            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<Campaign>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync(expectedCampaign);

            // Act
            var result = await _campaignRepository.GetCampaign(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<Campaign>());
            Assert.That(result, Is.SameAs(expectedCampaign));
        }

        [Test]
        public async Task GetCampaign_ReturnsNullWithNonExistingCampaignId()
        {
            // Arrange
            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<Campaign>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync((Campaign)null);

            // Act
            var result = await _campaignRepository.GetCampaign(5);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetUserCampaigns_ReturnsUserCampaigns()
        {
            // Arrange
            var expectedCampaigns = new List<Campaign> { dbCampaigns[0], dbCampaigns[1] };

            _contextMock.Setup(c => c.QueryAsync<Campaign>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync(expectedCampaigns);

            // Act
            var result = await _campaignRepository.GetUserCampaigns(2);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<IEnumerable<Campaign>>());
            Assert.That(expectedCampaigns, Has.Count.EqualTo(result.Count()));
        }

        [Test]
        public async Task GetCampaignInfo_ReturnsCampaignInfo()
        {
            // Arrange
            var expectedCampaignInfo = new CampaignResponse
            {
                AuthUserName = "Samsung",
                CampaignTitle = "%20 İndirim",
                IsActive = true,
                StartsAt = "2023-09-06",
                EndsAt = "2023-09-15"
            };

            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<bool?>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync(false);

            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<CampaignResponse>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync(expectedCampaignInfo);

            // Act
            var result = await _campaignRepository.GetCampaignInfo(1, 1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.SameAs(expectedCampaignInfo));
        }

        [Test]
        public async Task GetCampaignInfo_ReturnsNullOnAnotherAdminCampaignInfo()
        {
            // Arrange
            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<bool?>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync(true);

            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<CampaignResponse>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync((CampaignResponse)null);

            // Act
            var result = await _campaignRepository.GetCampaignInfo(2, 3);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCampaignInfo_ReturnsNullWithNonExistingUserId()
        {
            // Arrange
            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<bool?>(It.IsAny<string>(), It.IsAny<object>()))
                                     .Returns(Task.FromResult<bool?>(null));

            // Act
            var result = await _campaignRepository.GetCampaignInfo(5, 1);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCampaignInfo_ReturnsNullWithNonExistingCampaignId()
        {
            // Arrange
            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<bool?>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync(true);

            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<CampaignResponse>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync((CampaignResponse)null);

            // Act
            var result = await _campaignRepository.GetCampaignInfo(1, 300);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateCampaign_ReturnsCreatedCampaign()
        {
            // Arrange
            var campaign = new CampaignForCreationDto { CampaignTitle = "%25 İndirim", IsActive = true, StartsAt = "2023-09-06", EndsAt = "2023-09-15", UserId = 3 };
            var expectedCreatedCampaign = new Campaign
            {
                CampaignId = 4,
                CampaignTitle = "%25 İndirim",
                IsActive = true,
                StartsAt = "2023-09-06",
                EndsAt = "2023-09-15",
                UserId = 3
            };

            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<int>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync(4);

            // Act
            var result = await _campaignRepository.CreateCampaign(campaign);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.CampaignId, Is.EqualTo(expectedCreatedCampaign.CampaignId));
                Assert.That(result.CampaignTitle, Is.EqualTo(expectedCreatedCampaign.CampaignTitle));
                Assert.That(result.IsActive, Is.EqualTo(expectedCreatedCampaign.IsActive));
                Assert.That(result.StartsAt, Is.EqualTo(expectedCreatedCampaign.StartsAt));
                Assert.That(result.EndsAt, Is.EqualTo(expectedCreatedCampaign.EndsAt));
                Assert.That(result.UserId, Is.EqualTo(expectedCreatedCampaign.UserId));
            });
        }

        [Test]
        public async Task UpdateCampaign_UpdatesCampaignSuccessfully()
        {
            // Arrange
            var campaign = new CampaignForUpdateDto { IsActive = false, StartsAt = "2023-09-06", EndsAt = "2023-09-15" };
            var expectedUpdatedCampaign = new Campaign
            {
                CampaignId = 1,
                CampaignTitle = "%20 İndirim",
                IsActive = false,
                StartsAt = "2023-09-06",
                EndsAt = "2023-09-15",
                UserId = 2
            };

            _contextMock.Setup(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()))
                                     .Returns(Task.CompletedTask);

            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<Campaign>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(expectedUpdatedCampaign);

            // Act
            await _campaignRepository.UpdateCampaign(1, campaign);

            // Assert
            _contextMock.Verify(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
            var updatedCampaign = await _campaignRepository.GetCampaign(1);

            Assert.Multiple(() =>
            {
                Assert.That(updatedCampaign.IsActive, Is.EqualTo(expectedUpdatedCampaign.IsActive));
                Assert.That(updatedCampaign.StartsAt, Is.EqualTo(expectedUpdatedCampaign.StartsAt));
                Assert.That(updatedCampaign.EndsAt, Is.EqualTo(expectedUpdatedCampaign.EndsAt));
            });
        }

        [Test]
        public async Task DeleteCampaign_DeletesCampaignSuccessfully()
        {
            // Arrange
            var connectionMock = new Mock<IDbConnection>();
            _contextMock.Setup(c => c.CreateConnection()).Returns(connectionMock.Object);

            var transactionMock = new Mock<IDbTransaction>();
            connectionMock.Setup(c => c.BeginTransaction()).Returns(transactionMock.Object);

            // Act
            await _campaignRepository.DeleteCampaign(1);

            // Assert
            _contextMock.Verify(c => c.ExecuteAsync("UPDATE AkakceProduct WITH (ROWLOCK) SET CampaignId = NULL WHERE CampaignId = @CampaignId", It.IsAny<object>(), transactionMock.Object), Times.Once);
            _contextMock.Verify(c => c.ExecuteAsync("DELETE FROM AkakceCampaign WHERE CampaignId = @CampaignId", It.IsAny<object>(), transactionMock.Object), Times.Once);

            transactionMock.Verify(t => t.Commit(), Times.Once);
        }

        [Test]
        public async Task DeleteUserCampaigns_DeletesUserCampaignsSuccessfully()
        {
            // Arrange
            var connectionMock = new Mock<IDbConnection>();
            _contextMock.Setup(c => c.CreateConnection()).Returns(connectionMock.Object);

            var transactionMock = new Mock<IDbTransaction>();
            connectionMock.Setup(c => c.BeginTransaction()).Returns(transactionMock.Object);

            // Act
            await _campaignRepository.DeleteUserCampaigns(2);

            // Assert
            _contextMock.Verify(c => c.ExecuteAsync("UPDATE AkakceProduct WITH (ROWLOCK) SET CampaignId = NULL WHERE UserId = @UserId", It.IsAny<object>(), transactionMock.Object), Times.Once);
            _contextMock.Verify(c => c.ExecuteAsync("DELETE FROM AkakceCampaign WHERE UserId = @UserId", It.IsAny<object>(), transactionMock.Object), Times.Once);

            transactionMock.Verify(t => t.Commit(), Times.Once);
        }
    }
}