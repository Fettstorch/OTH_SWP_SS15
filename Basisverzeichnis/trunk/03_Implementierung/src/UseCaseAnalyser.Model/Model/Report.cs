using System.Collections.Generic;

namespace UseCaseAnalyser.Model.Model
{
    public class Report
    {

        public enum Entrytype
        {
            ERROR, WARNING, LOG, DEFAULT
        }

        public ReportEntry[] ErrorReportEntries
        {
            get { return this.mReportErrorEntries.ToArray(); }
        }

        public ReportEntry[] WarningReportEntries
        {
            get { return this.mReportWarningEntries.ToArray(); }
        }

        public ReportEntry[] LogReportEntries
        {
            get { return this.mReportLogEntries.ToArray(); }
        }

        private readonly List<ReportEntry> mReportErrorEntries = new List<ReportEntry>();

        private readonly List<ReportEntry> mReportWarningEntries = new List<ReportEntry>();

        private readonly List<ReportEntry> mReportLogEntries = new List<ReportEntry>();

        public void AddReportEntry(ReportEntry entry)
        {
            switch (entry.Type)
            {
                case Entrytype.ERROR: this.mReportErrorEntries.Add(entry);
                    break;
                case Entrytype.WARNING: this.mReportWarningEntries.Add(entry);
                    break;
                case Entrytype.LOG: this.mReportLogEntries.Add(entry);
                    break;
            }
        }

        public List<ReportEntry> GetEntriesByTag(string tag, Entrytype type = Entrytype.DEFAULT)
        {
            List<ReportEntry> entries = new List<ReportEntry>();
            entries.AddRange(this.mReportErrorEntries.FindAll(x => string.Equals(x.Tag, tag)));
            entries.AddRange(this.mReportWarningEntries.FindAll(x => string.Equals(x.Tag, tag)));
            entries.AddRange(this.mReportLogEntries.FindAll(x => string.Equals(x.Tag, tag)));
            return entries;
        }

        public class ReportEntry
        {
            public string Heading { get; private set; }
            public string Content { get; private set; }
            public Entrytype Type { get; private set; }
            public string Tag { get; private set; }

            public ReportEntry(string heading, string content, Entrytype type, string tag = "")
            {
                Heading = heading;
                Content = content;
                Type = type;
                Tag = tag;
            }
        }
    }
}
