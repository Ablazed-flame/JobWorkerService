using JobShared.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobWorkerService
{
    public class ScheduledJobMonitorService : BackgroundService
    {
        private readonly ILogger<ScheduledJobMonitorService> _logger;
        private readonly IConnectionMultiplexer _redis;
        private readonly JobQueue _jobQueue;

        public ScheduledJobMonitorService(
            ILogger<ScheduledJobMonitorService> logger,
            IConnectionMultiplexer redis,
            JobQueue jobQueue)
        {
            _logger = logger;
            _redis = redis;
            _jobQueue = jobQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var db = _redis.GetDatabase();
            const string ScheduleKey = "scheduled-jobs";

            while (!stoppingToken.IsCancellationRequested)
            {
                double now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var dueJobs = await db.SortedSetRangeByScoreAsync(ScheduleKey, stop: now, take: 5);

                foreach (var jobData in dueJobs)
                {
                    await db.SortedSetRemoveAsync(ScheduleKey, jobData);
                    await _jobQueue.EnqueueSerializedJobAsync(jobData!);
                    _logger.LogInformation($"[Scheduled Dispatch] Job moved to queue");
                }

                await Task.Delay(1000, stoppingToken); // check every second
            }
        }
    }
}
