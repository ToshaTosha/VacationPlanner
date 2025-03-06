using VacationPlanner.Api.Models;
using System.Collections.Generic;
using System.Linq;

public class UserSeeder
{
    private readonly VacationPlannerDbContext _context;

    public UserSeeder(VacationPlannerDbContext context)
    {
        _context = context;
    }

    public void Seed()
    {
        if (_context.Users.Any())
        {
            return; // База данных уже заполнена
        }

        var users = new List<User>
        {
            new User
            {
                DepartmentId = 1,
                PositionId = 1,
                FirstName = "Иван",
                LastName = "Тестовый",
                MiddleName = "Иванов",
                HireDate = new DateTime(2025, 3, 6),
                Email = "ivan.test@example.com",
                PasswordHash = "hashed_password_1", 
                IsMultipleChildren = true,
                HasDisabledChild = true,
                IsVeteran = true,
                IsHonorDonor = true
            },
            new User
            {
                DepartmentId = 1,
                PositionId = 2,
                FirstName = "Мария",
                LastName = "Иванова",
                MiddleName = "Петровна",
                HireDate = new DateTime(2025, 4, 10),
                Email = "maria.ivanova@example.com",
                PasswordHash = "hashed_password_2",
                //RoleId = 2,
                IsMultipleChildren = false,
                HasDisabledChild = false,
                IsVeteran = false,
                IsHonorDonor = false
            },
            new User
            {
                DepartmentId = 2,
                PositionId = 3,
                FirstName = "Петр",
                LastName = "Сидоров",
                MiddleName = "Алексеевич",
                HireDate = new DateTime(2025, 5, 15),
                Email = "petr.sidorov@example.com",
                PasswordHash = "hashed_password_3",
                IsMultipleChildren = true,
                HasDisabledChild = false,
                IsVeteran = true,
                IsHonorDonor = false
            }
        };

        _context.Users.AddRange(users);
        _context.SaveChanges();
    }
}
