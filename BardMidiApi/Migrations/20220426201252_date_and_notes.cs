using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BardMidiApi.Migrations
{
    public partial class date_and_notes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ServiceId",
                table: "Users",
                type: "decimal(20,0)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<string>(
                name: "AuthorNotes",
                table: "MidiItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UploadDate",
                table: "MidiItems",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorNotes",
                table: "MidiItems");

            migrationBuilder.DropColumn(
                name: "UploadDate",
                table: "MidiItems");

            migrationBuilder.AlterColumn<long>(
                name: "ServiceId",
                table: "Users",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,0)");
        }
    }
}
