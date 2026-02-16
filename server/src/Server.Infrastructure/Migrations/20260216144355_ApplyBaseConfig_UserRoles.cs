using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApplyBaseConfig_UserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "user_roles",
                type: "timestamptz",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "user_roles",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTime>(
                name: "deleted_at",
                table: "user_roles",
                type: "timestamptz",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user_roles",
                type: "timestamptz",
                nullable: false,
                defaultValueSql: "NOW()",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "refresh_tokens",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "ix_userrole_created_at",
                table: "user_roles",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_userrole_deleted_at",
                table: "user_roles",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "ix_userrole_is_active",
                table: "user_roles",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_refreshtoken_is_active",
                table: "refresh_tokens",
                column: "is_active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_userrole_created_at",
                table: "user_roles");

            migrationBuilder.DropIndex(
                name: "ix_userrole_deleted_at",
                table: "user_roles");

            migrationBuilder.DropIndex(
                name: "ix_userrole_is_active",
                table: "user_roles");

            migrationBuilder.DropIndex(
                name: "ix_refreshtoken_is_active",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "refresh_tokens");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "user_roles",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamptz",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "user_roles",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "deleted_at",
                table: "user_roles",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamptz",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "user_roles",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamptz",
                oldDefaultValueSql: "NOW()");
        }
    }
}
