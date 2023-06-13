using Microsoft.AspNetCore.SignalR;
using Scheduler.Dtos;
using Scheduler.Extensions;
using Scheduler.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler
{
    public class SchedulerHub : Hub
    {
        private QuartzHostedService _quartzHostedService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="quartzHostedService">Quartz Scheduling Service</param>
        public SchedulerHub (QuartzHostedService quartzHostedService)
        {
            _quartzHostedService = quartzHostedService;
        }

        /// <summary>
        /// Request to get Job Status
        /// </summary>
        public async Task RequestJobStatus()
        {
            if (Clients != null)
            {
                var jobs = await _quartzHostedService.GetJobSchedules();
                var jobSummary = jobs.Select(e => 
                        new JobScheduleSummary { 
                            JobName = e.JobName, 
                            CronExpression = e.CronExpression, 
                            JobStatusName = e.JobStatus.GetDescription(), 
                            JobStatusId = e.JobStatus, 
                            JobType = e.JobType.FullName 
                        }
                    );

                await Clients.Caller.SendAsync("ReceiveJobStatus", jobSummary);
            }
        }

        /// <summary>
        /// Notify Job status changes
        /// </summary>
        public async Task NotifyJobStatusChange()
        {
            if (Clients != null)
            {
                await Clients.All.SendAsync("JobStatusChange");
            }
        }

        /// <summary>
        /// Manually trigger job execution
        /// </summary>
        public async Task TriggerJob(string jobName)
        {
            await _quartzHostedService.TriggerJobAsync(jobName);
        }

        /// <summary>
        /// Manually interrupt job execution
        /// </summary>
        public async Task InterruptJob(string jobName)
        {
            await _quartzHostedService.InterruptJobAsync(jobName);
        }

        
        /// <summary>
        /// Start the scheduler
        /// </summary>
        public async Task StartScheduler()
        {
            await _quartzHostedService.StartAsync(_quartzHostedService.CancellationToken);
        }

        /// <summary>
        /// close scheduler
        /// </summary>
        public async Task StopScheduler()
        {
            await _quartzHostedService.StopAsync(_quartzHostedService.CancellationToken);
        }


        /// <summary>
        /// user connection event
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await NotifyJobStatusChange();
            await base.OnConnectedAsync();
        }


        /// <summary>
        /// User disconnection event
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
