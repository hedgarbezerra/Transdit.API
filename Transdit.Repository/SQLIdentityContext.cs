using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transdit.Core.Domain;
using Transdit.Core.Models;
using Transdit.Repository.Mappings;

namespace Transdit.Repository
{
    [ExcludeFromCodeCoverage]
    public class SqlIdentityContext : IdentityDbContext<ApplicationUser, ServicePlan, int, IdentityUserClaim<int>, ApplicationUserPlan,
                    IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DbSet<Transcription> Transcriptions { get; set; }
        public DbSet<LogItem> Logs { get; set; }
        public DbSet<CustomDictionary> Dictionaries { get; set; }

        public SqlIdentityContext() { }
        public SqlIdentityContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new ServicePlanEntityMap());
            builder.ApplyConfiguration(new UserEntityMap());
            builder.ApplyConfiguration(new TranscriptionEntityMap());
            builder.ApplyConfiguration(new LogItemEntityMap());
            builder.ApplyConfiguration(new UserServicePlanEntityMap());
            builder.ApplyConfiguration(new CustomDictionaryEntityMap());

            InitializeServicePlans(builder);
        }

        private static void InitializeServicePlans(ModelBuilder builder)
        {
            builder.Entity<ServicePlan>().HasData(
                new ServicePlan("Grátis", "Plano gratuíto com limite de 30 minutos de transcrição para quem deseja experimentar a ferramenta.", false, 0, TimeSpan.FromMinutes(30), TimeSpan.FromDays(10)) { Id = 1 , NormalizedName = "GRATIS"},
                new ServicePlan("Básico", "Plano basico mensal limitado à 100 minutos de transcrição.", false, 22.99, TimeSpan.FromMinutes(100)) { Id = 2, NormalizedName = "BASICO"},
                new ServicePlan("Padrão", "Plano padrão com capacidade de transcrição de 250 minutos mensais e salvamento do resultado das transcrições, se desejar", true, 52.99, TimeSpan.FromMinutes(250)) { Id = 3, NormalizedName = "PADRAO" },
                new ServicePlan("Premium", "Plano Premium com capacidade de transcrição de 500 minutos mensais, assim como capacidade de salvar as transcrições", true, 100.00, TimeSpan.FromMinutes(500)) { Id = 4, NormalizedName = "PREMIUM" },
                new ServicePlan("Pago por Uso", "Plano pago por uso mensal com todas capacidades do plano Premium porém sem limite de tempo, mas cada minuto sendo cobrado por R$0,2357736", true, 0, TimeSpan.MaxValue) { Id = 5, NormalizedName = "PAGOPORUSO" },
                new ServicePlan("Administrator", "Hidden", true, 0, TimeSpan.MaxValue, TimeSpan.MaxValue) { Id = 6, NormalizedName = "ADMINISTRATOR" });
        }
    }
}
