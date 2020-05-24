using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Ananas.Utility.Logger {
    public class ModifiedDebugger : IModifiedLogger {
        public ILogger Logger { get; set; }
        public ModifiedDebugger (ILogger logger) {
            Logger = logger;
        }
        public ModifiedDebugger () {

        }
        public void Log (LogLevel logLevel, string message, params object[] args) {
            Logger?.Log (logLevel, message, args);
            Debug.WriteLine ($"{message}");
        }
        public void Log (LogLevel logLevel, string message, Exception exception, params object[] args) {
            Logger?.Log (logLevel, exception, message, args);
            Debug.WriteLine ($"{message}\n{exception.Message}\n{exception.StackTrace}\n{exception.TargetSite}\n{exception.Source}\n{exception.HelpLink}");
        }

        public void Log (LogLevel logLevel, Exception exception, string message, params object[] args) {
            Logger?.Log (logLevel, exception, message, args);
            Debug.WriteLine ($"{message}\n{exception.Message}\n{exception.StackTrace}\n{exception.TargetSite}\n{exception.Source}\n{exception.HelpLink}");
        }

        public void Log (string message, Exception exception) {
            Debug.WriteLine ($"{message}\n{exception.Message}\n{exception.StackTrace}\n{exception.TargetSite}\n{exception.Source}\n{exception.HelpLink}");
        }

        public void Log (string message) {
            Debug.WriteLine (message);
        }
    }
}