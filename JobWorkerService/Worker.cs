using JobShared.Jobs;
using JobShared.Services;
using JobWorkerService.Data;
using JobWorkerService.Models;

namespace JobWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly JobQueue _jobQueue;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, JobQueue jobQueue,IServiceProvider serviceProvider)
        {
            _logger = logger;
            _jobQueue = jobQueue;
            
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var job = await _jobQueue.DequeueAsync();


                if (job != null)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<JobDbContext>();

                        var history = new JobHistory
                        {
                            JobId = Guid.NewGuid(),
                            JobName = job.Name,
                            RetryCount = job.RetryCount,
                            Status = "Pending",
                            CreatedAt = DateTime.UtcNow
                        };

                        db.JobHistories.Add(history);
                        await db.SaveChangesAsync();

                        try
                        {
                            await job.ExecuteAsync();
                            history.Status = "Success";
                            history.CompletedAt = DateTime.UtcNow;
                        }
                        catch (Exception ex)
                        {
                            history.Status = "Failed";
                            history.ErrorMessage = ex.Message;
                            history.RetryCount = (++job.RetryCount);

                            if (job.RetryCount <= job.MaxRetries)
                            {
                                _logger.LogInformation($"[Retrying] {job.Name} in 5 seconds");
                                await _jobQueue.RequeueAsync(job, 5);
                            }
                            else
                            {
                                _logger.LogError($"[Gave Up] {job.Name} after {job.RetryCount} attempts");
                                await _jobQueue.MoveToDeadLetterAsync(job, ex.Message);
                            }
                        }

                        await db.SaveChangesAsync();
                    }
                }
                else
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }

}
