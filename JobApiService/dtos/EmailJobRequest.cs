namespace JobApiService.dtos
{
    public class EmailJobRequest
    {
        public string To { get; set; } = "";
        public string Message { get; set; } = "";
        public DateTime? ScheduledAt { get; set; }
    }
}
