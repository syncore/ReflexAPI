using System;
using System.Reflection;
using log4net;

namespace ReflexApi.Util
{
    /// <summary>
    ///     Utility class for logging operations.
    /// </summary>
    public static class LoggerUtil
    {
        public static readonly Type LogClassType = MethodBase.GetCurrentMethod().DeclaringType;
        public static readonly ILog Logger = LogManager.GetLogger(LogClassType);
        
        /// <summary>
        ///     Logs the current message to both the loggers specified for handling information and debug messages.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        /// <param name="type">The type of class that initiated the log request.</param>
        public static void LogInfoAndDebug(string msg, Type type)
        {
            ILog logger = LogManager.GetLogger(type);
            logger.Debug(msg);
            logger.Info(msg);
        }
    }
}