using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Logging.TableStorage
{
    public class TableStorageLogger : ILogger
    {

        private readonly TableServiceClient _client;
        private readonly TableClient _tableClient;
        private readonly TableStorageConfiguration _config;

        public TableStorageLogger(TableStorageConfiguration config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));

            _config = config;   
            _client = new TableServiceClient(_config.ConnectionString);

            var table = _client.CreateTableIfNotExists(_config.TableName);
            _tableClient = _client.GetTableClient(table.Value.Name);

        }

        #region Interface Implementation

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _config.AllowedLogLevels.Contains(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var msg = formatter(state, exception);
            var log = exception == null ? LogEntity.Create(_config.LoggerName, msg, logLevel) :
                                          LogEntity.Create(_config.LoggerName, msg, logLevel, exception);

            _tableClient.AddEntityAsync(log);
        } 

        #endregion


    }
}
