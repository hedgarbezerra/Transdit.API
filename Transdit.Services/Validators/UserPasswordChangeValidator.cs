using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Models.Users;

namespace Transdit.Services.Validators
{
    public class UserPasswordChangeValidator : AbstractValidator<UserPasswordChange>
    {
        public UserPasswordChangeValidator()
        {
            RuleFor(x => x.Password).NotEmpty()
                .DependentRules(() =>
                {
                    RuleFor(x => x.NewPassword).NotEmpty()
                   .NotEqual(x => x.Password)
                   .MinimumLength(6).MaximumLength(14)
                   .Matches(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,14}$")
                   .DependentRules(() =>
                   {
                       RuleFor(x => x.NewPasswordConfirm).Equal(p => p.NewPassword)
                       .WithName("Confirmação da senha")
                       .WithMessage("{PropertyName} deve ser igual à nova senha");
                   })
                    .WithName("Nova senha");
            }).WithName("Antiga senha");
        }
    }
}
