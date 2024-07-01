using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Transdit.Repository.Migrations
{
    public partial class Adiçãodeplanos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "AspNetRoles",
                type: "decimal(11,3)",
                precision: 11,
                scale: 3,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(6,4)",
                oldPrecision: 6,
                oldScale: 4);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "AllowTranscriptionSaving", "ConcurrencyStamp", "Description", "Maturity", "MonthlyLimitUsage", "Name", "NormalizedName", "Price" },
                values: new object[,]
                {
                    { 1, false, "edc40dfc-0d1f-40b4-909e-ba9bbe9e7f45", "Plano gratuíto com limite de 30 minutos de transcrição para quem deseja experimentar a ferramenta.", 8640000000000L, 18000000000L, "Grátis", "GRATIS", 0m },
                    { 2, false, "a3074307-912d-468c-a5ca-be1ecaaa7d1e", "Plano basico mensal limitado à 100 minutos de transcrição.", 25920000000000L, 60000000000L, "Básico", "BASICO", 22.99m },
                    { 3, true, "3c185a91-9d76-4ca8-ab0c-7bb99db900a8", "Plano padrão com capacidade de transcrição de 250 minutos mensais e salvamento do resultado das transcrições, se desejar", 25920000000000L, 150000000000L, "Padrão", "PADRAO", 52.99m },
                    { 4, true, "8eb51e23-24d4-4d19-ad64-19e8097be5da", "Plano Premium com capacidade de transcrição de 500 minutos mensais, assim como capacidade de salvar as transcrições", 25920000000000L, 300000000000L, "Premium", "PREMIUM", 100m },
                    { 5, true, "ca9a3ef4-198e-4824-bfef-2a4fc5ff722f", "Plano pago por uso mensal com todas capacidades do plano Premium porém sem limite de tempo, mas cada minuto sendo cobrado por R$0,2357736", 25920000000000L, 9223372036854775807L, "Pago por Uso", "PAGOPORUSO", 0m },
                    { 6, true, "232cd765-0feb-4ac7-bfec-06f40505e216", "Hidden", 9223372036854775807L, 9223372036854775807L, "Administrator", "ADMINISTRATOR", 0m }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "AspNetRoles",
                type: "decimal(6,4)",
                precision: 6,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(11,3)",
                oldPrecision: 11,
                oldScale: 3);
        }
    }
}
