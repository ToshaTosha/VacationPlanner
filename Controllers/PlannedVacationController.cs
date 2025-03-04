using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacationPlanner.Api.Models;

namespace VacationPlanner.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlannedVacationsController : ControllerBase
    {
        private readonly VacationPlannerDbContext _context;

        public PlannedVacationsController(VacationPlannerDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlannedVacation>>> GetPlannedVacations()
        {
            return await _context.PlannedVacations
                .Include(pv => pv.Employee)
                .Include(pv => pv.VacationType)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PlannedVacation>> GetPlannedVacation(int id)
        {
            var plannedVacation = await _context.PlannedVacations
                .Include(pv => pv.Employee)
                .Include(pv => pv.VacationType)
                .FirstOrDefaultAsync(pv => pv.PlannedVacationId == id);

            return plannedVacation ?? (ActionResult<PlannedVacation>)NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PlannedVacation>> PostPlannedVacation(PlannedVacation plannedVacation)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            if (!await _context.Employees.AnyAsync(e => e.EmployeeId == plannedVacation.EmployeeId))
                return BadRequest("Employee not found");

            if (!await _context.VacationTypes.AnyAsync(vt => vt.VacationTypeId == plannedVacation.VacationTypeId))
                return BadRequest("Vacation type not found");

            if (plannedVacation.StartDate >= plannedVacation.EndDate)
                return BadRequest("End date must be after start date");

            _context.PlannedVacations.Add(plannedVacation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlannedVacation", 
                new { id = plannedVacation.PlannedVacationId }, plannedVacation);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlannedVacation(int id, PlannedVacation plannedVacation)
        {
            if (id != plannedVacation.PlannedVacationId) return BadRequest();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Entry(plannedVacation).State = EntityState.Modified;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlannedVacationExists(id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlannedVacation(int id)
        {
            var plannedVacation = await _context.PlannedVacations.FindAsync(id);
            if (plannedVacation == null) return NotFound();

            _context.PlannedVacations.Remove(plannedVacation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlannedVacationExists(int id) => _context.PlannedVacations.Any(e => e.PlannedVacationId == id);
    }
}