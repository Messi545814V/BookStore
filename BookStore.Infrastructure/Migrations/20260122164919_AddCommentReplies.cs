using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentReplies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "BookComments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookComments_ParentId",
                table: "BookComments",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookComments_BookComments_ParentId",
                table: "BookComments",
                column: "ParentId",
                principalTable: "BookComments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookComments_BookComments_ParentId",
                table: "BookComments");

            migrationBuilder.DropIndex(
                name: "IX_BookComments_ParentId",
                table: "BookComments");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "BookComments");
        }
    }
}
