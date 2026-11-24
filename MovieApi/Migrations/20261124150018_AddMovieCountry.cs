using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMovieCountry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Movies",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(
                "INSERT INTO [Countries] ([Name]) VALUES (N'Unknown'), (N'United States');");

            migrationBuilder.Sql(
                "UPDATE [Movies] SET [CountryId] = CASE [Title] " +
                "WHEN N'The Shawshank Redemption' THEN (SELECT [Id] FROM [Countries] WHERE [Name] = N'United States') " +
                "WHEN N'The Matrix' THEN (SELECT [Id] FROM [Countries] WHERE [Name] = N'United States') " +
                "WHEN N'The Dark Knight' THEN (SELECT [Id] FROM [Countries] WHERE [Name] = N'United States') " +
                "ELSE (SELECT [Id] FROM [Countries] WHERE [Name] = N'Unknown') END;");

            migrationBuilder.AlterColumn<int>(
                name: "CountryId",
                table: "Movies",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movies_CountryId",
                table: "Movies",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_Name",
                table: "Countries",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Countries_CountryId",
                table: "Movies",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_Countries_CountryId",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_CountryId",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Movies");

            migrationBuilder.DropTable(
                name: "Countries");
        }
    }
}
