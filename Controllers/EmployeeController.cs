using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacationPlanner.Api.Models;
using Microsoft.AspNetCore.Authorization;
using BCrypt.Net;
using VacationPlanner.Api.Dtos;
using VacationPlanner.Api.Utils;
using System.ComponentModel.DataAnnotations; 
namespace VacationPlanner.Api.Controllers
{
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly VacationPlannerDbContext _context;
        private readonly UserUtils _userUtils;

        public EmployeesController(VacationPlannerDbContext context)
        {
            _context = context;
            _userUtils = new UserUtils();
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            var userDepartmentId = int.Parse(User.FindFirst("DepartmentId")?.Value);
            return await _context.Employees
                .Where(e => e.DepartmentId == userDepartmentId) // Фильтр по отделу
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.Role)
                .ToListAsync();
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.Role)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            return employee ?? (ActionResult<Employee>)NotFound();
        }

       // В EmployeesController.cs

        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(CreateEmployeeDto createEmployeeDto)
        {   // Получаем DepartmentId из токена
        var userDepartmentId = int.Parse(User.FindFirst("DepartmentId")?.Value);
        createEmployeeDto.DepartmentId = userDepartmentId;
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _context.Departments.AnyAsync(d => d.DepartmentId == createEmployeeDto.DepartmentId))
                return BadRequest("Department not found");

            if (!await _context.Positions.AnyAsync(p => p.PositionId == createEmployeeDto.PositionId))
                return BadRequest("Position not found");

            if (!await _context.Roles.AnyAsync(r => r.RoleId == createEmployeeDto.RoleId))
                return BadRequest("Role not found");

            if (string.IsNullOrWhiteSpace(createEmployeeDto.Email))
                {
                    return BadRequest("Email cannot be null or empty.");
                }


            // Генерация временного пароля
            // string temporaryPassword = _userUtils.GenerateTemporaryPassword();
            
            // Хэширование временного пароля
            // string passwordHash = BCrypt.Net.BCrypt.HashPassword(temporaryPassword);

            // Маппинг из DTO в Employee
            var employee = new Employee
            {
                DepartmentId = createEmployeeDto.DepartmentId,
                PositionId = createEmployeeDto.PositionId,
                FirstName = createEmployeeDto.FirstName,
                LastName = createEmployeeDto.LastName,
                MiddleName = createEmployeeDto.MiddleName,
                HireDate = createEmployeeDto.HireDate,
                Email = createEmployeeDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createEmployeeDto.PasswordHash),
                RoleId = createEmployeeDto.RoleId,
                IsMultipleChildren = createEmployeeDto.IsMultipleChildren,
                HasDisabledChild = createEmployeeDto.HasDisabledChild,
                IsVeteran = createEmployeeDto.IsVeteran,
                IsHonorDonor = createEmployeeDto.IsHonorDonor,
                AccumulatedVacationDays = createEmployeeDto.AccumulatedVacationDays
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // _userUtils.SendEmail(createEmployeeDto.Email, employee.FirstName, createEmployeeDto.PasswordHash);

            return CreatedAtAction("GetEmployee", new { id = employee.EmployeeId }, employee);
        }


        // PUT: api/Employees/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _context.Departments.AnyAsync(d => d.DepartmentId == employee.DepartmentId))
                return BadRequest("Department not found");

            if (!await _context.Positions.AnyAsync(p => p.PositionId == employee.PositionId))
                return BadRequest("Position not found");

            if (!await _context.Roles.AnyAsync(r => r.RoleId == employee.RoleId))
                return BadRequest("Role not found");

            _context.Entry(employee).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if(!EmployeeExists(id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.EmployeeVacationDays)
                .Include(e => e.PlannedVacations)
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
                return NotFound();

            if (employee.EmployeeVacationDays.Any() || employee.PlannedVacations.Any())
                return Conflict("Cannot delete employee with related vacation records");

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Временный эндпоинт для ручного обновления накопленных дней
        [HttpPost("update-vacation-days")]
        public async Task<IActionResult> UpdateVacationDays()
        {
            try
            {
                var employees = await _context.Employees.ToListAsync();
                
                foreach (var employee in employees)
                {
                    var hireDate = employee.HireDate;
                    var currentDate = DateOnly.FromDateTime(DateTime.Now);
                    
                    // Вычисляем количество лет, прошедших с даты трудоустройства
                    var yearsPassed = (currentDate.Year - hireDate.Year) + ((currentDate.Month - hireDate.Month) / 12.0) + ((currentDate.Day - hireDate.Day) / 365.0);
                    
                    // Рассчитываем количество полугодий
                    var halfYears = (int)(yearsPassed * 2);
                    
                    // Устанавливаем накопленные дни (14 дней за каждое полугодие)
                    employee.AccumulatedVacationDays = halfYears * 14;
                    
                    _context.Update(employee);
                }
                
                await _context.SaveChangesAsync();
                
                return Ok("Vacation days updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}