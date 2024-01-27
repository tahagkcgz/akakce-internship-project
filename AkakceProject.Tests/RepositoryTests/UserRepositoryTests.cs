using AkakceProject.Core.Dto;
using AkakceProject.Core.Entities;
using AkakceProject.Core.Responses;
using AkakceProject.Infrastructure.Contracts;
using AkakceProject.Infrastructure.Repositories;
using Dapper;
using Moq;
using NUnit.Framework;
using System.Data;

namespace AkakceProject.Tests.RepositoryTests
{
    [TestFixture]
    public class UserRepositoryTests
    {
        private Mock<IDapperContext> _contextMock;
        private UserRepository _userRepository;

        public List<User> dbUsers;
        public List<Campaign> dbCampaigns;
        public List<Product> dbProducts;

        [SetUp]
        public void Setup()
        {
            _contextMock = new Mock<IDapperContext>();
            _userRepository = new UserRepository(_contextMock.Object);

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
        public async Task GetUsers_ReturnsListOfUsers()
        {
            // Arrange
            _contextMock.Setup(c => c.QueryAsync<User>(It.IsAny<string>(), null))
                                     .ReturnsAsync(dbUsers);

            // Act
            var result = await _userRepository.GetUsers();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<IEnumerable<User>>());
            Assert.That(dbUsers, Has.Count.EqualTo(result.Count()));
        }

        [Test]
        public async Task GetUser_ReturnsUser()
        {
            // Arrange
            var expectedUser = dbUsers[0];

            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<User>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync(expectedUser);

            // Act
            var result = await _userRepository.GetUser(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.InstanceOf<User>());
            Assert.That(result, Is.SameAs(expectedUser));
        }

        [Test]
        public async Task GetUser_ReturnsNullWithNonExistingUserId()
        {
            // Arrange
            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<User>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync((User)null);

            // Act
            var result = await _userRepository.GetUser(5);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task IsUserAdmin_ReturnsTrueWithAdminUserId()
        {
            // Arrange
            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<bool?>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync(true);

            // Act
            var result = await _userRepository.IsUserAdmin(2);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task IsUserAdmin_ReturnsFalseWithNormalUserId()
        {
            // Arrange
            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<bool?>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync(false);

            // Act
            var result = await _userRepository.IsUserAdmin(1);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task IsUserAdmin_ReturnsNullWithNonExistingUserId()
        {
            // Arrange
            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<bool?>(It.IsAny<string>(), It.IsAny<object>()))
                                     .Returns(Task.FromResult<bool?>(null));

            // Act
            var result = await _userRepository.IsUserAdmin(5);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetUserInfo_ReturnsNormalUserInfo()
        {
            // Arrange
            var expectedUserInfo = new UserResponse
            {
                UserName = "asli",
                EmailAddress = "asli@gmail.com",
                PhoneNumber = "",
                UserCampaigns = new List<CampaignResponse>
                {
                    new CampaignResponse { AuthUserName = "Samsung", CampaignTitle = "%20 �ndirim", IsActive = true , StartsAt = "2023-09-06", EndsAt = "2023-09-15" },
                    new CampaignResponse { AuthUserName = "Samsung", CampaignTitle = "Sigorta Hediye", IsActive = true, StartsAt = "2023-09-06", EndsAt = "2023-09-15" },
                    new CampaignResponse { AuthUserName = "Apple", CampaignTitle = "%10 �ndirim", IsActive = false, StartsAt = "2023-09-08", EndsAt = "2023-09-20" }
                },
                UserProducts = new List<ProductResponse>
                {
                    new ProductResponse { BrandName = "Samsung", CampaignName = "%20 �ndirim", ProductName = "Galaxy Z Flip 4 256 GB", ProductDescription = "G�zel telefon", ProductPrice = 40000 },
                    new ProductResponse { BrandName = "Samsung", CampaignName = "Sigorta Hediye", ProductName = "Galaxy Z Flip 3 128 GB", ProductDescription = "Sa�lam telefon", ProductPrice = 30000 },
                    new ProductResponse { BrandName = "Apple", CampaignName = "%10 �ndirim", ProductName = "Iphone 12 128 GB", ProductDescription = "�ok iyi telefon", ProductPrice = 42000 },
                    new ProductResponse { BrandName = "Samsung", CampaignName = null, ProductName = "Galaxy A3 64 GB", ProductDescription = "�darelik telefon", ProductPrice = 6000 },
                }
            };

            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<bool?>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync(false);

            _contextMock.Setup(c => c.QueryAsync<UserResponse, CampaignResponse, ProductResponse, UserResponse>
                                (It.IsAny<string>(),
                                 It.IsAny<Func<UserResponse, CampaignResponse, ProductResponse, UserResponse>>(),
                                 It.IsAny<object>(),
                                 It.IsAny<string>()))
                                     .ReturnsAsync(new List<UserResponse> { expectedUserInfo });

            // Act
            var result = await _userRepository.GetUserInfo(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.SameAs(expectedUserInfo));
        }

        [Test]
        public async Task GetUserInfo_ReturnsAdminUserInfo()
        {
            // Arrange
            var expectedUserInfo = new UserResponse
            {
                UserName = "Samsung",
                EmailAddress = "info@samsung.com",
                PhoneNumber = "08502256060",
                UserCampaigns = new List<CampaignResponse>
                {
                    new CampaignResponse { AuthUserName = "Samsung", CampaignTitle = "%20 �ndirim", IsActive = true , StartsAt = "2023-09-06", EndsAt = "2023-09-15" },
                    new CampaignResponse { AuthUserName = "Samsung", CampaignTitle = "Sigorta Hediye", IsActive = true, StartsAt = "2023-09-06", EndsAt = "2023-09-15" }
                },
                UserProducts = new List<ProductResponse>
                {
                    new ProductResponse { BrandName = "Samsung", CampaignName = "%20 �ndirim", ProductName = "Galaxy Z Flip 4 256 GB", ProductDescription = "G�zel telefon", ProductPrice = 40000 },
                    new ProductResponse { BrandName = "Samsung", CampaignName = "Sigorta Hediye", ProductName = "Galaxy Z Flip 3 128 GB", ProductDescription = "Sa�lam telefon", ProductPrice = 30000 },
                    new ProductResponse { BrandName = "Samsung", CampaignName = null, ProductName = "Galaxy A3 64 GB", ProductDescription = "�darelik telefon", ProductPrice = 6000 }
                }
            };

            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<bool?>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync(true);

            _contextMock.Setup(c => c.QueryAsync<UserResponse, CampaignResponse, ProductResponse, UserResponse>
                                (It.IsAny<string>(),
                                 It.IsAny<Func<UserResponse, CampaignResponse, ProductResponse, UserResponse>>(),
                                 It.IsAny<object>(),
                                 It.IsAny<string>()))
                                     .ReturnsAsync(new List<UserResponse> { expectedUserInfo });

            // Act
            var result = await _userRepository.GetUserInfo(2);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.SameAs(expectedUserInfo));
        }

        [Test]
        public async Task GetUserInfo_ReturnsNullWithNonExistingUserId()
        {
            // Arrange
            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<bool?>(It.IsAny<string>(), It.IsAny<object>()))
                                     .Returns(Task.FromResult<bool?>(null));

            // Act
            var result = await _userRepository.GetUserInfo(5);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateUser_ReturnsCreatedUser()
        {
            // Arrange
            var user = new UserForCreationDto { UserName = "taha", UserPassword = "usertaha", EmailAddress = "", PhoneNumber = "", IsAdmin = false };
            var expectedCreatedUser = new User { UserId = 4, UserName = "taha", UserPassword = "usertaha", EmailAddress = "", PhoneNumber = "", IsAdmin = false, IsUserActive = true };

            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<int>(It.IsAny<string>(), It.IsAny<object>()))
                                     .ReturnsAsync(4);

            // Act
            var result = await _userRepository.CreateUser(user);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.UserId, Is.EqualTo(expectedCreatedUser.UserId));
                Assert.That(result.UserName, Is.EqualTo(expectedCreatedUser.UserName));
                Assert.That(result.UserPassword, Is.EqualTo(expectedCreatedUser.UserPassword));
                Assert.That(result.EmailAddress, Is.EqualTo(expectedCreatedUser.EmailAddress));
                Assert.That(result.PhoneNumber, Is.EqualTo(expectedCreatedUser.PhoneNumber));
                Assert.That(result.IsAdmin, Is.EqualTo(expectedCreatedUser.IsAdmin));
                Assert.That(result.IsUserActive, Is.EqualTo(expectedCreatedUser.IsUserActive));
            });
        }

        [Test]
        public async Task UpdateUser_UpdatesUserSuccessfully()
        {
            // Arrange
            var user = new UserForUpdateDto { UserName = "asli", UserPassword = "yeni.sifre", EmailAddress = "asli@gmail.com", PhoneNumber = "" };
            var expectedUpdatedUser = new User { UserId = 1, UserName = "asli", UserPassword = "yeni.sifre", EmailAddress = "asli@gmail.com", PhoneNumber = "", IsAdmin = false, IsUserActive = true };

            _contextMock.Setup(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()))
                                     .Returns(Task.CompletedTask);

            _contextMock.Setup(c => c.QuerySingleOrDefaultAsync<User>(It.IsAny<string>(), It.IsAny<object>()))
                .ReturnsAsync(expectedUpdatedUser);

            // Act
            await _userRepository.UpdateUser(1, user);

            // Assert
            _contextMock.Verify(c => c.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
            var updatedUser = await _userRepository.GetUser(1);

            Assert.Multiple(() =>
            {
                Assert.That(updatedUser.UserName, Is.EqualTo(expectedUpdatedUser.UserName));
                Assert.That(updatedUser.UserPassword, Is.EqualTo(expectedUpdatedUser.UserPassword));
                Assert.That(updatedUser.EmailAddress, Is.EqualTo(expectedUpdatedUser.EmailAddress));
                Assert.That(updatedUser.PhoneNumber, Is.EqualTo(expectedUpdatedUser.PhoneNumber));
            });
        }

        [Test]
        public async Task DeactivateUser_DeactivatesUserSuccessfully()
        {
            // Arrange
            var connectionMock = new Mock<IDbConnection>();
            _contextMock.Setup(c => c.CreateConnection()).Returns(connectionMock.Object);

            var transactionMock = new Mock<IDbTransaction>();
            connectionMock.Setup(c => c.BeginTransaction()).Returns(transactionMock.Object);

            // Act
            await _userRepository.DeactivateUser(1);

            // Assert
            _contextMock.Verify(c => c.ExecuteAsync("DELETE FROM AkakceProduct WHERE UserId = @UserId", It.IsAny<object>(), transactionMock.Object), Times.Once);
            _contextMock.Verify(c => c.ExecuteAsync("DELETE FROM AkakceCampaign WHERE UserId = @UserId", It.IsAny<object>(), transactionMock.Object), Times.Once);
            _contextMock.Verify(c => c.ExecuteAsync("UPDATE AkakceUser WITH (ROWLOCK) SET IsUserActive = 0 WHERE UserId = @UserId", It.IsAny<object>(), transactionMock.Object), Times.Once);

            transactionMock.Verify(t => t.Commit(), Times.Once);
        }
    }
}