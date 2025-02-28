﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Transdit.Repository;

#nullable disable

namespace Transdit.Repository.Migrations
{
    [DbContext(typeof(SqlIdentityContext))]
    [Migration("20230421024100_Correção do nome do plano de admin")]
    partial class Correçãodonomedoplanodeadmin
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Transdit.Core.Domain.ApplicationUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("datetime");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateAdded")
                        .HasColumnType("datetime");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(360)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TermsAgreed")
                        .HasColumnType("bit");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Transdit.Core.Domain.ApplicationUserPlan", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<DateTime>("Maturity")
                        .HasColumnType("datetime");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Transdit.Core.Domain.ServicePlan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("AllowTranscriptionSaving")
                        .HasColumnType("bit");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("varchar(max)");

                    b.Property<long>("Maturity")
                        .HasColumnType("bigint");

                    b.Property<long>("MonthlyLimitUsage")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<decimal>("Price")
                        .HasPrecision(11, 3)
                        .HasColumnType("decimal(11,3)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            AllowTranscriptionSaving = false,
                            ConcurrencyStamp = "e12b592a-b2f9-4e4f-b254-275c866a7b84",
                            Description = "Plano gratuíto com limite de 30 minutos de transcrição para quem deseja experimentar a ferramenta.",
                            Maturity = 8640000000000L,
                            MonthlyLimitUsage = 18000000000L,
                            Name = "Grátis",
                            NormalizedName = "GRATIS",
                            Price = 0m
                        },
                        new
                        {
                            Id = 2,
                            AllowTranscriptionSaving = false,
                            ConcurrencyStamp = "94b735ce-53ce-416a-98ae-18332bb37617",
                            Description = "Plano basico mensal limitado à 100 minutos de transcrição.",
                            Maturity = 25920000000000L,
                            MonthlyLimitUsage = 60000000000L,
                            Name = "Básico",
                            NormalizedName = "BASICO",
                            Price = 22.99m
                        },
                        new
                        {
                            Id = 3,
                            AllowTranscriptionSaving = true,
                            ConcurrencyStamp = "62453f51-6c4b-4229-bc6f-cc49b91ccbf9",
                            Description = "Plano padrão com capacidade de transcrição de 250 minutos mensais e salvamento do resultado das transcrições, se desejar",
                            Maturity = 25920000000000L,
                            MonthlyLimitUsage = 150000000000L,
                            Name = "Padrão",
                            NormalizedName = "PADRAO",
                            Price = 52.99m
                        },
                        new
                        {
                            Id = 4,
                            AllowTranscriptionSaving = true,
                            ConcurrencyStamp = "1fbc8177-9cf2-4c1a-929c-982fe56611c8",
                            Description = "Plano Premium com capacidade de transcrição de 500 minutos mensais, assim como capacidade de salvar as transcrições",
                            Maturity = 25920000000000L,
                            MonthlyLimitUsage = 300000000000L,
                            Name = "Premium",
                            NormalizedName = "PREMIUM",
                            Price = 100m
                        },
                        new
                        {
                            Id = 5,
                            AllowTranscriptionSaving = true,
                            ConcurrencyStamp = "dc96ba50-ae48-4334-995f-b5d4e6f34a82",
                            Description = "Plano pago por uso mensal com todas capacidades do plano Premium porém sem limite de tempo, mas cada minuto sendo cobrado por R$0,2357736",
                            Maturity = 25920000000000L,
                            MonthlyLimitUsage = 9223372036854775807L,
                            Name = "Pago por Uso",
                            NormalizedName = "PAGOPORUSO",
                            Price = 0m
                        },
                        new
                        {
                            Id = 6,
                            AllowTranscriptionSaving = true,
                            ConcurrencyStamp = "3cb963db-1b98-4597-a158-efc781e398ac",
                            Description = "Hidden",
                            Maturity = 9223372036854775807L,
                            MonthlyLimitUsage = 9223372036854775807L,
                            Name = "Administrator",
                            NormalizedName = "ADMINISTRATOR",
                            Price = 0m
                        });
                });

            modelBuilder.Entity("Transdit.Core.Domain.Transcription", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime");

                    b.Property<string>("InputedFileName")
                        .IsRequired()
                        .HasColumnType("varchar(max)");

                    b.Property<string>("Language")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Result")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StorageFileName")
                        .IsRequired()
                        .HasColumnType("varchar(max)");

                    b.Property<long>("Usage")
                        .HasColumnType("bigint");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("TbTranscriptions", (string)null);
                });

            modelBuilder.Entity("Transdit.Core.Models.LogItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime")
                        .HasColumnName("TimeStamp");

                    b.Property<string>("Exception")
                        .HasColumnType("nvarchar")
                        .HasColumnName("Exception");

                    b.Property<string>("LogLevel")
                        .IsRequired()
                        .HasColumnType("nvarchar")
                        .HasColumnName("Level");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar")
                        .HasColumnName("Message");

                    b.Property<string>("MessageTemplate")
                        .HasColumnType("nvarchar")
                        .HasColumnName("MessageTemplate");

                    b.Property<string>("Properties")
                        .HasColumnType("nvarchar")
                        .HasColumnName("Properties");

                    b.HasKey("Id");

                    b.ToTable("SerilogLoggingTable", null, t => t.ExcludeFromMigrations());
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("Transdit.Core.Domain.ServicePlan", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("Transdit.Core.Domain.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("Transdit.Core.Domain.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("Transdit.Core.Domain.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Transdit.Core.Domain.ApplicationUserPlan", b =>
                {
                    b.HasOne("Transdit.Core.Domain.ServicePlan", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Transdit.Core.Domain.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Transdit.Core.Domain.Transcription", b =>
                {
                    b.HasOne("Transdit.Core.Domain.ApplicationUser", "User")
                        .WithMany("Transcriptions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Transdit.Core.Domain.ApplicationUser", b =>
                {
                    b.Navigation("Transcriptions");
                });
#pragma warning restore 612, 618
        }
    }
}
