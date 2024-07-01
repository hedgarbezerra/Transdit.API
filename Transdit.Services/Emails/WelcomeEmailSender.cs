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
    public interface IWelcomeEmailSender
    {
        Task Send(ApplicationUser target, ServicePlan plan);
    }

    [ExcludeFromCodeCoverage]
    public class WelcomeEmailSender : IWelcomeEmailSender
    {
        private readonly ICryptography _cryptography;
        private readonly IMailNotificationBuilder _notificationBuilder;
        private IEmailFromFileGenerator _mailGenerator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificator<AzureEmailNotification> _notificator;
        private readonly string _url;

        public WelcomeEmailSender(IEmailFromFileGenerator mailGenerator, UserManager<ApplicationUser> userManager, INotificator<AzureEmailNotification> notificator, IMailNotificationBuilder notificationBuilder, string url, ICryptography cryptography) : base()
        {
            _mailGenerator = mailGenerator;
            _userManager = userManager;
            _notificator = notificator;
            _notificationBuilder = notificationBuilder;
            _url = url;
            _cryptography = cryptography;
        }

        public async Task Send(ApplicationUser target, ServicePlan plan)
        {
            var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(target);
            var htmlFile = _mailGenerator.Generate("\\Html\\confirmar-conta.html");
            var html = new HtmlDocument();
            html.LoadHtml(htmlFile);

            string emailHtml = GetEmailHtml(target, plan, html, emailToken);            
            var recipient = new Recipient() { Name = target.Name, Address = target.Email };
            AzureEmailNotification email = new AzureEmailNotification() { Title = $"Boas vindas, {target.Name}",  Message = emailHtml};
            email.Recipients.Add(recipient);

            await _notificator.Notify(email);
        }

        private string GetEmailHtml(ApplicationUser target, ServicePlan plan, HtmlDocument html, string token)
        {
            var textoPrincipal = html.DocumentNode.Descendants("p").FirstOrDefault(a => a.HasClass("texto-usuario"));
            if(textoPrincipal is not null)
                textoPrincipal.InnerHtml = string.Format(textoPrincipal.InnerHtml, target.Name, plan.Name);

            var botaoConfirmacao = html.DocumentNode.Descendants("a").FirstOrDefault(a => a.HasClass("confirmar-conta"));
            if (botaoConfirmacao is not null)
            {
                var criptoUser = _cryptography.Encrypt(target.Email);
                botaoConfirmacao.SetAttributeValue("href", $"{_url}/confirmacao?token={token}&user={criptoUser}&name={target.UserName}");
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
