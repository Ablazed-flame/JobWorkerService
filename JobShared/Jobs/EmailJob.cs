using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobShared.Jobs
{
   
    public class EmailJob : IJob
    {
        public string Name { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
        public int RetryCount { get; set; } = 0;
        public int MaxRetries { get; set; } = 3;
        
        public DateTime? ScheduledAt { get; set; } = null;
        public int TimeoutInSeconds { get; set; } = 60;
        public EmailJob(string to, string message)
        {
            Name = $"Email to {to}";
            To = to;
            Message = message;
        }

        public async Task ExecuteAsync()
        {
            Console.WriteLine($"Sending email to {To}...");
            await Task.Delay(1000);
            Console.WriteLine($"Email sent: {Message}");
        }
    }

}
