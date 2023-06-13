# Schedule
# ASP.NET Core scheduling with Quartz.NET and SignalR monitoring Dashboard

![](https://github.com/prakashp282/Schedule/blob/master/Scheduler.gif)

When the application website has some external data that needs to be obtained regularly, or some internal time-consuming operations need to be digested in batches one by one, it will need to be processed by scheduling jobs, and the more streamlined way is to host the scheduling job in the .NET Core application program, this project uses Quartz.NET scheduling job Hosted in ASP.NET Core as an example; after solving the operation problem, what is faced is the maintenance operation, how to make the maintenance personnel can clearly grasp the current various The execution status of the scheduled job, which requires a real-time Dashboard page to present relevant information. This part can use SignalR technology to allow the Dashboard and the back-end program to maintain a real-time and active communication channel with each other, so as to avoid the previous front-end regular back-end Network information consumption caused by pulling data on the terminal.


# Features.
a. Status management page
<br>
b. Click Interrupt to forcibly interrupt the job
<br>
c. Click Trigger to trigger job execution
<br>
d. Close the scheduler and start the scheduler


