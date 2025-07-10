namespace JobApiService.dtos
{
    public class PrintJobRequest
    {
        public string Name { get; set; }
        public DateTime? ScheduledAt { get; set; }
    }
}
