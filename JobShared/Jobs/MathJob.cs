using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobShared.Jobs
{
    public class MathJob : IJob
    {

        public string Name { get; set; }
        public int A { get; set; }
        public int B { get; set; }
        public int RetryCount { get; set; } = 0;
        public int MaxRetries { get; set; } = 2;
        public int TimeoutInSeconds { get; set; } = 60;
        public DateTime? ScheduledAt { get; set; }
        public MathJob(int a, int b)
        {
            A = a;
            B = b;
            Name = $"Add {a} + {b}";
        }

        public async Task ExecuteAsync()
        {
            await Task.Delay(500);
            Console.WriteLine($"{A} + {B} = {A + B}");
        }
    }
}
