using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SystemCore.Data.EF.Migrations
{
    public partial class ChangeNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bills_AspNetUsers_CustomerId",
                table: "Bills");

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                table: "Bills",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddForeignKey(
                name: "FK_Bills_AspNetUsers_CustomerId",
                table: "Bills",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bills_AspNetUsers_CustomerId",
                table: "Bills");

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                table: "Bills",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bills_AspNetUsers_CustomerId",
                table: "Bills",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
