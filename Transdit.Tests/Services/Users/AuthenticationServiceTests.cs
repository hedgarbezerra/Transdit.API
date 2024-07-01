using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Transdit.Core.Constants;
using Transdit.Core.Contracts;
using Transdit.Core.Domain;
using Transdit.Core.Models.Users;
using Transdit.Services.Users;

namespace Transdit.Tests.Services.Users
{
    [TestFixture]
    public class AuthenticationServiceTests
    {
        private MockRepository _mockRepository;
        private Fixture _fixture;

        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private Mock<IUsersService> _mockUsersService;
        private Mock<SignInManager<ApplicationUser>> _mockSignInManager;

        [OneTimeSetUp]
        public void SetUp()
        {
            _mockRepository = new MockRepository(MockBehavior.Default);
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _mockUserManager = MockUserManager();
            _mockSignInManager = MockSignInManager();
            _mockUsersService = _mockRepository.Create<IUsersService>();
        }

        [TearDown]
        public void Clean()
        {
            _mockUserManager.Reset();
            _mockUsersService.Reset();
            _mockSignInManager.Reset();
        } 

        private AuthenticationService CreateService()
        {
            return new AuthenticationService(
                _mockUserManager.Object,
                new JwtSettings() { Issuer = "issuer", ExpirationInMinutes = "30", Key = "(G+KaPdSgVkYp3s6v9y$B&E)H@McQeTh" },
                _mockUsersService.Object,
                _mockSignInManager.Object);
        }
        private Mock<UserManager<ApplicationUser>> MockUserManager()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            var user = _fixture.Build<ApplicationUser>()
                .With(p => p.Email, "email@email.com").CreateMany(5).AsQueryable();
            mgr.Object.UserValidators.Add(new UserValidator<ApplicationUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<ApplicationUser>());

            return mgr;
        }
        private Mock<SignInManager<ApplicationUser>> MockSignInManager()
        {
            var mqUserManager = _mockUserManager;
            var mqHttpContext = _mockRepository.Create<IHttpContextAccessor>();
            var mqClaimFactory = _mockRepository.Create<IUserClaimsPrincipalFactory<ApplicationUser>>();
            var signManager = new Mock<SignInManager<ApplicationUser>>(mqUserManager.Object, mqHttpContext.Object, mqClaimFactory.Object, null, null, null);
            var user = _fixture.Build<ApplicationUser>()
                .With(p => p.Email, "email@email.com").CreateMany(5).AsQueryable();

            return signManager;
        }


        [Test]
        [TestCase("")]
        [TestCase("1212")]
        [TestCase("xxxxx")]
        [TestCase("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYmYiOjE2ODA5MTA3NTgsImV4cCI6MTY4MDkxMjU1OCwiaWF0IjoxNjgwOTEwNzU4LCJpc3MiOiJub2lzc3VlciJ9.mxS0IcX2h3lgyeyBPoC0yOTNrK6GW991eutXhT3jcpk")]
        public void ValidateToken_InvalidToken_ResultShouldBeFalse(string token)
        {
            // Arrange
            var service = CreateService();
            // Act
            var result = service.ValidateToken(token);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public async Task ValidateToken_ValidToken()
        {
            // Arrange
            var service = CreateService();
            _mockUsersService.Setup(mq => mq.GetClaims(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.CreateMany<Claim>(0)));

            _mockUsersService.Setup(mq => mq.Get(It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.Create<ApplicationUser>()));

            _mockSignInManager
                .Setup(mq => mq.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(SignInResult.Success));

            Login login = new Login() { Email = "", Password = "" };
            var tokenResult = await service.Authenticate(login);
            // Act
            var result = service.ValidateToken(tokenResult.Data.Token);

            // Assert
            result.Should().BeTrue();
        }

        [Test]
        public async Task Authenticate_AuthenticatedUser_TokenCreated()
        {
            // Arrange
            var service = CreateService();
            
            _mockUsersService.Setup(mq => mq.GetClaims(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.CreateMany<Claim>(0)));

            _mockUsersService.Setup(mq => mq.Get(It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.Create<ApplicationUser>()));

            _mockSignInManager
                .Setup(mq => mq.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(SignInResult.Success));

            Login login = new Login() { Email = "", Password = "" };

            // Act
            var result = await service.Authenticate(login);

            // Assert

            result.Should().NotBeNull();
            result.Successful.Should().BeTrue();
            result.Messages.Should().BeEmpty();
            result.Data.Should().NotBeNull();
            result.Data.Token.Should().NotBeNull();
            result.Data.IsValid.Should().BeTrue();   

            _mockUsersService.Verify(mq => mq.Get(It.IsAny<string>()), Times.Once());
            _mockSignInManager.Verify(mq => mq.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
        }

        [Test]
        public async Task Authenticate_NoUserFound_TokenNotCreated()
        {
            // Arrange
            var service = CreateService();

            _mockUsersService.Setup(mq => mq.GetClaims(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.CreateMany<Claim>(0)));

            _mockSignInManager
                .Setup(mq => mq.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(SignInResult.Success));

            Login login = new Login() { Email = "", Password = "" };

            // Act
            var result = await service.Authenticate(login);

            // Assert

            result.Should().NotBeNull();
            result.Successful.Should().BeFalse();
            result.Messages.Should().NotBeEmpty().And.HaveCount(1);
            result.Data.Should().NotBeNull();
            _mockUsersService.Verify(mq => mq.Get(It.IsAny<string>()), Times.Once());
            _mockSignInManager.Verify(mq => mq.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
            _mockUserManager.Verify(mq => mq.GetAccessFailedCountAsync(It.IsAny<ApplicationUser>()), Times.Never);
        }

        [Test]
        public async Task Authenticate_UserNotAuthenticated_TokenNotCreated()
        {
            // Arrange
            var service = CreateService();

            _mockUsersService.Setup(mq => mq.GetClaims(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.CreateMany<Claim>(0)));

            _mockUsersService.Setup(mq => mq.Get(It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.Create<ApplicationUser>()));

            _mockSignInManager
                .Setup(mq => mq.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(SignInResult.Failed));

            Login login = new Login() { Email = "", Password = "" };

            // Act
            var result = await service.Authenticate(login);

            // Assert

            result.Should().NotBeNull();
            result.Successful.Should().BeFalse();
            result.Messages.Should().NotBeEmpty().And.HaveCount(1);
            result.Data.Should().NotBeNull();
            _mockUsersService.Verify(mq => mq.Get(It.IsAny<string>()), Times.Once());
            _mockSignInManager.Verify(mq => mq.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
            _mockUserManager.Verify(mq => mq.GetAccessFailedCountAsync(It.IsAny<ApplicationUser>()), Times.Once);
        }

        [Test]
        public async Task Authenticate_UserLockedOut_TokenNotCreated()
        {
            // Arrange
            var service = CreateService();

            _mockUsersService.Setup(mq => mq.GetClaims(It.IsAny<ApplicationUser>(), It.IsAny<bool>(), It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.CreateMany<Claim>(0)));

            _mockUsersService.Setup(mq => mq.Get(It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.Create<ApplicationUser>()));

            _mockSignInManager
                .Setup(mq => mq.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(SignInResult.LockedOut));

            Login login = new Login() { Email = "", Password = "" };

            // Act
            var result = await service.Authenticate(login);

            // Assert

            result.Should().NotBeNull();
            result.Successful.Should().BeFalse();
            result.Messages.Should().NotBeEmpty().And.HaveCount(1).And.Subject.FirstOrDefault().Should().Contain("Conta bloqueada");
            result.Data.Should().NotBeNull();
            _mockUsersService.Verify(mq => mq.Get(It.IsAny<string>()), Times.Once());
            _mockSignInManager.Verify(mq => mq.PasswordSignInAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once);
            _mockUserManager.Verify(mq => mq.GetAccessFailedCountAsync(It.IsAny<ApplicationUser>()), Times.Once);
        }
    }
}
