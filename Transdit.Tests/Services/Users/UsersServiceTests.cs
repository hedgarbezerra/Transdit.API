using AutoFixture;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Contracts;
using Transdit.Core.Domain;
using Transdit.Core.Models.Notification;
using Transdit.Core.Models.Pagination;
using Transdit.Core.Models.Users;
using Transdit.Repository.Repositories;
using Transdit.Services.Emails;
using Transdit.Services.Users;

namespace Transdit.Tests.Services.Users
{
    [TestFixture]
    public class UsersServiceTests
    {
        private MockRepository _mqRepo;
        private Fixture _fixture;

        [OneTimeSetUp]
        public void Setup()
        {
            _mqRepo = new MockRepository(MockBehavior.Default);
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Test]
        public async Task Get_FindByEmail_UserExists()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository, out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);
            var expectedUser = _fixture.Create<ApplicationUser>();
            mqUserManager.Setup(mq => mq.FindByEmailAsync(expectedUser.Email))
                .Returns(Task.FromResult(expectedUser));

            var user = await userService.Get(expectedUser.Email);

            user.Should().NotBeNull().And.Be(expectedUser);
        }
        [Test]
        public async Task Get_FindByWrongEmail_UserNotExists()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository, out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);
            var expectedUser = _fixture.Create<ApplicationUser>();
            mqUserManager.Setup(mq => mq.FindByEmailAsync(expectedUser.Email))
                .Returns(Task.FromResult(expectedUser));

            var user = await userService.Get("random@email.com");

            user.Should().BeNull();
        }

        [Test]
        public async Task Get_FindByEmailAndUsername_UserNotFound()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository, out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);

            var user = await userService.Get("email@cotoso.com");

            user.Should().BeNull();
        }

        [Test]
        public async Task Get_FindByUsername_UserExists()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository, out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);
            var expectedUser = _fixture.Create<ApplicationUser>();
            mqUserManager.Setup(mq => mq.FindByNameAsync(expectedUser.UserName))
                .Returns(Task.FromResult(expectedUser));

            var user = await userService.Get(expectedUser.UserName);

            user.Should().NotBeNull().And.Be(expectedUser);
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(23)]
        public async Task Get_FindbyId_UserNotFound(int id)
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository, out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);
            var expectedUser = _fixture.Create<ApplicationUser>();
            mqUserManager.Setup(mq => mq.FindByIdAsync("3"))
                .Returns(Task.FromResult(expectedUser));

            var user = await userService.Get(id);

            user.Should().BeNull();
        }

        [Test]
        public async Task Get_FindbyId_UserFound()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository, out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);
            var expectedUser = _fixture.Create<ApplicationUser>();
            mqUserManager.Setup(mq => mq.FindByIdAsync(expectedUser.Id.ToString()))
                .Returns(Task.FromResult(expectedUser));

            var user = await userService.Get(expectedUser.Id);

            user.Should().NotBeNull().And.Be(expectedUser);
        }

        [Test]
        [TestCase(1, 1, 5)]
        [TestCase(2, 2, 5)]
        [TestCase(3, 1, 5)]
        public async Task Get_FilterPaginationNoFilter(int index, int pageSize, int expectedCount)
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository, out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);
            var users = await userService.Get(new PaginationInput(index, pageSize), string.Empty);

            users.Should().NotBeNull();
            users.Data.Should().NotBeNullOrEmpty();
            users.TotalCount.Should().Be(expectedCount);
            users.HasNextPage.Should().BeTrue();
        }

        [Test]
        [TestCase(1, 1, "usernameid", 0, 0)]
        [TestCase(2, 2, "notrealemail", 0, 0)]
        [TestCase(1, 1, "email@email.com", 5, 1)]
        public async Task Get_FilterPaginationWithSearchTerm(int index, int pageSize, string searchTerm, int expectedCount, int expectedDataCount)
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository, out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);
            var users = await userService.Get(new PaginationInput(index, pageSize, searchTerm), string.Empty);

            users.Should().NotBeNull();
            users.Data.Should().HaveCount(expectedDataCount);
            users.TotalCount.Should().Be(expectedCount);
        }

        [Test]
        public async Task Create_ValidationFailed()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository, out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);
            var validationResult = _fixture.Build<ValidationResult>()
                .With(p => p.Errors, _fixture.CreateMany<ValidationFailure>(1).ToList()).Create();
            mqSignUpValidator.Setup(mq => mq.Validate(It.IsAny<UserSignUp>())).Returns(validationResult);

            var identityResult = IdentityResult.Failed(_fixture.CreateMany<IdentityError>(0).ToArray());
            mqUserManager.Setup(mq => mq.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(Task.FromResult(identityResult));

            var servicePlan = _fixture.Build<ServicePlan>().Create();
            mqRoleManager.Setup(mq => mq.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(servicePlan));

            var claimsResult = IdentityResult.Failed(_fixture.CreateMany<IdentityError>(0).ToArray());
            mqUserManager.Setup(mq => mq.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<Claim>>()))
                .Returns(Task.FromResult(claimsResult));
            mqUserManager.Setup(mq => mq.GetRolesAsync(It.IsAny<ApplicationUser>())).Returns(Task.FromResult<IList<string>>(_fixture.CreateMany<string>(1).ToList()));
            var signUpUser = _fixture.Create<UserSignUp>();
            var result = await userService.Create(signUpUser);

            result.Should().NotBeNull();
            result.Successful.Should().BeFalse();
            result.Data.Should().NotBeNull();
            result.Messages.Should().HaveCount(1);
            mqSignUpValidator.Verify(mq => mq.Validate(It.IsAny<UserSignUp>()), Times.Once);
        }

        [Test]
        public async Task Create_CreateIdentityFailed()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository, out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);
            var validationResult = _fixture.Build<ValidationResult>()
                .With(p => p.Errors, _fixture.CreateMany<ValidationFailure>(0).ToList()).Create();
            mqSignUpValidator.Setup(mq => mq.Validate(It.IsAny<UserSignUp>())).Returns(validationResult);

            var identityResult = IdentityResult.Success;
            mqUserManager.Setup(mq => mq.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(Task.FromResult(identityResult));
           
            var servicePlan = _fixture.Build<ServicePlan>().Create();
            mqRoleManager.Setup(mq => mq.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(servicePlan));
            
            var claimsResult = IdentityResult.Failed(_fixture.CreateMany<IdentityError>(0).ToArray());
            mqUserManager.Setup(mq => mq.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<Claim>>()))
                .Returns(Task.FromResult(claimsResult));
            mqUserManager.Setup(mq => mq.GetRolesAsync(It.IsAny<ApplicationUser>())).Returns(Task.FromResult<IList<string>>(_fixture.CreateMany<string>(1).ToList()));
            var signUpUser = _fixture.Create<UserSignUp>();
            var result = await userService.Create(signUpUser);

            result.Should().NotBeNull();
            result.Successful.Should().BeFalse();
            result.Data.Should().NotBeNull();
            result.Messages.Should().HaveCount(0);
            mqSignUpValidator.Verify(mq => mq.Validate(It.IsAny<UserSignUp>()), Times.Once);
            mqUserManager.Verify(mq => mq.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Create_UserCreated_RoleNotFound()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository, out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);
            var validationResult = _fixture.Build<ValidationResult>()
                .With(p => p.Errors, _fixture.CreateMany<ValidationFailure>(0).ToList()).Create();
            mqSignUpValidator.Setup(mq => mq.Validate(It.IsAny<UserSignUp>())).Returns(validationResult);

            var identityResult = IdentityResult.Success;
            mqUserManager.Setup(mq => mq.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(Task.FromResult(identityResult));

            var claimsResult = IdentityResult.Failed(_fixture.CreateMany<IdentityError>(0).ToArray());
            mqUserManager.Setup(mq => mq.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<Claim>>()))
                .Returns(Task.FromResult(claimsResult));
            mqUserManager.Setup(mq => mq.GetRolesAsync(It.IsAny<ApplicationUser>())).Returns(Task.FromResult<IList<string>>(_fixture.CreateMany<string>(1).ToList()));
            var signUpUser = _fixture.Create<UserSignUp>();
            var result = await userService.Create(signUpUser);

            result.Should().NotBeNull();
            result.Successful.Should().BeFalse();
            result.Data.Should().NotBeNull();
            result.Messages.Should().HaveCount(1);
            mqSignUpValidator.Verify(mq => mq.Validate(It.IsAny<UserSignUp>()), Times.Once);
            mqUserManager.Verify(mq => mq.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            mqRoleManager.Verify(mq => mq.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mqUserPlanRepository.Verify(mq => mq.Add(It.IsAny<ApplicationUserPlan>()), Times.Never);
        }

        [Test]
        public async Task Create_UserCreated_UserAddedToPlanClaimsFailed()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository, out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);
            var validationResult = _fixture.Build<ValidationResult>()
                .With(p => p.Errors, _fixture.CreateMany<ValidationFailure>(0).ToList()).Create();
            mqSignUpValidator.Setup(mq => mq.Validate(It.IsAny<UserSignUp>())).Returns(validationResult);

            var identityResult = IdentityResult.Success;
            mqUserManager.Setup(mq => mq.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(Task.FromResult(identityResult));

            var servicePlan = _fixture.Build<ServicePlan>().Create();
            mqRoleManager.Setup(mq => mq.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(servicePlan));

            var claimsResult = IdentityResult.Failed(_fixture.CreateMany<IdentityError>(3).ToArray());
            mqUserManager.Setup(mq => mq.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<Claim>>()))
                .Returns(Task.FromResult(claimsResult));
            mqUserManager.Setup(mq => mq.GetRolesAsync(It.IsAny<ApplicationUser>())).Returns(Task.FromResult<IList<string>>(_fixture.CreateMany<string>(1).ToList()));
            var userPlansGet = _fixture.Build<ApplicationUserPlan>()
                .With(p => p.IsActive, true)
                .With(p => p.UserId, 0)
                .With(p => p.Maturity, DateTime.Now.AddDays(1))
                .CreateMany(2).AsQueryable();
            mqUserPlanRepository.Setup(mq => mq.Get())
                .Returns(userPlansGet);
            
            var signUpUser = _fixture.Create<UserSignUp>();            
            var result = await userService.Create(signUpUser);

            result.Should().NotBeNull();
            result.Successful.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Messages.Should().HaveCount(3);
            mqSignUpValidator.Verify(mq => mq.Validate(It.IsAny<UserSignUp>()), Times.Once);
            mqUserManager.Verify(mq => mq.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            mqRoleManager.Verify(mq => mq.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mqUserPlanRepository.Verify(mq => mq.Add(It.IsAny<ApplicationUserPlan>()), Times.Once);
        }

        [Test]
        public async Task Create_UserCreated_UserAddedToPlanClaimsSucceed()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository, out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);
            var validationResult = _fixture.Build<ValidationResult>()
                .With(p => p.Errors, _fixture.CreateMany<ValidationFailure>(0).ToList()).Create();
            mqSignUpValidator.Setup(mq => mq.Validate(It.IsAny<UserSignUp>())).Returns(validationResult);

            var identityResult = IdentityResult.Success;
            mqUserManager.Setup(mq => mq.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(Task.FromResult(identityResult));

            var servicePlan = _fixture.Build<ServicePlan>().Create();
            mqRoleManager.Setup(mq => mq.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(servicePlan));

            var claimsResult = IdentityResult.Success;
            mqUserManager.Setup(mq => mq.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<Claim>>()))
                .Returns(Task.FromResult(claimsResult));
            mqUserManager.Setup(mq => mq.GetRolesAsync(It.IsAny<ApplicationUser>())).Returns(Task.FromResult<IList<string>>(_fixture.CreateMany<string>(1).ToList()));
            var userPlansGet = _fixture.Build<ApplicationUserPlan>()
                .With(p => p.IsActive, true)
                .With(p => p.UserId, 0)
                .With(p => p.Maturity, DateTime.Now.AddDays(1))
                .CreateMany(2).AsQueryable();
            mqUserPlanRepository.Setup(mq => mq.Get())
                .Returns(userPlansGet);
            
            var signUpUser = _fixture.Create<UserSignUp>();
            var result = await userService.Create(signUpUser);

            result.Should().NotBeNull();
            result.Successful.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Messages.Should().HaveCount(0);
            mqSignUpValidator.Verify(mq => mq.Validate(It.IsAny<UserSignUp>()), Times.Once);
            mqUserManager.Verify(mq => mq.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            mqRoleManager.Verify(mq => mq.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mqUserPlanRepository.Verify(mq => mq.Add(It.IsAny<ApplicationUserPlan>()), Times.Once);
        }

        [Test]
        public async Task Update_EntityValidationFailed()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository, out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);
            var validationResult = _fixture.Build<ValidationResult>()
                .With(p => p.Errors, _fixture.CreateMany<ValidationFailure>(3).ToList()).Create();
            mqSignUpValidator.Setup(mq => mq.Validate(It.IsAny<UserSignUp>())).Returns(validationResult);

            var identityResult = IdentityResult.Success;
            mqUserManager.Setup(mq => mq.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(Task.FromResult(identityResult));

            var servicePlan = _fixture.Build<ServicePlan>().Create();
            mqRoleManager.Setup(mq => mq.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(servicePlan));

            var claimsResult = IdentityResult.Failed(_fixture.CreateMany<IdentityError>(0).ToArray());
            mqUserManager.Setup(mq => mq.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<Claim>>()))
                .Returns(Task.FromResult(claimsResult));
            mqUserManager.Setup(mq => mq.GetRolesAsync(It.IsAny<ApplicationUser>())).Returns(Task.FromResult<IList<string>>(_fixture.CreateMany<string>(1).ToList()));

            var signUpUser = _fixture.Create<UserSignUp>();
            var result = await userService.Update(signUpUser, string.Empty);

            result.Should().NotBeNull();
            result.Successful.Should().BeFalse();
            result.Data.Should().NotBeNull();
            result.Messages.Should().HaveCount(3);
            mqSignUpValidator.Verify(mq => mq.Validate(It.IsAny<UserSignUp>()), Times.Once);
            mqUserManager.Verify(mq => mq.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task Update_UserNotFound()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository, out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);
            var validationResult = _fixture.Build<ValidationResult>()
                .With(p => p.Errors, _fixture.CreateMany<ValidationFailure>(0).ToList()).Create();
            mqSignUpValidator.Setup(mq => mq.Validate(It.IsAny<UserSignUp>())).Returns(validationResult);

            var identityResult = IdentityResult.Success;
            mqUserManager.Setup(mq => mq.UpdateAsync(It.IsAny<ApplicationUser>())).Returns(Task.FromResult(identityResult));

            var servicePlan = _fixture.Build<ServicePlan>().Create();
            mqRoleManager.Setup(mq => mq.FindByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(servicePlan));

            var claimsResult = IdentityResult.Failed(_fixture.CreateMany<IdentityError>(0).ToArray());
            mqUserManager.Setup(mq => mq.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<Claim>>()))
                .Returns(Task.FromResult(claimsResult));
            mqUserManager.Setup(mq => mq.GetRolesAsync(It.IsAny<ApplicationUser>())).Returns(Task.FromResult<IList<string>>(_fixture.CreateMany<string>(1).ToList()));

            var signUpUser = _fixture.Create<UserSignUp>();
            var result = await userService.Update(signUpUser, string.Empty);

            result.Should().NotBeNull();
            result.Successful.Should().BeFalse();
            result.Data.Should().NotBeNull();
            result.Messages.Should().HaveCount(1);
            mqSignUpValidator.Verify(mq => mq.Validate(It.IsAny<UserSignUp>()), Times.Once);
            mqUserManager.Verify(mq => mq.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            mqUserManager.Verify(mq => mq.FindByNameAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Update_UpdateFailed()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository, out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);
            var validationResult = _fixture.Build<ValidationResult>()
                .With(p => p.Errors, _fixture.CreateMany<ValidationFailure>(0).ToList()).Create();
            mqSignUpValidator.Setup(mq => mq.Validate(It.IsAny<UserSignUp>())).Returns(validationResult);

            var identityResult = IdentityResult.Failed(_fixture.CreateMany<IdentityError>(2).ToArray());
            mqUserManager.Setup(mq => mq.UpdateAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult(identityResult));

            var servicePlan = _fixture.Build<ServicePlan>().Create();
            mqRoleManager.Setup(mq => mq.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(servicePlan));

            var claimsResult = IdentityResult.Failed(_fixture.CreateMany<IdentityError>(0).ToArray());
            mqUserManager.Setup(mq => mq.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<Claim>>()))
                .Returns(Task.FromResult(claimsResult));
            mqUserManager.Setup(mq => mq.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.Create<ApplicationUser>()));

            var signUpUser = _fixture.Create<UserSignUp>();
            var result = await userService.Update(signUpUser, string.Empty);

            result.Should().NotBeNull();
            result.Successful.Should().BeFalse();
            result.Data.Should().NotBeNull();
            result.Messages.Should().HaveCount(2);
            mqSignUpValidator.Verify(mq => mq.Validate(It.IsAny<UserSignUp>()), Times.Once);
            mqUserManager.Verify(mq => mq.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            mqUserManager.Verify(mq => mq.FindByNameAsync(It.IsAny<string>()), Times.Never);
            mqUserManager.Verify(mq => mq.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Once);

        }

        [Test]
        public async Task Update_FailedToAddToRole()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository, out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);
            var validationResult = _fixture.Build<ValidationResult>()
                .With(p => p.Errors, _fixture.CreateMany<ValidationFailure>(0).ToList()).Create();
            mqSignUpValidator.Setup(mq => mq.Validate(It.IsAny<UserSignUp>())).Returns(validationResult);

            var identityResult = IdentityResult.Success;
            mqUserManager.Setup(mq => mq.UpdateAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult(identityResult));

            var servicePlan = _fixture.Build<ServicePlan>().Create();
            mqRoleManager.Setup(mq => mq.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(servicePlan));

            var claimsResult = IdentityResult.Failed(_fixture.CreateMany<IdentityError>(0).ToArray());
            mqUserManager.Setup(mq => mq.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<Claim>>()))
                .Returns(Task.FromResult(claimsResult));
            var foundUser = _fixture.Create<ApplicationUser>();
            mqUserManager.Setup(mq => mq.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(foundUser)); 
            
            var removeClaimsResult = IdentityResult.Failed(_fixture.CreateMany<IdentityError>(1).ToArray());
             mqUserManager.Setup(mq => mq.RemoveClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IList<Claim>>()))
                .Returns(Task.FromResult(removeClaimsResult));


            var signUpUser = _fixture.Build<UserSignUp>()
                .With(p => p.PlanId, servicePlan.Id).Create();
            var result = await userService.Update(signUpUser, string.Empty);

            result.Should().NotBeNull();
            result.Successful.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Messages.Should().HaveCount(1);
            mqSignUpValidator.Verify(mq => mq.Validate(It.IsAny<UserSignUp>()), Times.Once);
            mqUserManager.Verify(mq => mq.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            mqUserManager.Verify(mq => mq.FindByNameAsync(It.IsAny<string>()), Times.Never);
            mqUserManager.Verify(mq => mq.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Once);
            mqRoleManager.Verify(mq => mq.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mqUserManager.Verify(mq => mq.GetClaimsAsync(It.IsAny<ApplicationUser>()), Times.Exactly(2));
        }

        [Test]
        public async Task Update_SucceedToAddToRole()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository, out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);
            var validationResult = _fixture.Build<ValidationResult>()
                .With(p => p.Errors, _fixture.CreateMany<ValidationFailure>(0).ToList()).Create();
            mqSignUpValidator.Setup(mq => mq.Validate(It.IsAny<UserSignUp>())).Returns(validationResult);

            var identityResult = IdentityResult.Success;
            mqUserManager.Setup(mq => mq.UpdateAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult(identityResult));

            var servicePlan = _fixture.Build<ServicePlan>().Create();
            mqRoleManager.Setup(mq => mq.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(servicePlan));

            var claimsResult = IdentityResult.Failed(_fixture.CreateMany<IdentityError>(0).ToArray());
            mqUserManager.Setup(mq => mq.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<Claim>>()))
                .Returns(Task.FromResult(claimsResult));
            var foundUser = _fixture.Create<ApplicationUser>();
            mqUserManager.Setup(mq => mq.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(foundUser));

            var removeClaimsResult = IdentityResult.Failed(_fixture.CreateMany<IdentityError>(1).ToArray());
            mqUserManager.Setup(mq => mq.RemoveClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IList<Claim>>()))
               .Returns(Task.FromResult(removeClaimsResult));


            var signUpUser = _fixture.Build<UserSignUp>()
                .With(p => p.PlanId, servicePlan.Id).Create();
            var result = await userService.Update(signUpUser, string.Empty);

            result.Should().NotBeNull();
            result.Successful.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Messages.Should().HaveCount(1);
            mqSignUpValidator.Verify(mq => mq.Validate(It.IsAny<UserSignUp>()), Times.Once);
            mqUserManager.Verify(mq => mq.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            mqUserManager.Verify(mq => mq.FindByNameAsync(It.IsAny<string>()), Times.Never);
            mqUserManager.Verify(mq => mq.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Once);
            mqRoleManager.Verify(mq => mq.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mqUserManager.Verify(mq => mq.GetClaimsAsync(It.IsAny<ApplicationUser>()), Times.Exactly(2));
        }

        [Test]
        public async Task Update_AllSuccess()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository, out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);
            var validationResult = _fixture.Build<ValidationResult>()
                .With(p => p.Errors, _fixture.CreateMany<ValidationFailure>(0).ToList()).Create();
            mqSignUpValidator.Setup(mq => mq.Validate(It.IsAny<UserSignUp>())).Returns(validationResult);

            var identityResult = IdentityResult.Success;
            mqUserManager.Setup(mq => mq.UpdateAsync(It.IsAny<ApplicationUser>()))
                .Returns(Task.FromResult(identityResult));

            var servicePlan = _fixture.Build<ServicePlan>().Create();
            mqRoleManager.Setup(mq => mq.FindByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(servicePlan));

            var claimsResult = IdentityResult.Success;
            mqUserManager.Setup(mq => mq.AddClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IEnumerable<Claim>>()))
                .Returns(Task.FromResult(claimsResult));
            var foundUser = _fixture.Create<ApplicationUser>();
            mqUserManager.Setup(mq => mq.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(foundUser));

            var removeClaimsResult = IdentityResult.Success;
            mqUserManager.Setup(mq => mq.RemoveClaimsAsync(It.IsAny<ApplicationUser>(), It.IsAny<IList<Claim>>()))
               .Returns(Task.FromResult(removeClaimsResult));


            var signUpUser = _fixture.Build<UserSignUp>()
                .With(p => p.PlanId, servicePlan.Id).Create();
            var result = await userService.Update(signUpUser, string.Empty);

            result.Should().NotBeNull();
            result.Successful.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Messages.Should().HaveCount(0);
            mqSignUpValidator.Verify(mq => mq.Validate(It.IsAny<UserSignUp>()), Times.Once);
            mqUserManager.Verify(mq => mq.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            mqUserManager.Verify(mq => mq.FindByNameAsync(It.IsAny<string>()), Times.Never);
            mqUserManager.Verify(mq => mq.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Once);
            mqRoleManager.Verify(mq => mq.FindByIdAsync(It.IsAny<string>()), Times.Once);
            mqUserManager.Verify(mq => mq.GetClaimsAsync(It.IsAny<ApplicationUser>()), Times.Exactly(2));
        }

        [Test]
        public async Task ChangeEmail_ValidationFailed_InvalidEmail()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository, 
            out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);
            var validationResult = _fixture.Build<ValidationResult>()
                .With(p => p.Errors, _fixture.CreateMany<ValidationFailure>(1).ToList()).Create();
            mqEmailUpdateValidator.Setup(mq => mq.Validate(It.IsAny<EmailUpdate>())).Returns(validationResult);

            var user = _fixture.Create<ApplicationUser>();
            var update = new EmailUpdate() { NewEmail = "", NewEmailConfirmation = "" };
            var changeResult = await userService.ChangeEmail(user, update);

            changeResult.Data.Should().NotBeNull();
            changeResult.Successful.Should().BeFalse();
            changeResult.Messages.Should().NotBeEmpty().And.HaveCount(1);
            mqEmailUpdateValidator.Verify(mq => mq.Validate(It.IsAny<EmailUpdate>()), Times.Once);
            mqUserManager.Verify(mq => mq.GenerateChangeEmailTokenAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()),Times.Never);
        }

        [Test]
        public async Task ChangeEmail_ValidEmail_FailedToChangeEmail()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository,
            out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);
            var validationResult = _fixture.Build<ValidationResult>()
                .With(p => p.Errors, _fixture.CreateMany<ValidationFailure>(0).ToList()).Create();
            mqEmailUpdateValidator.Setup(mq => mq.Validate(It.IsAny<EmailUpdate>())).Returns(validationResult);

            var identityResult = IdentityResult.Failed(_fixture.CreateMany<IdentityError>(2).ToArray());
            mqUserManager.Setup(mq => mq.ChangeEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(identityResult));


            var user = _fixture.Create<ApplicationUser>();
            var update = new EmailUpdate() { NewEmail = "", NewEmailConfirmation = "" };
            var changeResult = await userService.ChangeEmail(user, update);

            changeResult.Data.Should().NotBeNull();
            changeResult.Successful.Should().BeFalse();
            changeResult.Messages.Should().NotBeEmpty().And.HaveCount(2);
            mqEmailUpdateValidator.Verify(mq => mq.Validate(It.IsAny<EmailUpdate>()), Times.Once);
            mqUserManager.Verify(mq => mq.GenerateChangeEmailTokenAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            mqUserManager.Verify(mq => mq.ChangeEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ChangeEmail_ValidEmail_SuccessChangeEmail()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository,
            out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);
            var validationResult = _fixture.Build<ValidationResult>()
                .With(p => p.Errors, _fixture.CreateMany<ValidationFailure>(0).ToList()).Create();
            mqEmailUpdateValidator.Setup(mq => mq.Validate(It.IsAny<EmailUpdate>())).Returns(validationResult);

            var identityResult = IdentityResult.Success;
            mqUserManager.Setup(mq => mq.ChangeEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(identityResult));


            var user = _fixture.Create<ApplicationUser>();
            var update = new EmailUpdate() { NewEmail = "", NewEmailConfirmation = "" };
            var changeResult = await userService.ChangeEmail(user, update);

            changeResult.Data.Should().NotBeNull();
            changeResult.Successful.Should().BeFalse();
            changeResult.Messages.Should().BeEmpty();
            mqEmailUpdateValidator.Verify(mq => mq.Validate(It.IsAny<EmailUpdate>()), Times.Once);
            mqUserManager.Verify(mq => mq.GenerateChangeEmailTokenAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
            mqUserManager.Verify(mq => mq.ChangeEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ChangePassword_InvalidPassword()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository,
            out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);

            var validationResult = _fixture.Build<ValidationResult>()
                .With(p => p.Errors, _fixture.CreateMany<ValidationFailure>(1).ToList()).Create();
            mqPasswordChangeValidator.Setup(mq => mq.Validate(It.IsAny<UserPasswordChange>())).Returns(validationResult);

            var user = _fixture.Create<ApplicationUser>();
            var newPassword = new UserPasswordChange { Password = "", };
            var changeResult = await userService.ChangePassword(user, newPassword);

            changeResult.Should().NotBeNull();
            changeResult.Data.Should().NotBeNull();
            changeResult.Successful.Should().BeFalse();
            changeResult.Messages.Should().NotBeEmpty().And.HaveCount(1);
            mqPasswordChangeValidator.Verify(mq => mq.Validate(It.IsAny<UserPasswordChange>()), Times.Once);
         }

        [Test]
        public async Task ChangePassword_NotAbleToChange()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository,
            out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);
            
            var validationResult = _fixture.Build<ValidationResult>()
                .With(p => p.Errors, _fixture.CreateMany<ValidationFailure>(0).ToList()).Create();
            mqPasswordChangeValidator.Setup(mq => mq.Validate(It.IsAny<UserPasswordChange>())).Returns(validationResult);

            mqUserManager.Setup(mq => mq.ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Failed(_fixture.CreateMany<IdentityError>(2).ToArray())));

            var user = _fixture.Create<ApplicationUser>();
            var newPassword = new UserPasswordChange { Password = "", };
            var changeResult = await userService.ChangePassword(user, newPassword);

            changeResult.Should().NotBeNull();
            changeResult.Data.Should().NotBeNull();
            changeResult.Successful.Should().BeFalse();
            changeResult.Messages.Should().NotBeEmpty().And.HaveCount(2);
            mqPasswordChangeValidator.Verify(mq => mq.Validate(It.IsAny<UserPasswordChange>()), Times.Once);
            mqUserManager.Verify(mq => mq.ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ChangePassword_PasswordUpdated()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository,
            out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);

            var validationResult = _fixture.Build<ValidationResult>()
                .With(p => p.Errors, _fixture.CreateMany<ValidationFailure>(0).ToList()).Create();
            mqPasswordChangeValidator.Setup(mq => mq.Validate(It.IsAny<UserPasswordChange>())).Returns(validationResult);

            mqUserManager.Setup(mq => mq.ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success));
            var user = _fixture.Create<ApplicationUser>();
            var newPassword = new UserPasswordChange { Password = "", };
            var changeResult = await userService.ChangePassword(user, newPassword);

            changeResult.Should().NotBeNull();
            changeResult.Data.Should().NotBeNull();
            changeResult.Successful.Should().BeTrue();
            changeResult.Messages.Should().BeEmpty();
            mqPasswordChangeValidator.Verify(mq => mq.Validate(It.IsAny<UserPasswordChange>()), Times.Once);
            mqUserManager.Verify(mq => mq.ChangePasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task RecoverPassword_NotValidUser()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository,
            out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);

            var changeResult = await userService.RecoverPassword("");

            changeResult.Should().NotBeNull();
            changeResult.Data.Should().NotBeNull();
            changeResult.Successful.Should().BeFalse();
            changeResult.Messages.Should().NotBeEmpty().And.HaveCount(1);
            mqUserManager.Verify(mq => mq.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            mqUserManager.Verify(mq => mq.FindByNameAsync(It.IsAny<string>()), Times.Once);
            mqPasswordSender.Verify(mq => mq.Send(It.IsAny<ApplicationUser>()), Times.Never);
        }

        [Test]
        public async Task RecoverPassword_RecoveryEmailSent()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository,
            out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);

            mqUserManager.Setup(mq => mq.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.Create<ApplicationUser>()));
            var changeResult = await userService.RecoverPassword("");

            changeResult.Should().NotBeNull();
            changeResult.Data.Should().NotBeNull();
            changeResult.Successful.Should().BeTrue();
            changeResult.Messages.Should().NotBeEmpty().And.HaveCount(1);
            mqUserManager.Verify(mq => mq.FindByEmailAsync(It.IsAny<string>()), Times.Once);
            mqUserManager.Verify(mq => mq.FindByNameAsync(It.IsAny<string>()), Times.Never);
            mqPasswordSender.Verify(mq => mq.Send(It.IsAny<ApplicationUser>()), Times.Once);
        }

        [Test]
        public async Task ResetPassword_ValidationFailed()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository,
            out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);

            var validationResult = _fixture.Build<ValidationResult>()
                .With(p => p.Errors, _fixture.CreateMany<ValidationFailure>(2).ToList()).Create();
            mqPasswordResetValidator.Setup(mq => mq.Validate(It.IsAny<UserPasswordReset>()))
                .Returns(validationResult);

            var newPassword = new UserPasswordReset { Password = "", };
            var changeResult = await userService.ResetPassword(newPassword);

            changeResult.Should().NotBeNull();
            changeResult.Data.Should().NotBeNull();
            changeResult.Successful.Should().BeFalse();
            changeResult.Messages.Should().NotBeEmpty().And.HaveCount(2);
            mqPasswordResetValidator.Verify(mq => mq.Validate(It.IsAny<UserPasswordReset>()), Times.Once);
            mqUserManager.Verify(mq => mq.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mqUserManager.Verify(mq => mq.FindByEmailAsync(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ResetPassword_FailedToReset()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository,
            out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);

            var validationResult = _fixture.Build<ValidationResult>()
                .With(p => p.Errors, _fixture.CreateMany<ValidationFailure>(0).ToList()).Create();
            mqPasswordResetValidator.Setup(mq => mq.Validate(It.IsAny<UserPasswordReset>()))
                .Returns(validationResult);

            mqUserManager.Setup(mq => mq.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.Create<ApplicationUser>()));
            mqUserManager.Setup(mq => mq.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Failed()));


            var newPassword = new UserPasswordReset { Password = "", };
            var changeResult = await userService.ResetPassword(newPassword);

            changeResult.Should().NotBeNull();
            changeResult.Data.Should().NotBeNull();
            changeResult.Successful.Should().BeFalse();
            changeResult.Messages.Should().BeEmpty();
            mqPasswordResetValidator.Verify(mq => mq.Validate(It.IsAny<UserPasswordReset>()), Times.Once);
            mqUserManager.Verify(mq => mq.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mqUserManager.Verify(mq => mq.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ResetPassword()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository,
            out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);

            var validationResult = _fixture.Build<ValidationResult>()
                .With(p => p.Errors, _fixture.CreateMany<ValidationFailure>(0).ToList()).Create();
            mqPasswordResetValidator.Setup(mq => mq.Validate(It.IsAny<UserPasswordReset>()))
                .Returns(validationResult);

            mqUserManager.Setup(mq => mq.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.Create<ApplicationUser>()));

            mqUserManager.Setup(mq => mq.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success));

            var newPassword = new UserPasswordReset { Password = "", };
            var changeResult = await userService.ResetPassword(newPassword);

            changeResult.Should().NotBeNull();
            changeResult.Data.Should().NotBeNull();
            changeResult.Successful.Should().BeTrue();
            changeResult.Messages.Should().HaveCount(1);
            mqPasswordResetValidator.Verify(mq => mq.Validate(It.IsAny<UserPasswordReset>()), Times.Once);
            mqUserManager.Verify(mq => mq.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mqUserManager.Verify(mq => mq.FindByEmailAsync(It.IsAny<string>()), Times.Once);
        }


        [Test]
        public async Task ConfirmUser_UserNotFound()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository,
            out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);

            var changeResult = await userService.ConfirmUser("", "");

            changeResult.Should().NotBeNull();
            changeResult.Data.Should().NotBeNull();
            changeResult.Successful.Should().BeFalse();
            changeResult.Messages.Should().NotBeEmpty().And.HaveCount(1);
            mqUserManager.Verify(mq => mq.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task ConfirmUser_FailedToConfirmEmail()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository,
            out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);

            mqUserManager.Setup(mq => mq.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.Create<ApplicationUser>()));

            mqUserManager.Setup(mq => mq.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Failed()));

            var changeResult = await userService.ConfirmUser("", "");

            changeResult.Should().NotBeNull();
            changeResult.Data.Should().NotBeNull();
            changeResult.Successful.Should().BeFalse();
            changeResult.Messages.Should().BeEmpty();
            mqUserManager.Verify(mq => mq.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ConfirmUser_Succeeded()
        {
            var userService = GetService(true, Enumerable.Empty<IdentityError>(), out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository,
            out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator);

            mqUserManager.Setup(mq => mq.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(_fixture.Create<ApplicationUser>()));

            mqUserManager.Setup(mq => mq.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(IdentityResult.Success));

            var changeResult = await userService.ConfirmUser("", "");

            changeResult.Should().NotBeNull();
            changeResult.Data.Should().NotBeNull();
            changeResult.Successful.Should().BeTrue();
            changeResult.Messages.Should().HaveCount(1);
            mqUserManager.Verify(mq => mq.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Once);
        }

        private UsersService GetService(bool successo, IEnumerable<IdentityError> identityErrors, out Mock<UserManager<ApplicationUser>> mqUserManager, out Mock<IValidator<UserSignUp>> mqSignUpValidator, out Mock<IPasswordRecoveryEmailSender> mqPasswordSender,
            out Mock<IUriService> mqUriService, out Mock<RoleManager<ServicePlan>> mqRoleManager, out Mock<IValidator<UserPasswordChange>> mqPasswordChangeValidator, out Mock<IValidator<UserPasswordReset>> mqPasswordResetValidator, out Mock<IRepository<ApplicationUserPlan>> mqUserPlanRepository,
            out Mock<IWelcomeEmailSender> mqWelcomeSender, out Mock<IValidator<EmailUpdate>> mqEmailUpdateValidator)
        {
            mqUserManager = MockUserManager(successo, identityErrors.ToArray());
            mqRoleManager = MockRoleManager(successo, identityErrors.ToArray());
            mqSignUpValidator = _mqRepo.Create<IValidator<UserSignUp>>();
            mqEmailUpdateValidator = _mqRepo.Create<IValidator<EmailUpdate>>();
            mqUriService = _mqRepo.Create<IUriService>();
            mqPasswordChangeValidator = _mqRepo.Create<IValidator<UserPasswordChange>>();
            mqPasswordResetValidator = _mqRepo.Create<IValidator<UserPasswordReset>>();
            mqUserPlanRepository = _mqRepo.Create<IRepository<ApplicationUserPlan>>();
            mqPasswordSender = _mqRepo.Create<IPasswordRecoveryEmailSender>();
            mqWelcomeSender = _mqRepo.Create<IWelcomeEmailSender>();
            var mqMapper = _mqRepo.Create<IMapper>();
            mqMapper.Setup(mq => mq.Map<OutputUser>(It.IsAny<ApplicationUser>()))
                .Returns(new OutputUser());
            return new UsersService(mqUserManager.Object, mqSignUpValidator.Object, mqUriService.Object, mqRoleManager.Object, mqPasswordChangeValidator.Object, mqUserPlanRepository.Object, mqPasswordSender.Object,
                                    mqWelcomeSender.Object, mqEmailUpdateValidator.Object, mqPasswordResetValidator.Object, mqMapper.Object);
        }
        private Mock<UserManager<ApplicationUser>> MockUserManager(bool success = true, IdentityError[] identityErrors = default)
        {
            IdentityResult result = success ? IdentityResult.Success : IdentityResult.Failed(identityErrors);
            var store = new Mock<IUserStore<ApplicationUser>>();
            var mgr = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            var user = _fixture.Build<ApplicationUser>()
                .With(p => p.Email, "email@email.com").CreateMany(5).AsQueryable();
            mgr.Object.UserValidators.Add(new UserValidator<ApplicationUser>());
            mgr.Object.PasswordValidators.Add(new PasswordValidator<ApplicationUser>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(result);
            mgr.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(result);
            mgr.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(result);
            mgr.SetupGet(x => x.Users).Returns(user);
            return mgr;
        }
        private Mock<RoleManager<ServicePlan>> MockRoleManager(bool success = true, IdentityError[] identityErrors = default)
        {
            IdentityResult result  = success ? IdentityResult.Success : IdentityResult.Failed(identityErrors);
            var store = new Mock<IRoleStore<ServicePlan>>();
            var mgr = new Mock<RoleManager<ServicePlan>>(store.Object, null, null, null, null);
            mgr.Object.RoleValidators.Add(new RoleValidator<ServicePlan>());

            mgr.Setup(x => x.DeleteAsync(It.IsAny<ServicePlan>())).ReturnsAsync(result);
            mgr.Setup(x => x.CreateAsync(It.IsAny<ServicePlan>())).ReturnsAsync(result);
            mgr.Setup(x => x.UpdateAsync(It.IsAny<ServicePlan>())).ReturnsAsync(result);
            mgr.SetupGet(x => x.Roles).Returns(_fixture.CreateMany<ServicePlan>(5).AsQueryable());

            return mgr;
        }
    }
}
