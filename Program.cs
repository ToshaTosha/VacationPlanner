using Microsoft.EntityFrameworkCore;
using VacationPlanner.Api.Models;
using System.Text.Json.Serialization;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

// Добавление сервисов
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManagerOnly", policy => 
        policy.RequireRole("Manager"));
    
    options.AddPolicy("EmployeeOnly", policy => 
        policy.RequireRole("Employee"));
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.MaxDepth = 128; // При необходимости увеличьте глубину
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Добавьте контекст БД
builder.Services.AddDbContext<VacationPlannerDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Настройка middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

DotNetEnv.Env.Load();
app.UseCors("AllowAllOrigins");
app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseAuthentication();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<VacationPlannerDbContext>();

    var vacationTypeSeeder = new VacationTypeSeeder(context);
    vacationTypeSeeder.Seed();

    var vacationStatusSeeder = new VacationStatusSeeder(context);
    vacationStatusSeeder.Seed();

    var organizationSeeder = new OrganizationSeeder(context);
    organizationSeeder.Seed();

    var departmentSeeder = new DepartmentSeeder(context);
    departmentSeeder.Seed();

    var positionSeeder = new PositionSeeder(context);
    positionSeeder.Seed();

    var roleSeeder = new RoleSeeder(context);
    roleSeeder.Seed();

    var employeeSeeder = new EmployeeSeeder(context);
    employeeSeeder.Seed();
}

app.Run();
