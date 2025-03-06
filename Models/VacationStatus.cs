using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VacationPlanner.Api.Models;

public partial class VacationStatus
{
    [Key]
    public int VacationStatusId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [InverseProperty("VacationStatus")]
    public virtual ICollection<PlannedVacation> PlannedVacations { get; set; } = new List<PlannedVacation>();
}
