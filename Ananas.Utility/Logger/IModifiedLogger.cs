using System;
using Microsoft.Extensions.Logging;
namespace Ananas.Utility.Logger {
    public interface IModifiedLogger {
        ILogger Logger { get; set; }
        void Log (LogLevel logLevel, Exception exception, string message, params object[] args);
        void Log (LogLevel logLevel, string message, params object[] args);
        void Log (LogLevel logLevel, string message, Exception exception, params object[] args);
        void Log (string message, Exception exception);
        void Log (string message);
    }
}