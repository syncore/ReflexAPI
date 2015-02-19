using System;
using System.Reflection;
using ReflexApi.Util;

namespace ReflexApi
{
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
            LoggerUtil.LogInfoAndDebug("Starting Reflex API...", MethodBase.GetCurrentMethod().DeclaringType);
            
            new ReflexApiAppHost().Init().Start(listen);

            LoggerUtil.LogInfoAndDebug(string.Format("ReflexAPI Host launched at {0} and is listening on {1}",
                DateTime.Now, listen), MethodBase.GetCurrentMethod().DeclaringType);
        }
    }
}