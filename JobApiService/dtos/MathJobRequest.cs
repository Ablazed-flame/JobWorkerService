namespace JobApiService.dtos
{
    public class MathJobRequest
    {
        public int A { get; set; }
        public int B { get; set; }
        public DateTime? ScheduledAt { get; set; }
    }
}
