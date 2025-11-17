namespace JobAdminDashboard.Models
{
    public class DeadLetterJob
    {
        public string Id { get; set; } = "";
        public string Type { get; set; } = "";
        public string Data { get; set; } = "";
        public string Error { get; set; } = "";
        public DateTime FailedAt { get; set; }
    }
}
