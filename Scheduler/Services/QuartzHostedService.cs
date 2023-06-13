using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Spi;
using Scheduler.Dtos;
using Scheduler.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Scheduler.Services
{
    public class QuartzHostedService : IHostedService
    {

        private readonly ISchedulerFactory _schedulerFactory;

        private readonly IJobFactory _jobFactory;

        private readonly ILogger<QuartzHostedService> _logger;

        private readonly IEnumerable<JobSchedule> _injectJobSchedules;

        private List<JobSchedule> _allJobSchedules;

        private readonly IJobListener _jobListener;

        private readonly ISchedulerListener _schedulerListener;
        
        public IScheduler Scheduler { get; set; }

        public CancellationToken CancellationToken { get; private set; }



        public QuartzHostedService(ILogger<QuartzHostedService> logger, ISchedulerFactory schedulerFactory, IJobFactory jobFactory, IEnumerable<JobSchedule> jobSchedules, IJobListener jobListener, ISchedulerListener schedulerListener)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _schedulerFactory = schedulerFactory ?? throw new ArgumentNullException(nameof(schedulerFactory));
            _jobFactory = jobFactory ?? throw new ArgumentNullException(nameof(jobFactory));
            _jobListener = jobListener ?? throw new ArgumentNullException(nameof(jobListener));
            _schedulerListener = schedulerListener ?? throw new ArgumentNullException(nameof(schedulerListener));
            _injectJobSchedules = jobSchedules ?? throw new ArgumentNullException(nameof(jobSchedules));
        }



        /// <summary>
        /// start scheduler
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (Scheduler == null || Scheduler.IsShutdown)
            {
                // save the cancellation token
                CancellationToken = cancellationToken;

                // First join the Job that is registered and injected in startup
                _allJobSchedules = new List<JobSchedule>();
                _allJobSchedules.AddRange(_injectJobSchedules);

                // Then simulate the dynamic addition of new Job items (e.g. from the DB, which can dynamically determine the output timing for different reports)
                //Trigger Every 30 secs
                _allJobSchedules.Add(new JobSchedule(jobName: "03", jobType: typeof(ReportJob), cronExpression: "0/30 * * * * ?"));
                //Trigger Every 10 secs
                _allJobSchedules.Add(new JobSchedule(jobName: "04", jobType: typeof(ReportJob), cronExpression: "0/10 * * * * ?"));

                // Initial Scheduler Scheduler
                Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
                Scheduler.JobFactory = _jobFactory;
                Scheduler.ListenerManager.AddJobListener(_jobListener);
                Scheduler.ListenerManager.AddSchedulerListener(_schedulerListener);

                // Add work items to the scheduler one by one
                foreach (var jobSchedule in _allJobSchedules)
                {
                    var jobDetail = CreateJobDetail(jobSchedule);
                    var trigger = CreateTrigger(jobSchedule);
                    await Scheduler.ScheduleJob(jobDetail, trigger, cancellationToken);
                    jobSchedule.JobStatus = JobStatus.Scheduled;
                }

                // 啟動排程
                await Scheduler.Start(cancellationToken);
            }
        }

        /// <summary>
        /// stop scheduler
        /// </summary>
        /// <returns></returns>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (Scheduler != null && !Scheduler.IsShutdown)
            {
                _logger.LogInformation($"@{DateTime.Now:HH:mm:ss} - Scheduler StopAsync");
                await Scheduler.Shutdown(cancellationToken);
            }
        }

        /// <summary>
        /// Get the latest status of all jobs
        /// </summary>
        public async Task<IEnumerable<JobSchedule>> GetJobSchedules()
        {
            if (Scheduler.IsShutdown)
            {
                // When the scheduler is stopped, update the status of each job to stop
                foreach (var jobSchedule in _allJobSchedules)
                {
                    jobSchedule.JobStatus = JobStatus.Stopped;
                }
            }
            else
            {
                // Get the currently executing Job to update the status of each Job
                var executingJobs = await Scheduler.GetCurrentlyExecutingJobs();
                foreach (var jobSchedule in _allJobSchedules)
                {
                    var isRunning = executingJobs.FirstOrDefault(j => j.JobDetail.Key.Name == jobSchedule.JobName) != null;
                    jobSchedule.JobStatus = isRunning ? JobStatus.Running : JobStatus.Scheduled;
                }

            }

            return _allJobSchedules;
        }

        /// <summary>
        /// Trigger jobs manually
        /// </summary>
        public async Task TriggerJobAsync(string jobName)
        {
            if (Scheduler != null && !Scheduler.IsShutdown)
            {
                _logger.LogInformation($"@{DateTime.Now:HH:mm:ss} - job{jobName} - TriggerJobAsync");
                await Scheduler.TriggerJob(new JobKey(jobName), CancellationToken);
            }
        }

        /// <summary>
        /// Manually interrupt a job
        /// </summary>
        public async Task InterruptJobAsync(string jobName)
        {
            if (Scheduler != null && !Scheduler.IsShutdown)
            {
                var targetExecutingJob = await GetExecutingJob(jobName);
                if (targetExecutingJob != null)
                {
                    _logger.LogInformation($"@{DateTime.Now:HH:mm:ss} - job{jobName} - InterruptJobAsync");
                    await Scheduler.Interrupt(new JobKey(jobName));
                }

            }
        }

        /// <summary>
        /// Get a specific running job
        /// /// </summary>
        private async Task<IJobExecutionContext> GetExecutingJob(string jobName)
        {
            if (Scheduler != null)
            {
                var executingJobs = await Scheduler.GetCurrentlyExecutingJobs();
                return executingJobs.FirstOrDefault(j => j.JobDetail.Key.Name == jobName);
            }

            return null;
        }

        /// <summary>
        /// Create job details (the Job entity will be retrieved from the DI container through the JobFactory based on this information later)
        /// </summary>
        private IJobDetail CreateJobDetail(JobSchedule jobSchedule)
        {
            var jobType = jobSchedule.JobType;
            var jobDetail = JobBuilder
                .Create(jobType)
                .WithIdentity(jobSchedule.JobName)  
                .WithDescription(jobType.Name)
                .Build();

            // You can pass data to the job when creating the job
            jobDetail.JobDataMap.Put("Payload", jobSchedule);

            return jobDetail;
        }

        /// <summary>
        /// Create trigger
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        private ITrigger CreateTrigger(JobSchedule schedule)
        {
            return TriggerBuilder
                .Create()
                .WithIdentity($"{schedule.JobName}.trigger") 
                .WithCronSchedule(schedule.CronExpression)
                .WithDescription(schedule.CronExpression)
                .Build();
        }
    }
}
