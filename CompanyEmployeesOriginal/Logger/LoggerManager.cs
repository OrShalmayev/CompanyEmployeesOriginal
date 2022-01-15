using System;
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
    public void LogInfo(string message, bool isCustomMessage = false)
    {
        logger.Info(message);
    }

    public void LogInfo(string message)
    {
        throw new NotImplementedException();
    }

    // log warn
    public void LogWarn(string message)
    {
        logger.Warn(message);
    }
}
public static class LoggerCustomMessages
{
    public const string
        IdNotFoundInDB = "{0} with id: {1} doesn't exist in the database.",
        ObjectFromClientIsNull = "{0} object sent from client is null.",
        ParameterIsNull = "Parameter {0} is null.",
        CollectionParametersIsNull = "Some {0} are not valid in a {1} collection",
        CollectionFromClientIsNull = "{0} collection is null";
}
