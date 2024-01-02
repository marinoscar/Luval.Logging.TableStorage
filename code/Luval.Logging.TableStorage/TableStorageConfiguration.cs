using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Logging.TableStorage
{
    public class TableStorageConfiguration
    {
        public TableStorageConfiguration(string connectionString, string loggerName)
        {
            if (string.IsNullOrWhiteSpace(loggerName)) throw new ArgumentNullException(nameof(loggerName));
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));

            ConnectionString = connectionString;
            LoggerName = loggerName;
            TableName = "LogEvents";
            AllowedLogLevels = new List<LogLevel>() { LogLevel.Debug, LogLevel.Trace, 
                LogLevel.Information, LogLevel.Warning, LogLevel.Error, LogLevel.Critical };
        }

        public string ConnectionString { get; private set; }
        public string TableName { get; set; }
        public string LoggerName { get; private set; }
        public IList<LogLevel> AllowedLogLevels { get; set; }

    }
}
