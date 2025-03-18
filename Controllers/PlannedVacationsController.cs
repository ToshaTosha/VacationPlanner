using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacationPlanner.Api.Dtos;
using VacationPlanner.Api.Models;

namespace VacationPlanner.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PlannedVacationController : ControllerBase
    {
        private readonly VacationPlannerDbContext _context;

        public PlannedVacationController(VacationPlannerDbContext context)
        {
            _context = context;
        }

        // Создать новую заявку (для сотрудников)
        [HttpPost]
        public async Task<ActionResult<PlannedVacation>> CreatePlannedVacation(CreatePlannedVacationDto dto)
        {
            // Проверка наличия EmployeeId в токене
            var employeeIdClaim = User.FindFirst("EmployeeId");
            if (employeeIdClaim == null || !int.TryParse(employeeIdClaim.Value, out int employeeId))
            {
                return Unauthorized("Invalid employee identifier");
            }

            var vacation = new PlannedVacation
            {
                EmployeeId = employeeId,
                VacationTypeId = dto.VacationTypeId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Comment = dto.Comment,
                VacationStatusId = 1 // Статус "Запланирован"
            };

            _context.PlannedVacations.Add(vacation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlannedVacation), 
                new { id = vacation.PlannedVacationId }, vacation);
        }

        // Получить заявку по ID (публичный метод)
        [HttpGet("{id}")]
        public async Task<ActionResult<PlannedVacation>> GetPlannedVacation(int id)
        {
            var vacation = await _context.PlannedVacations
                .Include(p => p.Employee)
                .Include(p => p.VacationType)
                .Include(p => p.VacationStatus)
                .FirstOrDefaultAsync(p => p.PlannedVacationId == id);

            if (vacation == null)
            {
                return NotFound();
            }

            // Проверка прав доступа
            var isManager = User.IsInRole("Руководитель");
            var currentEmployeeId = int.Parse(User.FindFirst("EmployeeId")!.Value);
            
            if (!isManager && vacation.EmployeeId != currentEmployeeId)
            {
                return Forbid();
            }

            return Ok(vacation);
        }

        // Получить все заявки (только для руководителей)
        [Authorize(Policy = "RequireManagerRole")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlannedVacation>>> GetAllPlannedVacations()
        {
            return await _context.PlannedVacations
                .Include(p => p.Employee)
                .Include(p => p.VacationType)
                .Include(p => p.VacationStatus)
                .ToListAsync();
        }

        // Обновить статус заявки (только для руководителей)
        [Authorize(Policy = "RequireManagerRole")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateVacationStatus(int id, UpdateVacationStatusDto dto)
        {
            var vacation = await _context.PlannedVacations
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(p => p.PlannedVacationId == id);

            if (vacation == null) 
            {
                return NotFound();
            }

            vacation.VacationStatusId = dto.VacationStatusId;
            vacation.Comment = dto.Comment;

            if (dto.VacationStatusId == 2) // Подтвержден
            {
                var daysCount = (vacation.EndDate.DayNumber - vacation.StartDate.DayNumber) + 1;
                var year = vacation.StartDate.Year;

                var vacationDays = new EmployeeVacationDay
                {
                    EmployeeId = vacation.EmployeeId,
                    VacationTypeId = vacation.VacationTypeId,
                    DaysCount = daysCount,
                    Year = year
                };

                _context.EmployeeVacationDays.Add(vacationDays);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Получить заявки текущего пользователя
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<PlannedVacation>>> GetMyVacations()
        {
            var employeeId = int.Parse(User.FindFirst("EmployeeId")!.Value);
            
            return await _context.PlannedVacations
                .Where(p => p.EmployeeId == employeeId)
                .Include(p => p.VacationType)
                .Include(p => p.VacationStatus)
                .ToListAsync();
        }

        // Получить все подтвержденные заявки
        [HttpGet("approved")]
        public async Task<ActionResult<IEnumerable<PlannedVacation>>> GetApprovedVacations()
        {
            return await _context.PlannedVacations
                .Where(p => p.VacationStatusId == 2)
                .Include(p => p.Employee)
                .Include(p => p.VacationType)
                .ToListAsync();
        }
    }
}
