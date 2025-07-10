using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobShared.Jobs;

public class PrintJob : IJob
{

    public string Name { get; set; }
    public int RetryCount { get; set; } = 0;
    public int MaxRetries { get; set; } = 3;
    public DateTime? ScheduledAt { get; set; }
    public int TimeoutInSeconds { get; set; } = 60;
    public PrintJob(string name)
    {
        Name = name;
    }

    public async Task ExecuteAsync()
    {
        Console.WriteLine($"[Executing] {Name}");

        // Simulate random failure
        //if (new Random().Next(1, 4) == 1) // 25% fail chance
        //    throw new Exception("Simulated job failure.");

        throw new Exception("Simulated job failure."); // Force failure for testing
        await Task.Delay(1000);
        Console.WriteLine($"[Completed] {Name}");
    }
}

