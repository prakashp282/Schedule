using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Dtos
{
    public class JobSchedule
    {
        public JobSchedule(Type jobType, string cronExpression, string jobName)
        {
            JobType = jobType ?? throw new ArgumentNullException(nameof(jobType));
            CronExpression = cronExpression ?? throw new ArgumentNullException(nameof(cronExpression));
            JobName = jobName ?? throw new ArgumentNullException(nameof(jobName));
        }

        /// <summary>
        /// Job identification name
        /// </summary>
        public string JobName { get; private set; }

        /// <summary>
        /// Job type
        /// </summary>
        public Type JobType { get; private set; }

        /// <summary>
        /// Cron expression
        /// </summary>
        public string CronExpression { get; private set; }

        /// <summary>
        /// Job status
        /// </summary>
        public JobStatus JobStatus { get; set; } = JobStatus.Init;
    }

}
