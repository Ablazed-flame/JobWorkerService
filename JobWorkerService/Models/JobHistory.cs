using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobWorkerService.Models
{
    public class JobHistory
    {
        public int Id { get; set; }
        public Guid JobId { get; set; }
        public string JobName { get; set; } = "";
        public string Status { get; set; } = "Pending"; // Pending, Success, Failed
        public int RetryCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
