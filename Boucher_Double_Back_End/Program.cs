
using Boucher_Double_Back_End.Logging;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Add services to the container.
var loggerOptions = new BoucherDoubleFileLoggerOption
{
    FolderPath = configuration.GetSection("Logging:BoucherDoubleFile:Options:FolderPath").Value,
    FilePath = configuration.GetSection("Logging:BoucherDoubleFile:Options:FilePath").Value,
    LogLevel = BoucherDoubleFileLoggerOption.GetFromString(configuration.GetSection("Logging:BoucherDoubleFile:LogLevel:Default").Value)
};

builder.Services.AddScoped<ILogger>(serviceProvider =>
{
    var fileLogger = new BoucherDoubleFileLogger(new BoucherDoubleLoggerProvider(loggerOptions));
    return fileLogger;
});
builder.Services.AddSwaggerGen();
builder.Services.AddSession(session =>
{
    session.IdleTimeout = TimeSpan.FromMinutes(60);
    session.Cookie.HttpOnly = true;
    session.Cookie.SecurePolicy = CookieSecurePolicy.Always;
}
    );
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseFileServer();
app.UseAuthorization();
app.UseSession();
app.MapControllers();

try
{
    app.Run();
}
catch(Exception ex)
{
    var fileLogger = new BoucherDoubleFileLogger(new BoucherDoubleLoggerProvider(loggerOptions));
    fileLogger.LogError("Erreur globale", ex.Message);
}

