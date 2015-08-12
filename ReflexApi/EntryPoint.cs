namespace ReflexAPI
{
    using System;
    using System.Reflection;
    using Util;

    /// <summary>
    /// The entry point into the application.
    /// </summary>
    internal class EntryPoint
    {
        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            string hostAndPort = "http://*:29405/";
            int secsBetweenMasterQuery = 75;
            if (args.Length == 1)
            {
                if (args[0].StartsWith("http://"))
                {
                    hostAndPort = args[0];
                }
            }
            if (args.Length == 2)
            {
                if (args[0].StartsWith("http://"))
                {
                    hostAndPort = args[0];
                }
                int secs;
                if (int.TryParse(args[1], out secs) && secs >= 20)
                {
                    secsBetweenMasterQuery = secs;
                }

            }
            
            var v = typeof(EntryPoint).Assembly.GetName().Version.ToString().Split('.');
            var version = string.Format("{0}.{1}", v[0], v[1]);
            var launchingInfo = string.Format("Starting ReflexAPI v{0} by syncore <syncore@syncore.org>",
                version);
            var launchedInfo =
                string.Format("{0}ReflexAPI v{1} launched at {2} on {3} - querying Steam master every {4} seconds." +
                              "{5}To change, re-lanuch: '{6}.exe <http://ip:port/> [secondsBetweenMasterQueries]'",
                    Environment.NewLine, version, DateTime.Now, hostAndPort, secsBetweenMasterQuery,
                    Environment.NewLine, typeof(EntryPoint).Assembly.GetName().Name);
            
            LoggerUtil.LogInfoAndDebug(launchingInfo, MethodBase.GetCurrentMethod().DeclaringType);
            Console.WriteLine(launchingInfo);

            new ReflexAPIAppHost(secsBetweenMasterQuery).Init().Start(hostAndPort);

            LoggerUtil.LogInfoAndDebug(launchedInfo, MethodBase.GetCurrentMethod().DeclaringType);
            Console.WriteLine(launchedInfo);

            Console.WriteLine("Available endpoints: {0}servers, {0}queryserver, {0}unlistedquery", hostAndPort);
        }
    }
}
