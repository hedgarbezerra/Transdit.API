using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transdit.Repository.Migrations
{
    public partial class Melhorianosalvamentodositenstranscritos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "TbTranscriptions",
                newName: "InputedFileName");
            
            migrationBuilder.AddColumn<string>(
                name: "StorageFileName",
                table: "TbTranscriptions",
                type: "varchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "TbTranscriptions",
                type: "varchar(10)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "TbTranscriptions",
                type: "varchar(40)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "2413ad87-69f6-4f05-ab07-714d82556e41");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "62b0c708-4dcb-4742-bdf5-14b58c3dd27e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "ba044ad4-360f-411c-a600-ce440025292c");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "3c7e7b42-9186-4186-814d-c0ddaf6ee6f9");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                column: "ConcurrencyStamp",
                value: "c1e7bd15-a601-435d-82e0-1ca25e534621");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ConcurrencyStamp", "NormalizedName" },
                values: new object[] { "598c8e3f-da77-4b60-97da-1e2f636693d4", "a" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InputedFileName",
                table: "TbTranscriptions");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "TbTranscriptions");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "TbTranscriptions");

            migrationBuilder.RenameColumn(
                name: "StorageFileName",
                table: "TbTranscriptions",
                newName: "FileName");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "ConcurrencyStamp",
                value: "edc40dfc-0d1f-40b4-909e-ba9bbe9e7f45");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "ConcurrencyStamp",
                value: "a3074307-912d-468c-a5ca-be1ecaaa7d1e");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "ConcurrencyStamp",
                value: "3c185a91-9d76-4ca8-ab0c-7bb99db900a8");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                column: "ConcurrencyStamp",
                value: "8eb51e23-24d4-4d19-ad64-19e8097be5da");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                column: "ConcurrencyStamp",
                value: "ca9a3ef4-198e-4824-bfef-2a4fc5ff722f");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ConcurrencyStamp", "NormalizedName" },
                values: new object[] { "232cd765-0feb-4ac7-bfec-06f40505e216", "ADMINISTRATOR" });
        }
    }
}
