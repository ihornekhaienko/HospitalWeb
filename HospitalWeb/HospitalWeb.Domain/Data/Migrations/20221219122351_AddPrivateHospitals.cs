using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalWeb.Domain.Data.Migrations
{
    public partial class AddPrivateHospitals : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Hospitals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ServicePrice",
                table: "Doctors",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Hospitals");

            migrationBuilder.DropColumn(
                name: "ServicePrice",
                table: "Doctors");
        }
    }
}
