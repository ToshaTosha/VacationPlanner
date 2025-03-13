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
            if (_context.Employees.Any())
            {
                return; // База данных уже заполнена
            }

            var employees = new List<Employee>
            {
                new Employee
                {
                    DepartmentId = 1,
                    PositionId = 1,
                    RoleId = 2,
                    FirstName = "Иван",
                    LastName = "Тестовый",
                    MiddleName = "Иванов",
                    HireDate = DateOnly.FromDateTime(new DateTime(2025, 3, 6)),
                    Email = "ivan.test@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("12345"), 
                    IsMultipleChildren = true,
                    HasDisabledChild = true,
                    IsVeteran = true,
                    IsHonorDonor = true
                },
                new Employee
                {
                    DepartmentId = 1,
                    PositionId = 2,
                    RoleId = 2,
                    FirstName = "Мария",
                    LastName = "Иванова",
                    MiddleName = "Петровна",
                    HireDate = DateOnly.FromDateTime(new DateTime(2025, 4, 10)), 
                    Email = "maria.ivanova@example.com",
                    PasswordHash = "hashed_password_2",
                    IsMultipleChildren = false,
                    HasDisabledChild = false,
                    IsVeteran = false,
                    IsHonorDonor = false
                },
                new Employee
                {
                    DepartmentId = 2,
                    PositionId = 3,
                    RoleId = 2,
                    FirstName = "Петр",
                    LastName = "Сидоров",
                    MiddleName = "Алексеевич",
                    HireDate = DateOnly.FromDateTime(new DateTime(2025, 5, 15)), 
                    Email = "petr.sidorov@example.com",
                    PasswordHash = "hashed_password_3",
                    IsMultipleChildren = true,
                    HasDisabledChild = false,
                    IsVeteran = true,
                    IsHonorDonor = false
                }
            };

            _context.Employees.AddRange(employees);
            _context.SaveChanges();
        }
    }
