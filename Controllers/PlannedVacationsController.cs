using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacationPlanner.Api.Dtos;
using VacationPlanner.Api.Models;
using System.Security.Claims;

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
            // Получаем данные из токена
            var employeeId = int.Parse(User.FindFirst("EmployeeId")?.Value);
            var userDepartmentId = int.Parse(User.FindFirst("DepartmentId")?.Value);

            // Получаем сотрудника
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
            
            if (employee == null) return NotFound("Employee not found");

            // Проверка принадлежности сотрудника к отделу
            if (employee.DepartmentId != userDepartmentId)
                return Forbid("Доступ запрещен к данному отделу");

            // Вычисляем количество дней в заявке
            var daysCount = (dto.EndDate.DayNumber - dto.StartDate.DayNumber) + 1;

            // Проверка 1: Минимум 7 дней после отпуска
            if (employee.AccumulatedVacationDays - daysCount < 7)
            {
                return BadRequest(new { 
                    error = "Недостаточно дней для отпуска",
                    details = new {
                        available = employee.AccumulatedVacationDays,
                        requested = daysCount,
                        remaining = employee.AccumulatedVacationDays - daysCount
                    }
                });
            }

            // Проверка 2: Наличие длинного отпуска (14+ дней)
            var approvedVacations = await _context.PlannedVacations
                .Where(p => p.EmployeeId == employeeId && p.VacationStatusId == 2)
                .ToListAsync();

            var hasLongVacation = approvedVacations.Any(v => 
                (v.EndDate.DayNumber - v.StartDate.DayNumber) + 1 >= 14);

            // Основная логика обработки заявки
            if (daysCount >= 14)
            {
                return await ProcessLongVacation(dto, employee, userDepartmentId);
            }
            else
            {
                return await ProcessShortVacation(dto, employee, userDepartmentId, hasLongVacation, daysCount);
            }
        }

        // Получить заявку по ID
        [HttpGet("{id}")]
        public async Task<ActionResult<PlannedVacation>> GetPlannedVacation(int id)
        {
            var vacation = await _context.PlannedVacations
                .Include(p => p.Employee)
                .Include(p => p.VacationType)
                .Include(p => p.VacationStatus)
                .FirstOrDefaultAsync(p => p.PlannedVacationId == id);

            if (vacation == null) return NotFound();

            // Проверка прав доступа
            var currentEmployeeId = int.Parse(User.FindFirst("EmployeeId")!.Value);
            var currentDepartmentId = int.Parse(User.FindFirst("DepartmentId")!.Value);
            var isManager = User.IsInRole("Руководитель");

            if (!isManager && vacation.EmployeeId != currentEmployeeId)
                return Forbid();

            // Проверка отдела для менеджера
            if (isManager && vacation.Employee.DepartmentId != currentDepartmentId)
                return Forbid("Доступ к заявке другого отдела запрещен");

            return Ok(vacation);
        }

        // Получить все заявки отдела (для руководителей)
        [Authorize(Policy = "RequireManagerRole")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlannedVacation>>> GetAllPlannedVacations()
        {
            var departmentId = int.Parse(User.FindFirst("DepartmentId")?.Value);
            
            return await _context.PlannedVacations
                .Include(p => p.Employee)
                .Where(p => p.Employee.DepartmentId == departmentId)
                .Include(p => p.VacationType)
                .Include(p => p.VacationStatus)
                .ToListAsync();
        }

        // Обновить статус заявки
        [Authorize(Policy = "RequireManagerRole")]
[HttpPut("{id}/status")]
public async Task<IActionResult> UpdateVacationStatus(
    int id, 
    [FromBody] UpdateVacationStatusDto dto) // Добавлен [FromBody]
{
    // 1. Проверка принадлежности к отделу
    var departmentId = int.Parse(User.FindFirst("DepartmentId")?.Value);
    
    var vacation = await _context.PlannedVacations
        .Include(v => v.Employee)
        .FirstOrDefaultAsync(v => v.PlannedVacationId == id);

    if (vacation == null) return NotFound();
    
    if (vacation.Employee.DepartmentId != departmentId)
        return Forbid("Нет прав для управления заявками этого отдела");

    // 2. Сохраняем оригинальный статус для проверки изменений
    var originalStatus = vacation.VacationStatusId;

    // 3. Обновляем статус и комментарий
    vacation.VacationStatusId = dto.VacationStatusId;
    vacation.Comment = dto.Comment;

    // 4. Обработка изменения статуса на "Одобрено"
    if (dto.VacationStatusId == 2)
    {
        var daysCount = (vacation.EndDate.DayNumber - vacation.StartDate.DayNumber) + 1;
        
        if (vacation.Employee.AccumulatedVacationDays < daysCount)
            return BadRequest("Недостаточно дней");

        // Обновляем накопленные дни
        vacation.Employee.AccumulatedVacationDays -= daysCount;
        _context.Update(vacation.Employee);

        // Добавляем запись в EmployeeVacationDays
        if (!await _context.EmployeeVacationDays.AnyAsync(e => 
            e.EmployeeId == vacation.EmployeeId &&
            e.StartDate == vacation.StartDate &&
            e.EndDate == vacation.EndDate))
        {
            _context.EmployeeVacationDays.Add(new EmployeeVacationDay
            {
                EmployeeId = vacation.EmployeeId,
                VacationTypeId = vacation.VacationTypeId,
                StartDate = vacation.StartDate,
                EndDate = vacation.EndDate
            });
        }
    }

    // 5. Обработка отмены утвержденного статуса
    else if (originalStatus == 2)
    {
        var daysCount = (vacation.EndDate.DayNumber - vacation.StartDate.DayNumber) + 1;
        vacation.Employee.AccumulatedVacationDays += daysCount;
        _context.Update(vacation.Employee);
    }

    try
    {
        await _context.SaveChangesAsync();
    }
    catch (DbUpdateConcurrencyException ex)
    {
        // Логирование ошибки
        return StatusCode(500, "Ошибка при сохранении данных");
    }

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

        // Вспомогательные методы
        private async Task<ActionResult<PlannedVacation>> ProcessLongVacation(
            CreatePlannedVacationDto dto, 
            Employee employee,
            int departmentId)
        {
            // Проверка доступных дней
            var daysCount = (dto.EndDate.DayNumber - dto.StartDate.DayNumber) + 1;
            if (employee.AccumulatedVacationDays < daysCount)
            {
                return BadRequest(new { 
                    error = "Недостаточно накопленных дней",
                    details = new {
                        available = employee.AccumulatedVacationDays,
                        requested = daysCount
                    }
                });
            }

            return await CreateVacationRequest(dto, employee, departmentId);
        }

        private async Task<ActionResult<PlannedVacation>> ProcessShortVacation(
            CreatePlannedVacationDto dto,
            Employee employee,
            int departmentId,
            bool hasLongVacation,
            int daysCount)
        {
            if (!hasLongVacation && (employee.AccumulatedVacationDays - daysCount < 14))
            {
                return BadRequest(new { 
                    error = "Требуется отпуск не менее 14 дней",
                    details = new {
                        available = employee.AccumulatedVacationDays,
                        requested = daysCount,
                        remaining = employee.AccumulatedVacationDays - daysCount
                    }
                });
            }

            return await CreateVacationRequest(dto, employee, departmentId);
        }

        private async Task<ActionResult<PlannedVacation>> CreateVacationRequest(
            CreatePlannedVacationDto dto,
            Employee employee,
            int departmentId)
        {
            // Создание заявки
            var vacation = new PlannedVacation
            {
                EmployeeId = employee.EmployeeId,
                VacationTypeId = dto.VacationTypeId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Comment = dto.Comment,
                VacationStatusId = 1
            };

            _context.PlannedVacations.Add(vacation);
            await _context.SaveChangesAsync();

            // Поиск руководителя отдела
            var manager = await _context.Employees
                .FirstOrDefaultAsync(e => 
                    e.RoleId == 1 && 
                    e.DepartmentId == departmentId);

            if (manager == null)
                return StatusCode(500, "Руководитель отдела не найден");

            // Создание уведомления
            var notification = new Notification
            {
                EmployeeId = manager.EmployeeId,
                Message = $"Новая заявка от {employee.FirstName} {employee.LastName}",
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                RelatedVacationId = vacation.PlannedVacationId,
                IsManagerNotification = true
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlannedVacation), 
                new { id = vacation.PlannedVacationId }, vacation);
        }
    }
}