using JobWorkerService;
using JobShared.Services;
using StackExchange.Redis;
using JobWorkerService.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using JobShared.Jobs;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<ScheduledJobMonitorService>();
builder.Services.AddHostedService<RecurringJobSchedulerService>();
builder.Services.AddSingleton<JobQueue>();

builder.Services.AddDbContext<JobDbContext>(options =>
    options.UseSqlite("Data Source=jobs.db")
);
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost:6379")
);
var host = builder.Build();
//using (var scope = host.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<JobDbContext>();

//    if (!db.RecurringJobs.Any())
//    {
//        var emailJob = new EmailJob("admin@example.com", "Daily system check")
//        {
//            ScheduledAt = null
//        };

//        var recurring = new JobWorkerService.Models.RecurringJob
//        {
//            Name = "Daily Email Job",
//            JobType = emailJob.GetType().Name,
//            CronExpression = "*/1 * * * *", // every 1 minute
//            SerializedJobData = JsonSerializer.Serialize(emailJob)
//        };
        
//        db.RecurringJobs.Add(recurring);
//        db.SaveChanges();
//    }
//}

host.Run();
