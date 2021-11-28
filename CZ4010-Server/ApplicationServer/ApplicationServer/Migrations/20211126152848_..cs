using Microsoft.EntityFrameworkCore.Migrations;

namespace ApplicationServer.Migrations
{
    public partial class _ : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    URL = table.Column<string>(type: "text", nullable: false),
                    EncryptedFile = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.URL);
                });

            migrationBuilder.CreateTable(
                name: "Sharing",
                columns: table => new
                {
                    URL = table.Column<string>(type: "text", nullable: false),
                    TaggedUsername = table.Column<string>(type: "text", nullable: false),
                    IsOwner = table.Column<bool>(type: "boolean", nullable: false),
                    EncryptedKey = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sharing", x => new { x.URL, x.TaggedUsername });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "Sharing");
        }
    }
}
