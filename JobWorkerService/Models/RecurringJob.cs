using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobWorkerService.Models
{
    public class RecurringJob
    {
        public int Id { get; set; }
        public string JobType { get; set; } = ""; // Full type name like EmailJob
        public string CronExpression { get; set; } = "";
        public string SerializedJobData { get; set; } = ""; // JSON of job
        public DateTime? LastRunUtc { get; set; }
        public string Name { get; set; } = "";
    }
}
