using JobApiService.dtos;
using JobShared.Jobs;
using JobShared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly JobQueue _jobQueue;
        private readonly ILogger<JobsController> _logger;
        public JobsController(JobQueue jobQueue,ILogger<JobsController> logger)
        {
            _jobQueue = jobQueue;
            _logger = logger;
        }

        [HttpPost("print")]
        public async Task<IActionResult> SubmitPrintJob([FromBody] PrintJobRequest request)
        {
            var job = new PrintJob(request.Name)
            {
                ScheduledAt = request.ScheduledAt
            };
            await _jobQueue.EnqueueOrScheduleAsync(job);
            return Ok(new { message = "Job submitted", job.Name });
        }

        [HttpPost("email")]
        public async Task<IActionResult> SubmitEmailJob([FromBody] EmailJobRequest request)
        {
            var job = new EmailJob(request.To, request.Message)
            {
                ScheduledAt = request.ScheduledAt
            };
            await _jobQueue.EnqueueOrScheduleAsync(job);
            return Ok(new { message = "Email job submitted", job.Name });
        }

        [HttpPost("math")]
        public async Task<IActionResult> SubmitMathJob([FromBody] MathJobRequest request)
        {
            var job = new MathJob(request.A, request.B)
                { 
                ScheduledAt = request.ScheduledAt 
                };
            await _jobQueue.EnqueueOrScheduleAsync(job);
            return Ok(new { message = "Math job submitted", job.Name });
        }

        [HttpGet("dlq")]
        public async Task<IActionResult> GetDeadLetterQueue()
        {
            var jobs = await _jobQueue.GetDeadLetterJobsAsync();
            return Ok(jobs);
        }

        [HttpPost("dlq/requeue-by-id/{id}")]
        public async Task<IActionResult> RequeueById(string id)
        {
            var result = await _jobQueue.RequeueDeadLetterJobByIdAsync(id);
            if (!result)
                return NotFound(new { message = "Job ID not found or failed to requeue" });

            return Ok(new { message = $"Job {id} requeued successfully." });
        }


    }
}
