using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transdit.Repository.Migrations
{
    public partial class mapeamentododicionarioparaousuario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TbCustomDictionary",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", nullable: false),
                    Description = table.Column<string>(type: "varchar(255)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbCustomDictionary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbCustomDictionary_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbCustomDictionaryWord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDictionary = table.Column<int>(type: "int", nullable: false),
                    Word = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbCustomDictionaryWord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbCustomDictionaryWord_TbCustomDictionary_IdDictionary",
                        column: x => x.IdDictionary,
                        principalTable: "TbCustomDictionary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "d79b6742-3500-4a37-beb6-a3695ed589a0");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "c5d96f75-aaff-4887-87f2-0f8b7425a211");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "6bbb11cd-e80c-4e28-ae30-e044a2bff6d6");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "3314caa7-c178-4db3-81e6-8647cade12a5");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                column: "ConcurrencyStamp",
                value: "da41e41b-eff8-42d2-a14d-a84ee66d4f77");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                column: "ConcurrencyStamp",
                value: "f3da587b-a09e-4614-a0fa-c056bfca1d91");

            migrationBuilder.CreateIndex(
                name: "IX_TbCustomDictionary_UserId",
                table: "TbCustomDictionary",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_tbCustomDictionaryWord_IdDictionary",
                table: "tbCustomDictionaryWord",
                column: "IdDictionary");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbCustomDictionaryWord");

            migrationBuilder.DropTable(
                name: "TbCustomDictionary");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "e12b592a-b2f9-4e4f-b254-275c866a7b84");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "94b735ce-53ce-416a-98ae-18332bb37617");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "62453f51-6c4b-4229-bc6f-cc49b91ccbf9");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "1fbc8177-9cf2-4c1a-929c-982fe56611c8");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                column: "ConcurrencyStamp",
                value: "dc96ba50-ae48-4334-995f-b5d4e6f34a82");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                column: "ConcurrencyStamp",
                value: "3cb963db-1b98-4597-a158-efc781e398ac");
        }
    }
}
