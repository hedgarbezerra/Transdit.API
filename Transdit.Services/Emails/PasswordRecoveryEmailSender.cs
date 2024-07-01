using HtmlAgilityPack;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Contracts;
using Transdit.Core.Domain;
using Transdit.Core.Models.Notification;

namespace Transdit.Services.Emails
{
    public interface IPasswordRecoveryEmailSender
    {
        Task Send(ApplicationUser target);
    }

    [ExcludeFromCodeCoverage]
    public class PasswordRecoveryEmailSender : IPasswordRecoveryEmailSender
    {
        private readonly ICryptography _cryptography;
        private IEmailFromFileGenerator _mailGenerator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificator<AzureEmailNotification> _notificator;
        private readonly string _url;

        public PasswordRecoveryEmailSender(IEmailFromFileGenerator mailGenerator,
                UserManager<ApplicationUser> userManager, INotificator<AzureEmailNotification> notificator, string url, ICryptography cryptography)
        {
            _mailGenerator = mailGenerator;
            _userManager = userManager;
            _notificator = notificator;
            _url = url;
            _cryptography = cryptography;
        }

        public async Task Send(ApplicationUser target)
        {
            var emailToken = await _userManager.GeneratePasswordResetTokenAsync(target);
            var htmlFile = _mailGenerator.Generate("\\Html\\recuperacao.html");
            var html = new HtmlDocument();
            html.LoadHtml(htmlFile);
            string emailHtml = GetEmailHtml(target, html, emailToken);
            var recipient = new Recipient() { Name = target.Name, Address = target.Email };

            AzureEmailNotification email = new AzureEmailNotification() { Title = $"Aqui está a recuperação da senha, {target.Name}", Message = emailHtml };
            email.Recipients.Add(recipient);

            await _notificator.Notify(email);
        }
        private string GetEmailHtml(ApplicationUser target,  HtmlDocument html, string token)
        {
            var textoPrincipal = html.DocumentNode.Descendants("p").FirstOrDefault(a => a.HasClass("texto-usuario"));
            if (textoPrincipal is not null)
                textoPrincipal.InnerHtml = string.Format(textoPrincipal.InnerHtml, target.Name);

            var botaoRecuperacao = html.DocumentNode.Descendants("a").FirstOrDefault(a => a.HasClass("link-recuperacao"));
            if (botaoRecuperacao is not null)
            {
                var criptoUser = _cryptography.Encrypt(target.Email);
                botaoRecuperacao.SetAttributeValue("href", $"{_url}/recuperacao?token={token}&user={criptoUser}");
            }

            var contato = html.DocumentNode.Descendants("a").FirstOrDefault(a => a.HasClass("mailto"));
            var contatoHref = contato?.GetAttributes("href").SingleOrDefault();
            if (contatoHref is not null)
            {
                string novoHref = string.Format(contatoHref.Value, target.Name);
                contato.SetAttributeValue("href", novoHref);
            }

            return html.DocumentNode.OuterHtml;
        }
    }
}
