using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Scheduler.Listener
{
    /// <summary>
    /// Job listener
    /// </summary>
    public class JobListener : IJobListener
    {

        private readonly ILogger<JobListener> _logger;

        private readonly IServiceProvider _serviceProvider = null;  // Use this to generate schedulerHub entities to avoid circular references caused by direct injection



        string IJobListener.Name => "Jobs Listener";



        public JobListener(ILogger<JobListener> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }



        public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            // The job will be executed (currently the Job status has not entered the Executing list)

            var jobName = context.JobDetail.Key.Name;
            _logger.LogInformation($"@{DateTime.Now:HH:mm:ss} - job{jobName} - JobToBeExecuted");

            var schedulerHub = _serviceProvider.GetRequiredService<SchedulerHub>();
            await schedulerHub.NotifyJobStatusChange();
         
        }

        public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException, CancellationToken cancellationToken = default)
        {
            // The job execution is completed (currently the Job status has not been removed from the Executing list)

            var jobName = context.JobDetail.Key.Name;
            _logger.LogInformation($"@{DateTime.Now:HH:mm:ss} - job{jobName} - JobWasExecuted");


            var schedulerHub = _serviceProvider.GetRequiredService<SchedulerHub>();
            await schedulerHub.NotifyJobStatusChange();

        }

        public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

    }
}
