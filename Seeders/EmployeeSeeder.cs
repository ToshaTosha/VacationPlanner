using VacationPlanner.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using BCrypt.Net;

public class EmployeeSeeder
{
    private readonly VacationPlannerDbContext _context;

    public EmployeeSeeder(VacationPlannerDbContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        if (_context.Employees.Any()) return;

        var employees = new List<Employee>
        {
            // Руководители отделов (RoleId = 1)
            new Employee
            {
                DepartmentId = 1, // Отдел кадров
                PositionId = 1,   // HR-менеджер
                RoleId = 1,
                FirstName = "Ольга",
                LastName = "Петрова",
                MiddleName = "Сергеевна",
                HireDate = DateOnly.FromDateTime(new DateTime(2020, 1, 15)),
                Email = "hr.manager@company.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("HrPassword123!"),
                AccumulatedVacationDays = 28,
                IsMultipleChildren = false,
                HasDisabledChild = false,
                IsVeteran = false,
                IsHonorDonor = true
            },
            new Employee
            {
                DepartmentId = 2, // Финансовый отдел
                PositionId = 2,   // Финансовый директор
                RoleId = 1,
                FirstName = "Андрей",
                LastName = "Смирнов",
                MiddleName = "Игоревич",
                HireDate = DateOnly.FromDateTime(new DateTime(2019, 5, 10)),
                Email = "finance.director@company.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("FinancePass456!"),
                AccumulatedVacationDays = 30,
                IsMultipleChildren = true,
                HasDisabledChild = false,
                IsVeteran = true,
                IsHonorDonor = false
            },
            new Employee
            {
                DepartmentId = 3, // IT-отдел
                PositionId = 3,   // IT-директор
                RoleId = 1,
                FirstName = "Дмитрий",
                LastName = "Козлов",
                MiddleName = "Александрович",
                HireDate = DateOnly.FromDateTime(new DateTime(2021, 3, 1)),
                Email = "it.director@company.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("ITpassword789!"),
                AccumulatedVacationDays = 25,
                IsMultipleChildren = false,
                HasDisabledChild = true,
                IsVeteran = false,
                IsHonorDonor = true
            },
            new Employee
            {
                DepartmentId = 4, // Маркетинг
                PositionId = 4,   // Директор по маркетингу
                RoleId = 1,
                FirstName = "Екатерина",
                LastName = "Волкова",
                MiddleName = "Дмитриевна",
                HireDate = DateOnly.FromDateTime(new DateTime(2022, 7, 22)),
                Email = "marketing.head@company.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("MarketPass2024!"),
                AccumulatedVacationDays = 22,
                IsMultipleChildren = true,
                HasDisabledChild = false,
                IsVeteran = false,
                IsHonorDonor = false
            },

            // Обычные сотрудники (RoleId = 2)
            new Employee
            {
                DepartmentId = 1,
                PositionId = 5,   // Специалист по кадрам
                RoleId = 2,
                FirstName = "Ирина",
                LastName = "Соколова",
                MiddleName = "Андреевна",
                HireDate = DateOnly.FromDateTime(new DateTime(2023, 2, 14)),
                Email = "hr.specialist@company.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("HrSpecialist123!"),
                AccumulatedVacationDays = 20,
                IsMultipleChildren = false,
                HasDisabledChild = false,
                IsVeteran = false,
                IsHonorDonor = false
            },
            new Employee
            {
                DepartmentId = 2,
                PositionId = 6,   // Бухгалтер
                RoleId = 2,
                FirstName = "Алексей",
                LastName = "Морозов",
                MiddleName = "Викторович",
                HireDate = DateOnly.FromDateTime(new DateTime(2023, 8, 5)),
                Email = "accountant@company.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Accountant456!"),
                AccumulatedVacationDays = 18,
                IsMultipleChildren = true,
                HasDisabledChild = true,
                IsVeteran = false,
                IsHonorDonor = true
            },
            new Employee
            {
                DepartmentId = 3,
                PositionId = 7,   // Разработчик
                RoleId = 2,
                FirstName = "Сергей",
                LastName = "Павлов",
                MiddleName = "Олегович",
                HireDate = DateOnly.FromDateTime(new DateTime(2024, 1, 10)),
                Email = "developer@company.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("DevPass789!"),
                AccumulatedVacationDays = 15,
                IsMultipleChildren = false,
                HasDisabledChild = false,
                IsVeteran = false,
                IsHonorDonor = false
            },
            new Employee
            {
                DepartmentId = 4,
                PositionId = 8,   // Маркетолог
                RoleId = 2,
                FirstName = "Анна",
                LastName = "Ковалева",
                MiddleName = "Сергеевна",
                HireDate = DateOnly.FromDateTime(new DateTime(2023, 11, 30)),
                Email = "marketer@company.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("MarketerPass!"),
                AccumulatedVacationDays = 12,
                IsMultipleChildren = false,
                HasDisabledChild = false,
                IsVeteran = true,
                IsHonorDonor = false
            }
        };

        _context.Employees.AddRange(employees);
        _context.SaveChanges();
    }
}
