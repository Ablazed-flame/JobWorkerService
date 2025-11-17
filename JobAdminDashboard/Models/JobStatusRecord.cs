namespace JobAdminDashboard.Models
{
    public class JobStatusRecord
    {
        public int Id { get; set; }
        public string JobType { get; set; } = "";
        public string Status { get; set; } = ""; // Pending, Running, Succeeded, Failed
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
