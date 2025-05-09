using Microsoft.EntityFrameworkCore;
using VacationPlanner.Api.Models;
using System.Text.Json.Serialization;
using DotNetEnv;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using VacationPlanner.Api.Services;
var builder = WebApplication.CreateBuilder(args);

// Загрузка переменных окружения
DotNetEnv.Env.Load();

// Настройка аутентификации JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// Настройка CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// Настройка авторизации
builder.Services.AddAuthorization(options =>
{
   options.AddPolicy("RequireManagerRole", policy =>
        policy.RequireClaim("RoleId", "1"));

    options.AddPolicy("EmployeeOnly",
        policy => policy.RequireClaim("RoleId", "2"));
});

// Настройка контроллеров
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.MaxDepth = 128;
    });

// Настройка Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { 
        Title = "Vacation Planner API", 
        Version = "v1",
        Description = "API for managing employee vacations"
    });
    
    // Добавление поддержки JWT в Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Настройка базы данных
builder.Services.AddDbContext<VacationPlannerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.CommandTimeout(500); 
        }));

    

// Регистрация сервисов
builder.Services.AddScoped<VacationPlannerDbContext>();
builder.Services.AddScoped<VacationTransferService>(); // ✅ Scoped вместо Singleton
builder.Services.AddSingleton<IHostedService, VacationTransferBackgroundService>();
builder.Services.AddSingleton<IHostedService, VacationDaysUpdateService>(); // Изменено на Singleton

var app = builder.Build();

// Конфигурация middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vacation Planner API v1");
        c.OAuthClientId("swagger-ui");
        c.OAuthAppName("Swagger UI");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Инициализация базы данных
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<VacationPlannerDbContext>();
    
    // Запуск сидеров
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