using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Common.Migrations
{
    /// <inheritdoc />
    public partial class CloudStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MongoConnectionString",
                table: "AppSettings",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MongoConnectionString",
                table: "AppSettings");
        }
    }
}
