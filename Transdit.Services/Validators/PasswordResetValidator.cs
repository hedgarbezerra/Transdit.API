using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Models.Users;

namespace Transdit.Services.Validators
{
    public class PasswordResetValidator : AbstractValidator<UserPasswordReset>
    {
        public PasswordResetValidator()
        {
            RuleFor(x => x.Token).NotEmpty();
            RuleFor(x => x.Password).NotEmpty()
                  .MinimumLength(6).MaximumLength(14)
                  .Matches(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,14}$")
                  .DependentRules(() =>
                  {
                      RuleFor(x => x.PasswordConfirm).Equal(p => p.Password)
                      .WithName("Confirmação da senha")
                      .WithMessage("{PropertyName} deve ser igual à nova senha");
                  })
                   .WithName("Nova senha");
        }
    }
}
