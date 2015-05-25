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

    public enum LogfileNameType
    {
        Date,
        Rolling
    }

    #endregion

    class LogManager
    {
        #region Fields and Members

        object lockObject = new object();

        public LogTarget Target { get; set; }

        private LogfileNameType fileNameType;
        public LogfileNameType FileNameType
        {
            get { return fileNameType; }
            set
            {
                if ((value & (value - 1)) != 0) //has more than 1 flag
                {
                    throw new NotSupportedException("More that one flag is not allowed for FileNameType");
                }
                fileNameType = value;
            }
        }

        private LogLevel minimumLogLevel;
        public LogLevel LogLevel
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
        public string FileName { get { return filename; } set { filename = value; } }

        string filepath;
        public string FilePath { get { return filepath; } set { filename = filepath; } }

        #endregion

        public LogManager()
        {
            LogLevel = LogLevel.Trace;
            fileNameType = LogfileNameType.Date;
            filename = GetFileName();
            filepath = "Logs";
            Target = LogTarget.Console | LogTarget.File;
        }

        public void Log(string message, LogLevel level)
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
