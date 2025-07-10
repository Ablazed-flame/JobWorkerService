using JobWorkerService.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobWorkerService.Data
{

    public class JobDbContext : DbContext
    {
        public JobDbContext(DbContextOptions<JobDbContext> options) : base(options) { }

        public DbSet<JobHistory> JobHistories { get; set; } = null!;
        public DbSet<RecurringJob> RecurringJobs { get; set; } = null!;
    }
}
