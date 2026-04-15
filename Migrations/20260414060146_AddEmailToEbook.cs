using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShelf.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailToEbook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Ebooks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Ebooks");
        }
    }
}
