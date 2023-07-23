using Microsoft.Extensions.Options;

namespace Boucher_Double_Back_End.Logging
{
    public class BoucherDoubleLoggerProvider:ILoggerProvider
    {
        public readonly BoucherDoubleFileLoggerOption Options;

        public BoucherDoubleLoggerProvider(BoucherDoubleFileLoggerOption options) 
        {
            Options = options;

            if (!Directory.Exists(Options.FolderPath))
            {
                Directory.CreateDirectory(Options.FolderPath);
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new BoucherDoubleFileLogger(this);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
