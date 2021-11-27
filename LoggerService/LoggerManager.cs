using Contracts;
using NLog;
public class LoggerManager  : ILoggerManager
{
    private static NLog.ILogger logger = LogManager.GetCurrentClassLogger();
    public LoggerManager()
    {
        
    }
    public void LogDebug(string message)
    {
        logger.Debug(message);
    }
    // log error
    public void LogError(string message)
    {
        logger.Error(message);
    }
    // log info
    public void LogInfo(string message)
    {
        logger.Info(message);
    }
    // log warn
    public void LogWarn(string message)
    {
        logger.Warn(message);
    }
}