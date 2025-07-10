using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobShared.Jobs;

public interface IJob
{
    string Name { get; set; }
    int RetryCount { get; set; }
    int MaxRetries { get; }
    int TimeoutInSeconds { get; set; }
    DateTime? ScheduledAt { get; set; }
    Task ExecuteAsync();
}

