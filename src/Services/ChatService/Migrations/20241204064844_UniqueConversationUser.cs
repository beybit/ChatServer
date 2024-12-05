using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatService.Migrations
{
    /// <inheritdoc />
    public partial class UniqueConversationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConversationUsers_ConversationId",
                table: "ConversationUsers");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "ConversationMessages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_ConversationUsers_ConversationId_UserId",
                table: "ConversationUsers",
                columns: new[] { "ConversationId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ConversationUsers_ConversationId_UserId",
                table: "ConversationUsers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ConversationMessages");

            migrationBuilder.CreateIndex(
                name: "IX_ConversationUsers_ConversationId",
                table: "ConversationUsers",
                column: "ConversationId");
        }
    }
}
