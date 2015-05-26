#region Copyright information
// <summary>
// <copyright file="Logger.cs">Copyright (c) 2015</copyright>
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
using System.IO;

namespace LogManager
{
    #region Enums

    /// <summary>
    /// The LogLevel Enum
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// The trace LogLevel
        /// </summary>
        Trace,
        /// <summary>
        /// The debug LogLevel
        /// </summary>
        Debug,
        /// <summary>
        /// The error LogLevel
        /// </summary>
        Error,
        /// <summary>
        /// The exception LogLevel
        /// </summary>
        Exception,
        /// <summary>
        /// The status LogLevel
        /// </summary>
        Status
    }

    /// <summary>
    /// The LogTarget Enum
    /// </summary>
    [Flags]
    public enum LogTarget
    {
        /// <summary>
        /// The console LogTarget
        /// </summary>
        Console = 1,
        /// <summary>
        /// The file LogTarget
        /// </summary>
        File = 2,
    }

    /// <summary>
    /// The LogfileNameType enum
    /// </summary>
    public enum LogfileNameType
    {
        /// <summary>
        /// A date as logfile name.
        /// </summary>
        Date,
        /// <summary>
        /// A rolling logfile name
        /// </summary>
        Rolling
    }

    #endregion

    class LogManager
    {
        #region Fields and Members

        object lockObject = new object();

        /// <summary>
        /// Gets or sets the Logtarget.
        /// </summary>
        /// <value>
        /// The logtarget. Console and/or File.
        /// </value>
        internal LogTarget Target { get; set; }

        private LogfileNameType fileNameType;
        /// <summary>
        /// Gets or sets the type of the file name.
        /// </summary>
        /// <value>
        /// The type of the file name.
        /// </value>
        /// <exception cref="System.NotSupportedException">More than one flag is not allowed for FileNameType</exception>
        internal LogfileNameType FileNameType
        {
            get { return fileNameType; }
            set
            {
                if ((value & (value - 1)) != 0) //has more than 1 flag
                {
                    throw new NotSupportedException("More than one flag is not allowed for FileNameType");
                }
                fileNameType = value;
            }
        }

        private LogLevel minimumLogLevel;
        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        /// <value>
        /// The log level.
        /// </value>
        /// <exception cref="System.NotSupportedException">More that one flag is not allowed for LogLevel</exception>
        internal LogLevel LogLevel
        {
            get { return minimumLogLevel; }
            set
            {
                if ((value & (value - 1)) != 0) //has more than 1 flag
                {
                    throw new NotSupportedException("More that one flag is not allowed for LogLevel");
                }
                minimumLogLevel = value;
            }
        }

        string filename;
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        internal string FileName { get { return filename; } set { filename = value; } }

        string filepath;
        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        internal string FilePath { get { return filepath; } set { filename = filepath; } }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LogManager"/> class.
        /// </summary>
        internal LogManager()
        {
            LogLevel = LogLevel.Trace;
            fileNameType = LogfileNameType.Date;
            filename = GetFileName();
            filepath = "Logs";
            Target = LogTarget.Console | LogTarget.File;
        }


        /// <summary>
        /// Logs the specified message with the loglevel in the file and/or the console.
        /// </summary>
        /// <param name="message">The message to write into the log.</param>
        /// <param name="level">The loglevel.</param>
        internal void Log(string message, LogLevel level)
        {
            if (level < LogLevel) return;

            var time = DateTime.Now;
            string stringToLog = String.Format("{0}:{1}:{2}.{3} [{4}] {5}", time.Hour, time.Minute, time.Second, time.Millisecond, level, message);

            if (Target.HasFlag(LogTarget.File))
            {
                lock (lockObject)
                {
                    Directory.CreateDirectory(filepath);
                    string path = Path.Combine(filepath, filename);
                    if (!File.Exists(path)) File.Create(path).Close();

                    using (var sw = File.AppendText(path))
                    {
                        sw.WriteLine(stringToLog);
                    }
                }
            }
            if (Target.HasFlag(LogTarget.Console))
            {
                Console.WriteLine(stringToLog);
            }
        }

        /// <summary>
        /// Returns a new Filename for a logfile.
        /// If fileNameType is set to Date then this will give a filename containing the current date.
        /// If fileNameType is set to Rolling then this will rename the old logfiles and return "session.0.log.txt".
        /// </summary>
        /// <returns></returns>
        private string GetFileName()
        {
            if (fileNameType == LogfileNameType.Date)
            {
                var time = DateTime.Now;
                return String.Format("session.{0}.{1}.{2}_{3}.{4}.{5}.log.txt", time.Day, time.Month, time.Year, time.Hour, time.Minute, time.Second);
            }
            else //if (FileNameType == LogFileNameType.Rolling)
            {
                string obsoleteFile = Path.Combine(filepath, "session.9.log.txt");
                File.Delete(obsoleteFile);

                for (int i = 8; i < 0; i--)
                {
                    string oldFilename = String.Format("session.{0}.log.txt", i);
                    string newFilename = String.Format("session.{0}.log.txt", i + 1);

                    if (File.Exists(oldFilename))
                    {
                        File.Move(oldFilename, newFilename);
                    }
                }

                return "session.0.log.txt";
            }
        }
    }
}
