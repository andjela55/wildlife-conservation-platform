using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WildlifeConservation.Repositories.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PasswordSalt",
                table: "Users",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE "Users"
                SET "PasswordHash" = 'lqAUSV0CyDeazcHyVWUyWOaBpl2tKJEcNMt8NLu6OmY=',
                    "PasswordSalt" = 'UmFuZ2VyU2VlZFNhbHQxMjM='
                WHERE "Id" = 1;
                """);

            migrationBuilder.Sql("""
                UPDATE "Users"
                SET "PasswordHash" = 'BeTAbhUpC/HalIdpn4pENMoRhM6MTo8l1G0pP/IqoqI=',
                    "PasswordSalt" = 'UmVzZWFyY2hTZWVkU2FsdDE='
                WHERE "Id" = 2;
                """);

            migrationBuilder.Sql("""
                UPDATE "Users"
                SET "PasswordHash" = 'S17Qn4Xrpy+dcZCVD5zuHuq4t32wjdBDGTSMzxjSDLY=',
                    "PasswordSalt" = 'QWRtaW5TZWVkU2FsdDEyMw=='
                WHERE "Id" = 3;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "Users");
        }
    }
}
