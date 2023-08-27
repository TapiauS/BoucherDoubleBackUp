using System.Diagnostics.CodeAnalysis;

namespace Boucher_Double_Back_End.Logging
{
    public class BoucherDoubleFileLogger:ILogger
    {
        protected readonly BoucherDoubleLoggerProvider _provider;  
        
        public BoucherDoubleFileLogger([NotNull] BoucherDoubleLoggerProvider provider)
        {
            _provider = provider;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _provider.Options.LogLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if(!IsEnabled(logLevel))
            {
                return;
            }
            string fullFilePath = string.Format("{0}/{1}", _provider.Options.FolderPath, _provider.Options.FilePath.Replace("{date}",DateTime.UtcNow.ToString("yyyyMMdd")));
            string logRecord = string.Format("{0} [{1}] {2} {3} Error Message:{4}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:Gss"), logLevel.ToString(), formatter(state, exception), exception != null ? exception.StackTrace : "", exception != null ? exception.Message : "");
            using StreamWriter streamWriter = new(fullFilePath,true);
            streamWriter.WriteLine(logRecord);
        }
    }
}
