namespace VacationPlanner.Api.DTOs
{
    public class EmployeeVacationDayDto
    {
        public int EmployeeId { get; set; }
        public int VacationTypeId { get; set; }
        public int DaysCount { get; set; }
    }
}