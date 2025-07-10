using JobAdminDashboard.Models;

namespace JobAdminDashboard.Services
{
    public class JobApiClient
    {
        private readonly HttpClient _http;

        public JobApiClient(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("JobApi");
        }

        public async Task<List<DeadLetterJob>> GetDLQAsync()
        {
            var jobs = await _http.GetFromJsonAsync<List<DeadLetterJob>>("jobs/dlq");
            return jobs ?? new List<DeadLetterJob>();
        }

        public async Task<bool> RequeueFromDLQ(string id)
        {
            var res = await _http.PostAsync($"jobs/dlq/requeue-by-id/{id}", null);
            return res.IsSuccessStatusCode;
        }
        public async Task<List<JobStatusRecord>> GetAllJobsAsync()
        {
            var res = await _http.GetFromJsonAsync<List<JobStatusRecord>>("jobs/status");
            return res ?? new List<JobStatusRecord>();
        }

    }
}
