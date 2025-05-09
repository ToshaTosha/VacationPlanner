using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationPlanner.Api.Models;

namespace VacationPlanner.Controllers
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

        // GET: api/EmployeeVacationDays/department/{departmentId}
        [HttpGet("department/{departmentId}")]
        public async Task<ActionResult<IEnumerable<EmployeeVacationDay>>> GetEmployeeVacationDaysByDepartment(int departmentId)
        {
            return await _context.EmployeeVacationDays
                .Include(evd => evd.Employee)
                .Include(evd => evd.VacationType)
                .Where(evd => evd.Employee.DepartmentId == departmentId)
                .ToListAsync();
        }
    }
} 