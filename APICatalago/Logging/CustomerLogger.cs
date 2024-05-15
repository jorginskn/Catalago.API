namespace APICatalago.Logging
{
    public class CustomerLogger : ILogger
    {
        private string loggerName;
        readonly CustomLoggerProviderConfiguration loggerConfig;
        public CustomerLogger(string name, CustomLoggerProviderConfiguration config)
        {
            loggerName = name;
            loggerConfig = config;
        }


        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == loggerConfig.LogLevel;
        }
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            string message = $"{logLevel.ToString()} : {eventId.Id} - {formatter(state, exception)}";
            WriteLogMessage(message);
        }

        private void WriteLogMessage(string message)
        {
            string LogFilePath = @"C:\dados\log\Macoratti_Log.txt";
            using (StreamWriter streamWriter = new StreamWriter(LogFilePath))
            {
                try
                {
                    streamWriter.WriteLine(message);
                    streamWriter.Close();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
