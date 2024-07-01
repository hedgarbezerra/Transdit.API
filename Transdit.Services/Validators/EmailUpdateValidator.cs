using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Models.Users;

namespace Transdit.Services.Validators
{
    public class EmailUpdateValidator : AbstractValidator<EmailUpdate>
    {
        public EmailUpdateValidator()
        {
            RuleFor(x => x.NewEmail).NotEmpty().EmailAddress(EmailValidationMode.Net4xRegex)
                .WithName("Novo e-mail")
                .DependentRules(() =>
                {
                    RuleFor(x => x.NewEmailConfirmation).NotEmpty()
                    .EmailAddress(EmailValidationMode.Net4xRegex).Equal(x => x.NewEmail)
                    .WithName("Confirmação do e-mail").WithMessage("{PropertyName} deve ser igual ao novo e-mail"); ;
                });
        }
    }
}
