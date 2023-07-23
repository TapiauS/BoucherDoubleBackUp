namespace Boucher_Double_Back_End.Logging
{
    public class BoucherDoubleFileLoggerOption
    {
        public virtual string FilePath { get; set; }
        public virtual string FolderPath { get; set; }

        public virtual LogLevel LogLevel { get; set; }

        public static LogLevel GetFromString(string logLevel)
        {
            foreach(var item in Enum.GetValues(typeof(LogLevel)))
            {
                if (item.ToString() == logLevel)
                {
                    return (LogLevel)item;
                }
            }
            return LogLevel.None;
        }
    }
}
