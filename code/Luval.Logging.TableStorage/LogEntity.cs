using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Luval.Logging.TableStorage
{
    public class LogEntity : ITableEntity
    {
        public LogEntity(string loggerName)
        {
            RowKey = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
            PartitionKey = loggerName;
            MachineName = GetMachineName() ?? string.Empty;
            Level = string.Empty;
            Message = string.Empty;
            ExceptionType = string.Empty;
            Exception = string.Empty;
            StackTrace = string.Empty;
            Timestamp = DateTimeOffset.UtcNow;
        }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string ExceptionType { get; set; }
        public string Exception { get; set; }
        public string StackTrace { get; set; }
        public string MachineName { get; set; }


        public static LogEntity Create(string loggerName, string message, LogLevel level)
        {
            return new LogEntity(loggerName)
            {
                Message = message, Level = Enum.GetName<LogLevel>(level)
            };
        }

        public static LogEntity Create(string loggerName, string message, LogLevel level, Exception exception)
        {
            if(exception == null) throw new ArgumentNullException("exception");
            return new LogEntity(loggerName)
            {
                Message = message,
                Level = Enum.GetName<LogLevel>(level),
                Exception = exception.Message,
                StackTrace = exception.ToString(),
                ExceptionType = exception.GetType().Name
            };
        }

        private static string GetMachineName()
        {
            return TryLookupValue(() => Environment.GetEnvironmentVariable("COMPUTERNAME"), "COMPUTERNAME")
                ?? TryLookupValue(() => Environment.GetEnvironmentVariable("HOSTNAME"), "HOSTNAME")
                ?? TryLookupValue(() => Environment.MachineName, "MachineName")
                ?? TryLookupValue(() => System.Net.Dns.GetHostName(), "DnsHostName");
        }

        private static string TryLookupValue(Func<string> lookupFunc, string lookupType)
        {
            try
            {
                string lookupValue = lookupFunc()?.Trim();
                return string.IsNullOrEmpty(lookupValue) ? null : lookupValue;
            }
            catch (Exception ex)
            {
                Trace.TraceWarning("Failed to get Machine Name: Failed to lookup {0}", lookupType);
                return null;
            }
        }
    }
}
