using Microsoft.AspNetCore.Identity;
using SmartCampus.Core.Entities;
using SmartCampus.Infra.Data;
using Microsoft.EntityFrameworkCore;

public static class DbInitializer
{
    public static async Task SeedAsync(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        SmartCampusDbContext context)
    {
        // ✅ Seed Roles
        var roles = new[] { "Admin", "Instructor", "Student" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole<int> { Name = role });
        }

        // ✅ Seed Admin
        if (await userManager.FindByEmailAsync("admin@campus.com") == null)
        {
            var admin = new AppUser
            {
                UserName = "admin",
                Email = "admin@campus.com",
                FirstName = "System",
                LastName = "Admin",
                EmailConfirmed = true,
                Role = "Admin"
            };

            var result = await userManager.CreateAsync(admin, "Admin@123");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

        // ✅ Seed Departments
        if (!context.Departments.Any())
        {
            context.Departments.AddRange(
                new Department { Name = "Computer Science", Building = "A", HeadId = null },
                new Department { Name = "Information Systems", Building = "B", HeadId = null },
                new Department { Name = "Artificial Intelligence", Building = "C", HeadId = null },
                new Department { Name = "Software Engineering", Building = "D", HeadId = null }
            );
            await context.SaveChangesAsync();
        }

        // ✅ Seed Instructor User
        var instructorEmail = "instructor@campus.com";
        AppUser? instructorUser = await userManager.FindByEmailAsync(instructorEmail);

        if (instructorUser == null)
        {
            instructorUser = new AppUser
            {
                UserName = "instructor",
                Email = instructorEmail,
                FirstName = "John",
                LastName = "Doe",
                EmailConfirmed = true,
                Role = "Instructor"
            };

            var result = await userManager.CreateAsync(instructorUser, "Instructor@123");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(instructorUser, "Instructor");

            context.Instructors.Add(new Instructor
            {
                FullName = $"{instructorUser.FirstName} {instructorUser.LastName}",
                UserId = instructorUser.Id,
                DepartmentId = 1
            });

            await context.SaveChangesAsync();
        }

        // ✅ Seed Student User
        var studentEmail = "student@campus.com";
        AppUser? studentUser = await userManager.FindByEmailAsync(studentEmail);

        if (studentUser == null)
        {
            studentUser = new AppUser
            {
                UserName = "student",
                Email = studentEmail,
                FirstName = "Ali",
                LastName = "Mahmoud",
                EmailConfirmed = true,
                Role = "Student"
            };

            var result = await userManager.CreateAsync(studentUser, "Student@123");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(studentUser, "Student");

            context.Students.Add(new Student
            {
                FullName = $"{studentUser.FirstName} {studentUser.LastName}",
                StudentCode = "S1001",
                DepartmentId = 1,
                UserId = studentUser.Id
            });

            await context.SaveChangesAsync();
        }

        // ✅ Seed Course
        if (!context.Courses.Any())
        {
            context.Courses.Add(new Course
            {
                CourseCode = "CS101",
                Name = "Introduction to Programming",
                Credits = 3,
                DepartmentId = 1,
                InstructorId = context.Instructors.First().InstructorId
            });

            await context.SaveChangesAsync();
        }
    }
}
