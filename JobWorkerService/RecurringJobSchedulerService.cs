using JobShared.Jobs;
using JobShared.Registries;
using JobShared.Services;
using JobWorkerService.Data;
using NCrontab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JobWorkerService
{
    public class RecurringJobSchedulerService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly JobQueue _queue;
        private readonly ILogger<RecurringJobSchedulerService> _logger;

        public RecurringJobSchedulerService(
            IServiceProvider serviceProvider,
            JobQueue queue,
            ILogger<RecurringJobSchedulerService> logger)
        {
            _serviceProvider = serviceProvider;
            _queue = queue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("RecurringJobSchedulerService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<JobDbContext>();
      
                    var recurringJobs = db.RecurringJobs.ToList();
                    var now = DateTime.UtcNow;

                    foreach (var recurring in recurringJobs)
                    {
                        CrontabSchedule schedule;
                        try
                        {
                            schedule = CrontabSchedule.Parse(recurring.CronExpression);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Invalid CRON for job {recurring.Name}");
                            continue;
                        }

                        var lastRun = recurring.LastRunUtc ?? DateTime.MinValue;
                        var nextRun = schedule.GetNextOccurrence(lastRun);

                        if (nextRun <= now)
                        {
                            _logger.LogInformation($"[Recurring Trigger] {recurring.Name}");

                            var jobType = JobTypeRegistry.Resolve(recurring.JobType);
                            if (jobType == null)
                            {
                                _logger.LogWarning($"Job type '{recurring.JobType}' not found.");
                                continue;
                            }

                            var job = JsonSerializer.Deserialize(recurring.SerializedJobData, jobType) as IJob;
                            if (job != null)
                            {
                                job.Name = recurring.Name + $" [{now}]";
                                await _queue.EnqueueAsync(job);
                            }

                            recurring.LastRunUtc = now;
                            db.Update(recurring);
                        }
                    }

                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in RecurringJobSchedulerService loop.");
                }

                await Task.Delay(10000, stoppingToken); // Poll every 10 seconds
            }

            _logger.LogInformation("RecurringJobSchedulerService stopped.");
        }
    }
}
