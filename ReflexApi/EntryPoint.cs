namespace ReflexAPI
{
    using System;
    using System.Reflection;
    using ReflexAPI.Util;

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
            var listen = args.Length == 0 ? "http://*:29405/" : args[0];
            var v = typeof(EntryPoint).Assembly.GetName().Version.ToString().Split('.');
            var version = string.Format("{0}.{1}", v[0], v[1]);
            var launchingInfo = string.Format("Starting ReflexAPI v{0} by syncore <syncore@syncore.org>",
                version);
            var launchedInfo =
                string.Format("{0}ReflexAPI v{1} launched at {2} on {3}{4}To change, re-lanuch: '{5}.exe http://ip:port/'",
                    Environment.NewLine, version, DateTime.Now, listen, Environment.NewLine, typeof(EntryPoint).Assembly.GetName().Name);
            
            LoggerUtil.LogInfoAndDebug(launchingInfo, MethodBase.GetCurrentMethod().DeclaringType);
            Console.WriteLine(launchingInfo);

            new ReflexAPIAppHost().Init().Start(listen);

            LoggerUtil.LogInfoAndDebug(launchedInfo, MethodBase.GetCurrentMethod().DeclaringType);
            Console.WriteLine(launchedInfo);

            Console.WriteLine("Available endpoints: /servers, /queryserver, /unlistedquery");
        }
    }
}
