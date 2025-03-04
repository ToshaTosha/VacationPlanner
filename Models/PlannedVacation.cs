using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VacationPlanner.Api.Models;

public partial class PlannedVacation
{
    [Key]
    public int PlannedVacationId { get; set; }

    public int EmployeeId { get; set; }

    public int VacationTypeId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

    [StringLength(500)]
    public string? Comment { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("PlannedVacations")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("VacationTypeId")]
    [InverseProperty("PlannedVacations")]
    public virtual VacationType VacationType { get; set; } = null!;
}
