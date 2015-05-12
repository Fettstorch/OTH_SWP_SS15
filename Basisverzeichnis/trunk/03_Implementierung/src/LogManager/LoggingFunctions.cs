using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace LogManager
{
    public static class LoggingFunctions
    {
        private static LogManager logger;

        static LoggingFunctions()
        {
            logger = new LogManager();
        }

        public static void Trace(object toLog)
        {
            Log(toLog, LogLevel.Trace);
        }

        public static void Debug(object toLog)
        {
            Log(toLog, LogLevel.Debug);
        }

        public static void Error(object toLog)
        {
            Log(toLog, LogLevel.Error);
        }

        public static void Exception(object toLog)
        {
            Log(toLog, LogLevel.Exception);
        }

        public static void Status(object toLog)
        {
            Log(toLog, LogLevel.Status);
        }

        public static void Log(object toLog, LogLevel level)
        {
            if (toLog == null) return;

            string message = toLog.ToString();

            if (toLog is IList || toLog is Array || toLog is IDictionary)
            {
                var enumerable = toLog as IEnumerable;

                if (enumerable != null)
                {
                    message = String.Empty;
                    foreach (var part in enumerable)
                    {
                        message += String.Format("{0}" + Environment.NewLine, part.ToString());
                    }
                }
            }
            logger.Log(message, level);    
        }

        public static void SetLogLevel(LogLevel level)
        {
            logger.SetLogLevel(level);
        }

    }
}
