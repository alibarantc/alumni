using System;
using Konekto.DataAccess.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Konekto.DataAccess.Migrations;

/// <summary>
/// Align existing JobPostings columns with the documented API rules.
/// Legacy rows without a company must be repaired explicitly before this migration runs.
/// </summary>
[DbContext(typeof(KonektoDbContext))]
[Migration("20261112_EnforceJobPostingRules")]
public partial class EnforceJobPostingRules : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Do not invent a company for historical data or silently truncate a title.
        migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM [JobPostings] WHERE [CompanyId] IS NULL)
    THROW 51001, 'JobPostings contains rows with NULL CompanyId. Assign a valid CompanyId before applying this migration.', 1;

IF EXISTS (SELECT 1 FROM [JobPostings] WHERE LEN([Title]) > 120)
    THROW 51002, 'JobPostings contains titles longer than 120 characters. Correct them before applying this migration.', 1;
");

        migrationBuilder.DropForeignKey(
            name: "FK_JobPostings_Companies_CompanyId",
            table: "JobPostings");

        migrationBuilder.DropIndex(
            name: "IX_JobPostings_CompanyId",
            table: "JobPostings");

        migrationBuilder.AlterColumn<string>(
            name: "Title",
            table: "JobPostings",
            type: "nvarchar(120)",
            maxLength: 120,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(200)",
            oldMaxLength: 200);

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "JobPostings",
            type: "nvarchar(max)",
            maxLength: 5000,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<int>(
            name: "CompanyId",
            table: "JobPostings",
            type: "int",
            nullable: false,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_JobPostings_CompanyId",
            table: "JobPostings",
            column: "CompanyId");

        migrationBuilder.AddForeignKey(
            name: "FK_JobPostings_Companies_CompanyId",
            table: "JobPostings",
            column: "CompanyId",
            principalTable: "Companies",
            principalColumn: "Id",
            onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_JobPostings_Companies_CompanyId",
            table: "JobPostings");

        migrationBuilder.DropIndex(
            name: "IX_JobPostings_CompanyId",
            table: "JobPostings");

        migrationBuilder.AlterColumn<int>(
            name: "CompanyId",
            table: "JobPostings",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AlterColumn<string>(
            name: "Title",
            table: "JobPostings",
            type: "nvarchar(200)",
            maxLength: 200,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(120)",
            oldMaxLength: 120);

        migrationBuilder.Sql("UPDATE [JobPostings] SET [Description] = N'' WHERE [Description] IS NULL;");

        migrationBuilder.AlterColumn<string>(
            name: "Description",
            table: "JobPostings",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldMaxLength: 5000,
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_JobPostings_CompanyId",
            table: "JobPostings",
            column: "CompanyId");

        migrationBuilder.AddForeignKey(
            name: "FK_JobPostings_Companies_CompanyId",
            table: "JobPostings",
            column: "CompanyId",
            principalTable: "Companies",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);
    }
}
