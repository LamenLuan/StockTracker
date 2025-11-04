using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Common.Migrations
{
    /// <inheritdoc />
    public partial class NotificationControll : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastNotification",
                table: "AppSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TrackerGuid",
                table: "AppSettings",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastNotification",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "TrackerGuid",
                table: "AppSettings");
        }
    }
}
