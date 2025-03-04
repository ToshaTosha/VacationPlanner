using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacationPlanner.Api.Models;
using VacationPlanner.Api.DTOs;
namespace VacationPlanner.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeVacationDaysController : ControllerBase
    {
        private readonly VacationPlannerDbContext _context;

        public EmployeeVacationDaysController(VacationPlannerDbContext context)
        {
            _context = context;
        }

        // GET: api/EmployeeVacationDays
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeVacationDay>>> GetEmployeeVacationDays()
        {
            return await _context.EmployeeVacationDays
                .Include(evd => evd.Employee)
                .Include(evd => evd.VacationType)
                .ToListAsync();
        }

        // GET: api/EmployeeVacationDays/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeVacationDay>> GetEmployeeVacationDay(int id)
        {
            var employeeVacationDay = await _context.EmployeeVacationDays
                .Include(evd => evd.Employee)
                .Include(evd => evd.VacationType)
                .FirstOrDefaultAsync(evd => evd.EmployeeVacationDaysId == id);

            return employeeVacationDay ?? (ActionResult<EmployeeVacationDay>)NotFound();
        }

        // POST: api/EmployeeVacationDays
        [HttpPost]
public async Task<ActionResult<EmployeeVacationDay>> PostEmployeeVacationDay(EmployeeVacationDayDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    if (!await _context.Employees.AnyAsync(e => e.EmployeeId == dto.EmployeeId))
        return BadRequest("Employee not found");

    if (!await _context.VacationTypes.AnyAsync(vt => vt.VacationTypeId == dto.VacationTypeId))
        return BadRequest("VacationType not found");

    if (dto.DaysCount <= 0)
        return BadRequest("DaysCount must be positive");

    // Создаем новый объект EmployeeVacationDay
    var employeeVacationDay = new EmployeeVacationDay
    {
        EmployeeId = dto.EmployeeId,
        VacationTypeId = dto.VacationTypeId,
        DaysCount = dto.DaysCount
    };

    _context.EmployeeVacationDays.Add(employeeVacationDay);
    await _context.SaveChangesAsync();

    // Загружаем связанные объекты для возврата полного объекта
    await _context.Entry(employeeVacationDay)
        .Reference(evd => evd.Employee)
        .LoadAsync();
    await _context.Entry(employeeVacationDay)
        .Reference(evd => evd.VacationType)
        .LoadAsync();

    return CreatedAtAction("GetEmployeeVacationDay", 
        new { id = employeeVacationDay.EmployeeVacationDaysId }, employeeVacationDay);
}

// Аналогично измените метод PUT
[HttpPut("{id}")]
public async Task<IActionResult> PutEmployeeVacationDay(int id, EmployeeVacationDayDto dto)
{
    var existingRecord = await _context.EmployeeVacationDays.FindAsync(id);
    if (existingRecord == null)
        return NotFound();

    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    if (!await _context.Employees.AnyAsync(e => e.EmployeeId == dto.EmployeeId))
        return BadRequest("Employee not found");

    if (!await _context.VacationTypes.AnyAsync(vt => vt.VacationTypeId == dto.VacationTypeId))
        return BadRequest("VacationType not found");

    if (dto.DaysCount <= 0)
        return BadRequest("DaysCount must be positive");

    // Обновляем существующую запись
    existingRecord.EmployeeId = dto.EmployeeId;
    existingRecord.VacationTypeId = dto.VacationTypeId;
    existingRecord.DaysCount = dto.DaysCount;

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException)
    {
        if (!EmployeeVacationDayExists(id))
            return NotFound();
        throw;
    }

    return NoContent();
}

        // DELETE: api/EmployeeVacationDays/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeVacationDay(int id)
        {
            var employeeVacationDay = await _context.EmployeeVacationDays.FindAsync(id);
            if (employeeVacationDay == null)
                return NotFound();

            _context.EmployeeVacationDays.Remove(employeeVacationDay);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeVacationDayExists(int id)
        {
            return _context.EmployeeVacationDays.Any(e => e.EmployeeVacationDaysId == id);
        }
    }
}