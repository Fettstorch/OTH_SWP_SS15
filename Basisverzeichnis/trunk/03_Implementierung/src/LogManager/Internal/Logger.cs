using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LogManager
{
    #region Enums

    public enum LogLevel
    {
        Trace,
        Debug,
        Error,
        Exception,
        Status
    }

    [Flags]
    public enum LogTarget
    {
        Console = 1,
        File = 2,
    }

    public enum LogFileNameType
    {
        Date,
        Rolling
    }

    #endregion

    class LogManager
    {
        string Filename;
        object lockObject = new object();
        LogFileNameType FileNameType;
        LogTarget Target;
        string LogPath;
        LogLevel minimumLogLevel;

        public LogManager()
        {
            minimumLogLevel = LogLevel.Trace;
            FileNameType = LogFileNameType.Date;
            Filename = GetFileName();
            LogPath = "Logs";
            Target = LogTarget.Console;
        }

        public void Log(string message, LogLevel level)
        {
            if (level < minimumLogLevel) return;

            var time = DateTime.Now;
            string stringToLog = String.Format("{0}:{1}:{2}.{3} [{4}] {5}", time.Hour, time.Minute, time.Second, time.Millisecond, level, message);

            if (Target.HasFlag(LogTarget.File))
            {
                lock (lockObject)
                {
                    if (!File.Exists(Filename)) File.Create(Filename).Close();

                    using (var sw = File.AppendText(Filename))
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

        private string GetFileName()
        {
            if (FileNameType == LogFileNameType.Date)
            {
                var time = DateTime.Now;
                return String.Format("session.{0}.{1}.{2}_{3}.{4}.{5}", time.Day, time.Month, time.Year, time.Hour, time.Minute, time.Second);
            }
            else //if (FileNameType == LogFileNameType.Rolling)
            {
                string obsoleteFile = Path.Combine(LogPath, "session.9.log");
                File.Delete(obsoleteFile);

                for (int i = 8; i < 0; i--)
                {
                    string oldFilename = String.Format("session.{0}.log", i);
                    string newFilename = String.Format("session.{0}.log", i + 1);

                    if (File.Exists(oldFilename))
                    {
                        File.Move(oldFilename, newFilename);
                    }
                }

                return "session.0.log";
            }
        }

        internal void SetLogLevel(LogLevel level)
        {
            minimumLogLevel = level;
        }
    }
}
