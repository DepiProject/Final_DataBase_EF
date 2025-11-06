using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCampus.Infra.Migrations
{
    /// <inheritdoc />
    public partial class FixRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Departments_DepartmentId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Instructors_HeadId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Courses_CourseId",
                table: "Exams");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamSubmissions_Exams_ExamId",
                table: "ExamSubmissions");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamSubmissions_Instructors_GradedBy",
                table: "ExamSubmissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Instructors_Departments_DepartmentId",
                table: "Instructors");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_UserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Departments_DepartmentId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_ExamSubmissions_Exam_Student",
                table: "ExamSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_ExamAnswers_Submission_Question",
                table: "ExamAnswers");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_Student_Course",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_UniqueRecord",
                table: "Attendances");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "ExamSubmissions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ExamId",
                table: "ExamSubmissions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SubmissionId",
                table: "ExamAnswers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "Enrollments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "Attendances",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_ExamSubmissions_Exam_Student",
                table: "ExamSubmissions",
                columns: new[] { "ExamId", "StudentId" },
                unique: true,
                filter: "[ExamId] IS NOT NULL AND [StudentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ExamAnswers_Submission_Question",
                table: "ExamAnswers",
                columns: new[] { "SubmissionId", "QuestionId" },
                unique: true,
                filter: "[SubmissionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_Student_Course",
                table: "Enrollments",
                columns: new[] { "StudentId", "CourseId" },
                unique: true,
                filter: "[StudentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_UniqueRecord",
                table: "Attendances",
                columns: new[] { "StudentId", "CourseId", "Date" },
                unique: true,
                filter: "[StudentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Departments_DepartmentId",
                table: "Courses",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Instructors_HeadId",
                table: "Departments",
                column: "HeadId",
                principalTable: "Instructors",
                principalColumn: "InstructorId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Courses_CourseId",
                table: "Exams",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamSubmissions_Exams_ExamId",
                table: "ExamSubmissions",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "ExamId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamSubmissions_Instructors_GradedBy",
                table: "ExamSubmissions",
                column: "GradedBy",
                principalTable: "Instructors",
                principalColumn: "InstructorId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Instructors_Departments_DepartmentId",
                table: "Instructors",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Departments_DepartmentId",
                table: "Students",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Departments_DepartmentId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Instructors_HeadId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Courses_CourseId",
                table: "Exams");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamSubmissions_Exams_ExamId",
                table: "ExamSubmissions");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamSubmissions_Instructors_GradedBy",
                table: "ExamSubmissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Instructors_Departments_DepartmentId",
                table: "Instructors");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_UserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Students_Departments_DepartmentId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_ExamSubmissions_Exam_Student",
                table: "ExamSubmissions");

            migrationBuilder.DropIndex(
                name: "IX_ExamAnswers_Submission_Question",
                table: "ExamAnswers");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_Student_Course",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_UniqueRecord",
                table: "Attendances");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "ExamSubmissions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ExamId",
                table: "ExamSubmissions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SubmissionId",
                table: "ExamAnswers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "Enrollments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "Attendances",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamSubmissions_Exam_Student",
                table: "ExamSubmissions",
                columns: new[] { "ExamId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamAnswers_Submission_Question",
                table: "ExamAnswers",
                columns: new[] { "SubmissionId", "QuestionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_Student_Course",
                table: "Enrollments",
                columns: new[] { "StudentId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_UniqueRecord",
                table: "Attendances",
                columns: new[] { "StudentId", "CourseId", "Date" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Departments_DepartmentId",
                table: "Courses",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Instructors_HeadId",
                table: "Departments",
                column: "HeadId",
                principalTable: "Instructors",
                principalColumn: "InstructorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Courses_CourseId",
                table: "Exams",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "CourseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamSubmissions_Exams_ExamId",
                table: "ExamSubmissions",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "ExamId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamSubmissions_Instructors_GradedBy",
                table: "ExamSubmissions",
                column: "GradedBy",
                principalTable: "Instructors",
                principalColumn: "InstructorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Instructors_Departments_DepartmentId",
                table: "Instructors",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Departments_DepartmentId",
                table: "Students",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
