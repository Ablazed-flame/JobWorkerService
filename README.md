# ⚡ Distributed Job Scheduler

A **C# based distributed job scheduling system** that allows jobs to be submitted via an API and executed by worker services in the background.  
The system supports **retry mechanisms, error logging, and job history tracking**, making it extensible and production-ready.

---

## 🚀 Features
- **Job Queueing**: Submit jobs via REST API and enqueue them for processing.  
- **Background Workers**: Multiple workers process jobs concurrently.  
- **Retry Mechanism**: Failed jobs are retried up to a configurable limit.  
- **Job History Tracking**: Every execution is logged with status, retry count, timestamps, and error messages.  
- **Separation of Concerns**:  
  - **JobApiService** manages job submission, authentication, and APIs.  
  - **JobWorkerService** executes jobs and maintains execution logs.  
  - Each uses its **own database** for better isolation.  
- **Extensibility**: New job types can be added easily by implementing `IJob`.

---

## 🏗️ Architecture
        +--------------------+
        |   JobApiService    |
        |  (REST API Layer)  |
        +---------+----------+
                  |
                  v
          [ Shared JobQueue ]
                  |
    +-------------+--------------+
    |                            |
    +-------+--------+ +-------+--------+
    | JobWorkerService| | JobWorkerService|
    | (Executes Jobs) | | (Executes Jobs) |
    +-------+--------+ +-------+--------+
    |
    v
    +--------------------+
    | JobHistory DB |
    | (Logs executions) |
    +--------------------+
---

## 📂 Project Structure
DistributedJobScheduler/
│-- JobShared/ # Shared interfaces and JobQueue
│ ├── IJob.cs
│ ├── PrintJob.cs
│ └── JobQueue.cs
│
│-- JobWorkerService/ # Background worker service
│ ├── JobHistory.cs
│ └── Worker.cs
│
│-- JobApiService/ # REST API project
│ ├── Controllers/
│ ├── Models/
│ └── Services/

---

## ⚡ Installation & Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/Ablazed-flame/JobWorkerService.git
   cd JobWorkerService
2. Update the connection strings in appsettings.json for both:

=> JobApiService (API + Authentication DB)

=> JobWorkerService (JobHistory DB)
3. Apply Entity Framework migrations:
   ```bash
    dotnet ef database update
   ```
4. Start the API service:
   ```bash
   dotnet run --project JobApiService
   ```
5. Start one or more worker services:
   ```bash
   dotnet run --project JobWorkerService
   ```
6. Submit a job via API (example using cURL):
   ```bash
   curl -X POST https://localhost:5001/api/jobs \
   -H "Content-Type: application/json" \
   -d '{
    "jobType": "PrintJob",
   "message": "Hello World!"
   }'
   ```
7. Check job status/history:
   ```bash
   curl https://localhost:5001/api/jobs/{jobId}
   ```
   


