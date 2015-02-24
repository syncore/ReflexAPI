using System;
using System.Reflection;
using ReflexAPI.Util;

namespace ReflexAPI
{
    /// <summary>
    ///     The entry point into the application.
    /// </summary>
    internal class EntryPoint
    {
        /// <summary>
        ///     The main method.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            var listen = args.Length == 0 ? "http://*:29405/" : args[0];
            var version = typeof (EntryPoint).Assembly.GetName().Version;
            var launchingInfo = string.Format("Starting ReflexAPI v{0} by syncore <syncore@syncore.org>",
                version);
            var launchedInfo =
                string.Format("{0}ReflexAPI v{1} launched at {2} and is listening on {3}",
                    Environment.NewLine, version, DateTime.Now, listen);

            LoggerUtil.LogInfoAndDebug(launchingInfo, MethodBase.GetCurrentMethod().DeclaringType);
            Console.WriteLine(launchingInfo);

            new ReflexAPIAppHost().Init().Start(listen);

            LoggerUtil.LogInfoAndDebug(launchedInfo, MethodBase.GetCurrentMethod().DeclaringType);
            Console.WriteLine(launchedInfo);
        }
    }
}