using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using Scheduler.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Jobs
{
    [DisallowConcurrentExecution]
    public class ReportJob : IJob
    {
        private readonly ILogger<ReportJob> _logger;

        private readonly IServiceProvider _provider;


        public ReportJob(ILogger<ReportJob> logger, IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public Task Execute(IJobExecutionContext context)
        {

            // User-defined JobSchedule data can be obtained, and different report data can be created according to the content provided by JobSchedule
            var schedule = context.JobDetail.JobDataMap.Get("Payload") as JobSchedule;
            var jobName = schedule.JobName;

            using (var scope = _provider.CreateScope())
            {
                // If you want to use the object entity defined as Scope in the DI container, since Job is defined as singleton
                // Therefore, the entity of Scope cannot be obtained directly, and the entity needs to be generated in the scope in CreateScope at this time
                // ex. var dbContext = scope.ServiceProvider.GetService<AppDbContext>();
            }


            _logger.LogInformation($"@{DateTime.Now:HH:mm:ss} - job{jobName} - start");
            for (int i = 0; i < 5; i++)
            {

                // Define yourself which side is suitable to end when the job is forced to be interrupted
                // If not set, when the job is interrupted, it will not really be interrupted, but will run completely
                if (context.CancellationToken.IsCancellationRequested)
                {
                    break;
                }

                System.Threading.Thread.Sleep(1000);
                _logger.LogInformation($"@{DateTime.Now:HH:mm:ss} - job{jobName} - working{i}");

            }


            _logger.LogInformation($"@{DateTime.Now:HH:mm:ss} - job{jobName} - done");
            return Task.CompletedTask;
        }
    }
}
