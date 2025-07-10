using JobAdminDashboard.Services;
using Microsoft.AspNetCore.Mvc;

namespace JobAdminDashboard.Controllers
{
    public class DashboardController : Controller
    {
        private readonly JobApiClient _client;

        public DashboardController(JobApiClient client)
        {
            _client = client;
        }

        public async Task<IActionResult> DLQ()
        {
            var jobs = await _client.GetDLQAsync();
            return View(jobs);
        }

        [HttpPost]
        public async Task<IActionResult> Requeue(string id)
        {
            await _client.RequeueFromDLQ(id);
            return RedirectToAction("DLQ");
        }

        public async Task<IActionResult> Jobs()
        {
            var jobs = await _client.GetAllJobsAsync();
            return View(jobs);
        }

    }
}
