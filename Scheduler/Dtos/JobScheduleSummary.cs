using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler.Dtos
{
    /// <summary>
    /// Job排程中間物件 View
    /// </summary>
    public class JobScheduleSummary
    {

        /// <summary>
        /// Job identification name
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// Job type
        /// </summary>
        /// 
        public string JobType { get; set; }

        /// <summary>
        /// Cron expression
        /// </summary>
        /// 
        public string CronExpression { get; set; }

        /// <summary>
        /// Job status name
        /// </summary>
        /// 
        public string JobStatusName { get; set; }

        /// <summary>
        /// Job status code
        /// </summary>
        /// 
        public JobStatus JobStatusId { get; set; }

    }

}
