namespace Boucher_Double_Back_End.Logging
{
    public static class BoucherDoubleFileLoggerExtensions
    {
        public static ILoggingBuilder AddBoucherDoubleFileLogger(this ILoggingBuilder builder,Action<BoucherDoubleFileLoggerOption> configure) 
        {
            builder.Services.AddSingleton<ILoggerProvider, BoucherDoubleLoggerProvider>();
            builder.Services.Configure(configure);

            return builder;
        }
    }
}
