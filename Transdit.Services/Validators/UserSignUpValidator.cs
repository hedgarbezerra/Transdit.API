using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Domain;
using Transdit.Core.Models.Users;

namespace Transdit.Services.Validators
{
    public class UserSignUpValidator : AbstractValidator<UserSignUp>
    {
        public UserSignUpValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithName("Nome");
            RuleFor(x => x.UserName).NotEmpty().WithName("Usuário");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(14).Matches(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,14}$")
                .DependentRules(() =>
                {
                    RuleFor(x => x.PasswordConfirm).Equal(p => p.Password).WithName("Confirmação da senha");
                }).WithName("Senha");

            RuleFor(x => x.Email).NotEmpty().EmailAddress(EmailValidationMode.Net4xRegex);
            RuleFor(x => x.BirthDate).NotNull()
                .LessThan(DateTime.Now.Date.AddYears(-18))
                .WithMessage((u, dt) => "Você deve ter pelo menos 18 anos para utilizar a ferramenta.")
                .WithName("Data de nascimento");
            RuleFor(x => x.PlanId).GreaterThan(0)
                .WithMessage("Plano selecionado é inválido.");
        }
    }
}
