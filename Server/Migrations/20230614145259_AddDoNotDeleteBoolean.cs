using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Respicere.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddDoNotDeleteBoolean : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DoNotDelete",
                table: "Images",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoNotDelete",
                table: "Images");
        }
    }
}
