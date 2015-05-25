#region Copyright information
// <summary>
// <copyright file="LoggingFunctions.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>06/05/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace LogManager
{
    /// <summary>
    /// This is the public static access class for logging.
    /// You can log Arrays, Lists and Dictionarys and other objects.
    /// The objects will be logged with their .ToString() functions.
    /// See README.txt for more details and implementation.
    /// </summary>
    public static class LoggingFunctions
    {
        private static LogManager logger;

        /// <summary>
        /// Initializes the <see cref="LoggingFunctions"/> class.
        /// </summary>
        static LoggingFunctions()
        {
            logger = new LogManager();
        }

        /// <summary>
        /// Writes the object as Trace to the log.
        /// </summary>
        /// <param name="toLog">Object to log.</param>
        public static void Trace(object toLog)
        {
            Log(toLog, LogLevel.Trace);
        }

        /// <summary>
        /// Writes the object as Debug to the log.
        /// </summary>
        /// <param name="toLog">Object to log.</param>
        public static void Debug(object toLog)
        {
            Log(toLog, LogLevel.Debug);
        }

        /// <summary>
        /// Writes the object as Error to the log.
        /// </summary>
        /// <param name="toLog">Object to log.</param>
        public static void Error(object toLog)
        {
            Log(toLog, LogLevel.Error);
        }

        /// <summary>
        /// Writes the object as Exception to the log.
        /// </summary>
        /// <param name="toLog">Object to log.</param>
        public static void Exception(object toLog)
        {
            Log(toLog, LogLevel.Exception);
        }

        /// <summary>
        /// Writes the object as Status to the log.
        /// </summary>
        /// <param name="toLog">Object to log.</param>
        public static void Status(object toLog)
        {
            Log(toLog, LogLevel.Status);
        }

        /// <summary>
        /// Writes the object with a defined LogLevel to the log.
        /// </summary>
        /// <param name="toLog">Object to log.</param>
        /// <param name="level">The LogLevel.</param>
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

        /// <summary>
        /// Gets or sets the minimum LogLevel.
        /// Everything passed to Log(...) with a LogLevel below will be ignored.
        /// </summary>
        /// <param name="level">The LogLevel. Everything, passed to the Log(...) function, below the specified Loglevel will be ignored.</param>
        public static LogLevel SetLogLevel
        {
            get { return logger.LogLevel; }
            set { logger.LogLevel = value; }
        }

        /// <summary>
        /// Gets or sets the name of the LogFile.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public static string FileName
        {
            get { return logger.FileName; }
            set { logger.FileName = value; }
        }

        /// <summary>
        /// Gets or sets the path of the LogFile.
        /// Can be a relative or absolute path.
        /// </summary>
        /// <param name="path">Can be a relative or absolute path.</param>
        public static string FilePath
        {
            get { return logger.FilePath; }
            set { logger.FilePath = value; }
        }

        /// <summary>
        /// Gets or sets the type of the logfile name.
        /// Can be LogfileNameType.Date or LogfileNameType.Rolling
        /// </summary>
        /// <param name="type">The logfile name type. Can be LogfileNameType.Date or LogfileNameType.Rolling.</param>
        public static LogfileNameType LogfileNameType
        {
            get { return logger.FileNameType; }
            set { logger.FileNameType = value; }
        }

        /// <summary>
        /// Gets or sets the Logtarget.
        /// Can be LogTarget.Console and/or LogTarget.File
        /// </summary>
        /// <param name="target">The logging target. Can be LogTarget.Console and/or LogTarget.File.</param>
        public static LogTarget LogTarget
        {
            get { return logger.Target; }
            set { logger.Target = value;}
        }
    }
}
