using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieApi.Migrations
{
    /// <inheritdoc />
    public partial class AddMovieDirector : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Directors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directors", x => x.Id);
                });

            migrationBuilder.AddColumn<int>(
                name: "DirectorId",
                table: "Movies",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(
                "INSERT INTO [Directors] ([Name]) VALUES (N'Unknown'), (N'Frank Darabont'), (N'Lana Wachowski'), (N'Christopher Nolan');");

            migrationBuilder.Sql(
                "UPDATE [Movies] SET [DirectorId] = CASE [Title] " +
                "WHEN N'The Shawshank Redemption' THEN (SELECT [Id] FROM [Directors] WHERE [Name] = N'Frank Darabont') " +
                "WHEN N'The Matrix' THEN (SELECT [Id] FROM [Directors] WHERE [Name] = N'Lana Wachowski') " +
                "WHEN N'The Dark Knight' THEN (SELECT [Id] FROM [Directors] WHERE [Name] = N'Christopher Nolan') " +
                "ELSE (SELECT [Id] FROM [Directors] WHERE [Name] = N'Unknown') END;");

            migrationBuilder.AlterColumn<int>(
                name: "DirectorId",
                table: "Movies",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movies_DirectorId",
                table: "Movies",
                column: "DirectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Directors_Name",
                table: "Directors",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_Directors_DirectorId",
                table: "Movies",
                column: "DirectorId",
                principalTable: "Directors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_Directors_DirectorId",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_DirectorId",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "DirectorId",
                table: "Movies");

            migrationBuilder.DropTable(
                name: "Directors");
        }
    }
}
