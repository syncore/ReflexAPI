namespace ReflexAPI
{
    using System;
    using System.Reflection;
    using Quartz;
    using ReflexAPI.Util;

    /// <summary>
    /// Quartz Job class responsible for calling the ServerQueryProcessor at timed intervals to
    /// refresh the server list.
    /// </summary>
    public class ServerQueryJob : IJob
    {
        private static readonly Type LogClassType = MethodBase.GetCurrentMethod().DeclaringType;

        /// <summary>
        /// Called by the <see cref="T:Quartz.IScheduler"/> when a <see cref="T:Quartz.ITrigger"/>
        /// fires that is associated with the <see cref="T:Quartz.IJob"/>.
        /// </summary>
        /// <param name="context">The execution context.</param>
        /// <remarks>
        /// The implementation may wish to set a result object on the JobExecutionContext before
        /// this method exits. The result itself is meaningless to Quartz, but may be informative to
        /// <see cref="T:Quartz.IJobListener"/> s or <see cref="T:Quartz.ITriggerListener"/> s that
        /// are watching the job's execution.
        /// </remarks>
        public void Execute(IJobExecutionContext context)
        {
            var c = context.FireTimeUtc;
            var correctedFireTime = new DateTimeOffset();
            if (c != null)
            {
                // Quartz's FireTimeUtc is 5 hours ahead of where it should be. Set it back 5 hours.
                correctedFireTime = new DateTimeOffset(new DateTime(c.Value.Ticks), new TimeSpan(5, 0, 0));
            }
            LoggerUtil.LogInfoAndDebug(
                string.Format(
                    "Starting scheduled ServerQueryProcessor to retrieve server list. Scheduled time: {0}, Actual start time: {1}",
                    context.ScheduledFireTimeUtc, correctedFireTime.UtcDateTime), LogClassType);

            var sqp = new ServerQueryProcessor();
            if (sqp.GetAllServers())
            {
                return;
            }

            // Re-try once on master server query failure
            LoggerUtil.LogInfoAndDebug(
                string.Format(
                    "Server retrieval job that started at {0} failed to contact Steam Master server the first time, trying once more...",
                    correctedFireTime.UtcDateTime), LogClassType);
            sqp.GetAllServers();
        }
    }
}
