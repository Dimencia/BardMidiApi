using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BardMidiApi.Migrations
{
    public partial class indexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MidiItems_Hash",
                table: "MidiItems",
                column: "Hash");

            migrationBuilder.CreateIndex(
                name: "IX_MidiItems_Score",
                table: "MidiItems",
                column: "Score");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MidiItems_Hash",
                table: "MidiItems");

            migrationBuilder.DropIndex(
                name: "IX_MidiItems_Score",
                table: "MidiItems");
        }
    }
}
