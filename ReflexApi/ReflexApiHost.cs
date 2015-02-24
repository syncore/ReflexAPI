using System;
using System.IO;
using System.Reflection;
using Funq;
using log4net.Config;
using Quartz;
using Quartz.Impl;
using ReflexAPI.Util;
using ServiceStack;
// ReSharper disable InconsistentNaming

namespace ReflexAPI
{
    /// <summary>
    /// ServiceStack-related API class.
    /// </summary>
    public class ReflexAPIAppHost : AppSelfHostBase
    {
        private const int SecsWaitBeforeInitialRetrieval = 10;
        private const int DoRetrievalEverySecs = 90;
        private readonly IScheduler _scheduler = StdSchedulerFactory.GetDefaultScheduler();

        /// <summary>
        /// Initializes a new instance of the ServiceStack application, with the specified name and assembly containing the services.
        /// </summary>
        public ReflexAPIAppHost()
            : base("Reflex Server API by syncore", typeof(ReflexAPIAppHost).Assembly)
        {
            LoggerUtil.LogInfoAndDebug(
                string.Format("ReflexApiAppHost Init() called. Initial server list retrieval shoud occur in roughly {0} seconds.",
                SecsWaitBeforeInitialRetrieval), MethodBase.GetCurrentMethod().DeclaringType);

            DoStartupActivities();
        }

        /// <summary>
        /// Does the startup activities (scheduling jobs, configuration logging, etc.)
        /// </summary>
        private void DoStartupActivities()
        {
            // Logging
            XmlConfigurator.ConfigureAndWatch(
                new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reflexapilog.config")));

            // Define a scheduled job, tied to the ServerQueryJob class.
            IJobDetail srvListReqJob = JobBuilder.Create<ServerQueryJob>().WithIdentity("job1", "group1").Build();

            // Trigger the job (server list retrieval) to run 10 seconds from now, then every 90 seconds after that, forever
            ITrigger srvListReqTrigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartAt(DateBuilder.FutureDate(SecsWaitBeforeInitialRetrieval, IntervalUnit.Second))
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(DoRetrievalEverySecs).
                    RepeatForever())
                    .Build();

            // Schedule the job & start
            _scheduler.ScheduleJob(srvListReqJob, srvListReqTrigger);
            _scheduler.Start();
        }

        /// <summary>
        /// Configure the container with the necessary routes for this ServiceStack application.
        /// </summary>
        /// <param name="container">The built-in IoC used with ServiceStack.</param>
        public override void Configure(Container container)
        {
            // Reflex API will only provide JSON
            var disableFeatures = Feature.Jsv | Feature.Soap | Feature.Metadata | Feature.Xml |
                                      Feature.Html;
            SetConfig(new HostConfig
            {
                //DefaultRedirectPath = "empty.html",
                //Everything is enabled except jsv, soap, xml, & metadata/html pages
                EnableFeatures = Feature.All.Remove(disableFeatures),

                //Show StackTraces in service responses during development
                DebugMode = false,

                // Keep production errors as generic as possible
                WriteErrorsToResponse = true,

                DefaultContentType = MimeTypes.Json,
                AllowJsonpRequests = true,

                // In production, redirect from /api/ to the document root, which will be the standard server browser page.
                DefaultRedirectPath = "../"
            });

            //Handle exceptions occurring in services:
            ServiceExceptionHandlers.Add((httpReq, request, exception) =>
            {
                // Log the exception
                LoggerUtil.LogInfoAndDebug(string.Format("Service Exception: {0}", exception), MethodBase.GetCurrentMethod().DeclaringType);

                //call default exception handler or prepare a custom response
                return DtoUtils.CreateErrorResponse(request, exception);
            });

            //Handle unhandled exceptions occurring outside of services
            //E.g. Exceptions during Request binding or in filters:
            UncaughtExceptionHandlers.Add((req, res, operationName, ex) =>
            {
                LoggerUtil.LogInfoAndDebug(string.Format("Uncaught exception outside of service: {0}", ex),
                        MethodBase.GetCurrentMethod().DeclaringType);

                // Return a generic error message to user instead of all the internal exception info
                res.Write(@"{""error"":""Invalid request.""}");
                res.EndRequest(true);
            });
        }
    }
}