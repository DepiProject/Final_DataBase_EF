using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCampus.Infra.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTFTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuestionText",
                table: "TrueFalseQuestions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionText",
                table: "TrueFalseQuestions");
        }
    }
}
