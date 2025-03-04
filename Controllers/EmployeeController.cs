using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacationPlanner.Api.Models;
using Microsoft.AspNetCore.Authorization;
using BCrypt.Net;

namespace VacationPlanner.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly VacationPlannerDbContext _context;

        public EmployeesController(VacationPlannerDbContext context)
        {
            _context = context;
        }

        // GET: api/Employees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.Role) // Включаем роль
                .ToListAsync();
        }

        // GET: api/Employees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .Include(e => e.Role) // Включаем роль
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            return employee ?? (ActionResult<Employee>)NotFound();
        }

        // POST: api/Employees
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await _context.Departments.AnyAsync(d => d.DepartmentId == employee.DepartmentId))
                return BadRequest("Department not found");

            if (!await _context.Positions.AnyAsync(p => p.PositionId == employee.PositionId))
                return BadRequest("Position not found");

            if (!await _context.Roles.AnyAsync(r => r.RoleId == employee.RoleId))
                return BadRequest("Role not found");

            // Хешируем пароль, если он передается в открытом виде
            if (!string.IsNullOrEmpty(employee.PasswordHash))
            {
                employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(employee.PasswordHash);
            }

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

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
                if (!EmployeeExists(id))
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

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }
    }
}
