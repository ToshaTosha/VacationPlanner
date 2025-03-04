using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VacationPlanner.Api.Models;

public partial class Employee
{
    [Key]
    public int EmployeeId { get; set; }

    public int DepartmentId { get; set; }

    public int PositionId { get; set; }

    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [StringLength(50)]
    public string? MiddleName { get; set; }

    public DateOnly HireDate { get; set; }

    public bool? IsMultipleChildren { get; set; }

    public bool? HasDisabledChild { get; set; }

    public bool? IsVeteran { get; set; }

    public bool? IsHonorDonor { get; set; }
     [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;
     [Required]
    public string PasswordHash { get; set; } = null!;

    [ForeignKey("Role")]
    public int RoleId { get; set; }

    [ForeignKey("DepartmentId")]
    [InverseProperty("Employees")]
    public virtual Department Department { get; set; } = null!;

    [InverseProperty("Employee")]
    public virtual ICollection<EmployeeVacationDay> EmployeeVacationDays { get; set; } = new List<EmployeeVacationDay>();

    [InverseProperty("Employee")]
    public virtual ICollection<PlannedVacation> PlannedVacations { get; set; } = new List<PlannedVacation>();

    [ForeignKey("PositionId")]
    [InverseProperty("Employees")]
    public virtual Position Position { get; set; } = null!;
     public virtual Role Role { get; set; } = null!;
}
