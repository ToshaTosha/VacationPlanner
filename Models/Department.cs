using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace VacationPlanner.Api.Models;

public partial class Department
{
    [Key]
    public int DepartmentId { get; set; }

    public int OrganizationId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [InverseProperty("Department")]
    public virtual ICollection<DepartmentRestriction> DepartmentRestrictions { get; set; } = new List<DepartmentRestriction>();

    [InverseProperty("Department")]
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    [ForeignKey("OrganizationId")]
    [InverseProperty("Departments")]
    public virtual Organization Organization { get; set; } = null!;
}
