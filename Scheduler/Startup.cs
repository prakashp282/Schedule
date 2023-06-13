using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Scheduler.Dtos;
using Scheduler.Factories;
using Scheduler.Jobs;
using Scheduler.Listener;
using Scheduler.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scheduler
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();


            //Register the Quartz service with the DI container
            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<IJobListener, JobListener>();
            services.AddSingleton<ISchedulerListener, SchedulerListener>();

            //Register Job with DI container
            services.AddSingleton<ReportJob>();

            //Register JobSchedule with DI container
            //Trigger Every 5 mins
            services.AddSingleton(new JobSchedule(jobName: "01", jobType: typeof(ReportJob), cronExpression: "0 0/5 * * * ?"));
            //Trigger every min
            services.AddSingleton(new JobSchedule(jobName: "02", jobType: typeof(ReportJob), cronExpression: "0 0/1 * * * ?"));

            //Register the Host service with the DI container
            services.AddSingleton<QuartzHostedService>();
            services.AddHostedService(provider => provider.GetService<QuartzHostedService>());

            // If you want to use ASP.NET to perform regular scheduling, there is a premise that "you need to ensure that the website is always in the execution state"
            // Note that IIS will automatically withdraw the execution of the application and cause the scheduler to be interrupted
            // At this time, set Preload Enable = true, Start Mode = AlwaysRunning
            // https://docs.microsoft.com/zh-tw/archive/blogs/vijaysk/iis-8-whats-new-website-settings
            // https://docs.microsoft.com/zh-tw/archive/blogs/vijaysk/iis-8-whats-new-application-pool-settings
            // https://blog.darkthread.net/blog/hangfire-recurringjob-notes/


            // Register the DB container schedulerHub entity
            services.AddSingleton<SchedulerHub>();

            //Set up SignalR service
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                // Set signalR's router
                endpoints.MapHub<SchedulerHub>("/schedulerHub");
            });


         
        }
    }
}