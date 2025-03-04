﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VacationPlanner.Api.Models;

#nullable disable

namespace VacationPlanner.Api.Migrations
{
    [DbContext(typeof(VacationPlannerDbContext))]
    partial class VacationPlannerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("VacationPlanner.Api.Models.AdditionalVacationDay", b =>
                {
                    b.Property<int>("AdditionalVacationDaysId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AdditionalVacationDaysId"));

                    b.Property<int>("DaysCount")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("AdditionalVacationDaysId")
                        .HasName("PK__Addition__609164D8D44CE74C");

                    b.ToTable("AdditionalVacationDays");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.Department", b =>
                {
                    b.Property<int>("DepartmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DepartmentId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("OrganizationId")
                        .HasColumnType("int");

                    b.HasKey("DepartmentId")
                        .HasName("PK__Departme__B2079BEDA328D9C5");

                    b.HasIndex("OrganizationId");

                    b.ToTable("Departments");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.DepartmentRestriction", b =>
                {
                    b.Property<int>("DepartmentRestrictionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DepartmentRestrictionId"));

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int");

                    b.Property<int>("RestrictionId")
                        .HasColumnType("int");

                    b.HasKey("DepartmentRestrictionId")
                        .HasName("PK__Departme__ABCAF3CDD5199EFE");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("RestrictionId");

                    b.ToTable("DepartmentRestrictions");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.Employee", b =>
                {
                    b.Property<int>("EmployeeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EmployeeId"));

                    b.Property<int>("DepartmentId")
                        .HasColumnType("int");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool?>("HasDisabledChild")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<DateOnly>("HireDate")
                        .HasColumnType("date");

                    b.Property<bool?>("IsHonorDonor")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<bool?>("IsMultipleChildren")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<bool?>("IsVeteran")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("MiddleName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PositionId")
                        .HasColumnType("int");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.HasKey("EmployeeId")
                        .HasName("PK__Employee__7AD04F1107E6E4F2");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("PositionId");

                    b.HasIndex("RoleId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.EmployeeVacationDay", b =>
                {
                    b.Property<int>("EmployeeVacationDaysId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("EmployeeVacationDaysId"));

                    b.Property<int>("DaysCount")
                        .HasColumnType("int");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<int>("VacationTypeId")
                        .HasColumnType("int");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("EmployeeVacationDaysId")
                        .HasName("PK__Employee__2D6E62F473F37097");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("VacationTypeId");

                    b.ToTable("EmployeeVacationDays");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.Holiday", b =>
                {
                    b.Property<int>("HolidayId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("HolidayId"));

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("HolidayId")
                        .HasName("PK__Holidays__2D35D57AEAB79E08");

                    b.ToTable("Holidays");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.Organization", b =>
                {
                    b.Property<int>("OrganizationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OrganizationId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("OrganizationId")
                        .HasName("PK__Organiza__CADB0B12B6A1759C");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.PlannedVacation", b =>
                {
                    b.Property<int>("PlannedVacationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PlannedVacationId"));

                    b.Property<string>("Comment")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<DateOnly>("EndDate")
                        .HasColumnType("date");

                    b.Property<DateOnly>("StartDate")
                        .HasColumnType("date");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("VacationTypeId")
                        .HasColumnType("int");

                    b.HasKey("PlannedVacationId")
                        .HasName("PK__PlannedV__1C230134C01B9D4D");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("VacationTypeId");

                    b.ToTable("PlannedVacations");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.Position", b =>
                {
                    b.Property<int>("PositionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PositionId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("PositionId")
                        .HasName("PK__Position__60BB9A7994A6A7E7");

                    b.ToTable("Positions");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.Restriction", b =>
                {
                    b.Property<int>("RestrictionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RestrictionId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("RestrictionId")
                        .HasName("PK__Restrict__529D86BA45717F5E");

                    b.ToTable("Restrictions");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RoleId"));

                    b.Property<string>("NameRole")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("RoleId")
                        .HasName("PK_Role");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.VacationType", b =>
                {
                    b.Property<int>("VacationTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("VacationTypeId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("VacationTypeId")
                        .HasName("PK__Vacation__22E546763BF5943B");

                    b.ToTable("VacationTypes");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.Department", b =>
                {
                    b.HasOne("VacationPlanner.Api.Models.Organization", "Organization")
                        .WithMany("Departments")
                        .HasForeignKey("OrganizationId")
                        .IsRequired()
                        .HasConstraintName("FK__Departmen__Organ__4BAC3F29");

                    b.Navigation("Organization");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.DepartmentRestriction", b =>
                {
                    b.HasOne("VacationPlanner.Api.Models.Department", "Department")
                        .WithMany("DepartmentRestrictions")
                        .HasForeignKey("DepartmentId")
                        .IsRequired()
                        .HasConstraintName("FK__Departmen__Depar__6383C8BA");

                    b.HasOne("VacationPlanner.Api.Models.Restriction", "Restriction")
                        .WithMany("DepartmentRestrictions")
                        .HasForeignKey("RestrictionId")
                        .IsRequired()
                        .HasConstraintName("FK__Departmen__Restr__6477ECF3");

                    b.Navigation("Department");

                    b.Navigation("Restriction");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.Employee", b =>
                {
                    b.HasOne("VacationPlanner.Api.Models.Department", "Department")
                        .WithMany("Employees")
                        .HasForeignKey("DepartmentId")
                        .IsRequired()
                        .HasConstraintName("FK__Employees__Depar__5441852A");

                    b.HasOne("VacationPlanner.Api.Models.Position", "Position")
                        .WithMany("Employees")
                        .HasForeignKey("PositionId")
                        .IsRequired()
                        .HasConstraintName("FK__Employees__Posit__5535A963");

                    b.HasOne("VacationPlanner.Api.Models.Role", "Role")
                        .WithMany("Employees")
                        .HasForeignKey("RoleId")
                        .IsRequired()
                        .HasConstraintName("FK_Employees_Roles");

                    b.Navigation("Department");

                    b.Navigation("Position");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.EmployeeVacationDay", b =>
                {
                    b.HasOne("VacationPlanner.Api.Models.Employee", "Employee")
                        .WithMany("EmployeeVacationDays")
                        .HasForeignKey("EmployeeId")
                        .IsRequired()
                        .HasConstraintName("FK__EmployeeV__Emplo__59FA5E80");

                    b.HasOne("VacationPlanner.Api.Models.VacationType", "VacationType")
                        .WithMany("EmployeeVacationDays")
                        .HasForeignKey("VacationTypeId")
                        .IsRequired()
                        .HasConstraintName("FK__EmployeeV__Vacat__5AEE82B9");

                    b.Navigation("Employee");

                    b.Navigation("VacationType");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.PlannedVacation", b =>
                {
                    b.HasOne("VacationPlanner.Api.Models.Employee", "Employee")
                        .WithMany("PlannedVacations")
                        .HasForeignKey("EmployeeId")
                        .IsRequired()
                        .HasConstraintName("FK__PlannedVa__Emplo__6754599E");

                    b.HasOne("VacationPlanner.Api.Models.VacationType", "VacationType")
                        .WithMany("PlannedVacations")
                        .HasForeignKey("VacationTypeId")
                        .IsRequired()
                        .HasConstraintName("FK__PlannedVa__Vacat__68487DD7");

                    b.Navigation("Employee");

                    b.Navigation("VacationType");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.Department", b =>
                {
                    b.Navigation("DepartmentRestrictions");

                    b.Navigation("Employees");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.Employee", b =>
                {
                    b.Navigation("EmployeeVacationDays");

                    b.Navigation("PlannedVacations");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.Organization", b =>
                {
                    b.Navigation("Departments");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.Position", b =>
                {
                    b.Navigation("Employees");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.Restriction", b =>
                {
                    b.Navigation("DepartmentRestrictions");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.Role", b =>
                {
                    b.Navigation("Employees");
                });

            modelBuilder.Entity("VacationPlanner.Api.Models.VacationType", b =>
                {
                    b.Navigation("EmployeeVacationDays");

                    b.Navigation("PlannedVacations");
                });
#pragma warning restore 612, 618
        }
    }
}
