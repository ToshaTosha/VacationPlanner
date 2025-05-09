using Microsoft.EntityFrameworkCore;
using VacationPlanner.Api.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace VacationPlanner.Api.Services 
{
    public class VacationDaysUpdateService : IHostedService, IDisposable
    {
        private readonly ILogger<VacationDaysUpdateService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private Timer? _timer;

        public VacationDaysUpdateService(
            ILogger<VacationDaysUpdateService> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Vacation Days Update Service is starting.");

            // Запускаем таймер, который будет вызывать обновление каждый день
            _timer = new Timer(UpdateVacationDays, null, 0, (int)TimeSpan.FromDays(1).TotalMilliseconds);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Vacation Days Update Service is stopping.");

            // Останавливаем таймер
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private void UpdateVacationDays(object? state)
        {
            _ = UpdateVacationDaysAsync(CancellationToken.None); // Запускаем асинхронный метод
        }
        
        private async Task UpdateVacationDaysAsync(CancellationToken cancellationToken)
        {
            try 
            {
                _logger.LogInformation("Updating vacation days...");

                // Создаем новый scope для каждой операции
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<VacationPlannerDbContext>();
                    
                    // Получаем всех сотрудников с их одобренными заявками на отпуск
                    var employees = await dbContext.Employees
                        .Include(e => e.PlannedVacations.Where(pv => pv.VacationStatusId == 2)) // 2 - статус "Одобрено"
                        .ToListAsync(cancellationToken);

                    // Обновляем значение AccumulatedVacationDays для каждого сотрудника
                    foreach (var employee in employees)
                    {
                        var hireDate = employee.HireDate;
                        var currentDate = DateOnly.FromDateTime(DateTime.Now);

                        // Вычисляем количество лет, прошедших с даты трудоустройства
                        var yearsPassed = (currentDate.Year - hireDate.Year) + ((currentDate.Month - hireDate.Month) / 12.0) + ((currentDate.Day - hireDate.Day) / 365.0);

                        // Обновляем значение AccumulatedVacationDays только для сотрудников с полными годами/полугодиями
                        if (Math.Abs(yearsPassed - Math.Floor(yearsPassed)) < 0.01)
                        {
                            // Вычисляем количество дней в одобренных заявках
                            var approvedDays = employee.PlannedVacations
                                .Sum(pv => CalculateDaysBetween(pv.StartDate, pv.EndDate));

                            // Добавляем новые дни и вычитаем использованные
                            employee.AccumulatedVacationDays = (employee.AccumulatedVacationDays + 14) - approvedDays;
                            
                            _logger.LogInformation(
                                "Employee {EmployeeId}: Added 14 days, subtracted {ApprovedDays} days from approved vacations. New total: {NewTotal}",
                                employee.EmployeeId,
                                approvedDays,
                                employee.AccumulatedVacationDays
                            );
                            
                            dbContext.Update(employee);
                        }
                    }

                    await dbContext.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Vacation days updated successfully.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating vacation days.");
            }
        }

        private int CalculateDaysBetween(DateOnly startDate, DateOnly endDate)
        {
            return (endDate.DayNumber - startDate.DayNumber) + 1;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}