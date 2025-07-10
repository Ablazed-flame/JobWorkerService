using JobShared.Helper_classes;
using JobShared.Jobs;
using JobShared.Registries;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JobShared.Services
{
    public class JobQueue
    {
        private readonly IDatabase _redisDb;
        private const string QueueKey = "job-queue";
        private const string ScheduleKey = "scheduled-jobs";
        private const string DeadLetterKey = "dead-letter-queue";

        public JobQueue(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }

        public async Task EnqueueAsync(IJob job)
        {
            var serialized = SerializeJobWithType(job);
            await _redisDb.ListRightPushAsync(QueueKey, serialized);
        }

        public async Task EnqueueOrScheduleAsync(IJob job)
        {
            if (job.ScheduledAt.HasValue && job.ScheduledAt.Value > DateTime.UtcNow)
            {
                var payload = SerializeJobWithType(job);
                double score = ((DateTimeOffset)job.ScheduledAt.Value).ToUnixTimeSeconds();
                await _redisDb.SortedSetAddAsync(ScheduleKey, payload, score);
            }
            else
            {
                await EnqueueAsync(job);
            }
        }

        public async Task EnqueueSerializedJobAsync(string serializedJob)
        {
            await _redisDb.ListRightPushAsync(QueueKey, serializedJob);
        }

        public async Task<IJob?> DequeueAsync()
        {
            var serialized = await _redisDb.ListLeftPopAsync(QueueKey);
            if (serialized.IsNullOrEmpty) return null;

            var wrapper = JsonSerializer.Deserialize<JobWrapper>(serialized!);
            if (wrapper == null || string.IsNullOrEmpty(wrapper.Type)) return null;

            var jobType = JobTypeRegistry.Resolve(wrapper.Type);
            if (jobType == null) return null;

            return JsonSerializer.Deserialize(wrapper.Data, jobType) as IJob;
        }
        public async Task RequeueAsync(IJob job, int delaySeconds)
        {
            // Delay push by putting a timestamp prefix (optional advanced)
            await Task.Delay(delaySeconds * 1000);
            await EnqueueAsync(job);
        }


        public async Task MoveToDeadLetterAsync(IJob job, string error)
        {
            var wrapper = new
            {
                Id = Guid.NewGuid().ToString(),
                Type = job.GetType().Name,
                Data = JsonSerializer.Serialize(job, job.GetType()),
                Error = error,
                FailedAt = DateTime.UtcNow
            };

            string payload = JsonSerializer.Serialize(wrapper);
            await _redisDb.ListRightPushAsync(DeadLetterKey, payload);
        }

        public async Task<List<DeadLetterJob>> GetDeadLetterJobsAsync(int limit = 50)
        {
            var entries = await _redisDb.ListRangeAsync(DeadLetterKey, 0, limit - 1);
            return entries
                .Select(e =>
                {
                    try
                    {
                        return JsonSerializer.Deserialize<DeadLetterJob>(e!)!;
                    }
                    catch
                    {
                        return null!;
                    }
                })
                .Where(x => x != null)
                .ToList();
        }

        public async Task<bool> RequeueDeadLetterJobByIdAsync(string id)
        {
            var entries = await _redisDb.ListRangeAsync(DeadLetterKey, 0, -1);

            foreach (var entry in entries)
            {
                var wrapper = JsonSerializer.Deserialize<DeadLetterJob>(entry!);
               // Console.WriteLine("Arrived");
                if (wrapper != null && wrapper.Id == id)
                {
                    // Remove from DLQ
                   // Console.WriteLine("Arived inner");
                    await _redisDb.ListRemoveAsync(DeadLetterKey, entry);

                    // Deserialize and enqueue
                    var jobType = JobTypeRegistry.Resolve(wrapper.Type);
                    if (jobType == null) return false;

                    var job = JsonSerializer.Deserialize(wrapper.Data, jobType) as IJob;
                    if (job == null) return false;

                    job.RetryCount = 0; // reset retry
                    await EnqueueAsync(job);

                    return true;
                }
            }

            return false;
        }




        private string SerializeJobWithType(IJob job)
        {
            var wrapper = new
            {
                Type = job.GetType().Name,
                Data = JsonSerializer.Serialize(job, job.GetType())
            };
            return JsonSerializer.Serialize(wrapper);
        }

    }
}
