using JobShared.Jobs;
using JobShared.Services;

namespace JobDispatcherService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly JobQueue _jobQueue;
        private int _jobCounter = 1;

        public Worker(ILogger<Worker> logger, JobQueue jobQueue)
        {
            _logger = logger;
            _jobQueue = jobQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var job = new PrintJob($"Dispatched Job #{_jobCounter++}");
                await _jobQueue.EnqueueAsync(job);

                _logger.LogInformation($"[Dispatched] {job.Name}");
                await Task.Delay(3000, stoppingToken);
            }
        }
    }

}
