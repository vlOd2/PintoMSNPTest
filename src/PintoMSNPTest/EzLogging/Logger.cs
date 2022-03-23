using System;

namespace EzLogging
{
    public class Logger
    {
        private string logFormat;
        private LogLevel defaultLevel;

        /// <summary>
        /// A simple to use logging utility
        /// </summary>
        public Logger(string logFormat = "[{head}] {msg}", 
            LogLevel defaultLevel = LogLevel.INFO) 
        {
            this.logFormat = logFormat;
            this.defaultLevel = defaultLevel;
        }

        /// <summary>
        /// Function that fills the placeholders in the log format
        /// </summary>
        private string FillLogFormat(string logHeader, string logMessage) 
        {
            DateTime currentTime = DateTime.Now;

            return logFormat
                .Replace("{D}", currentTime.Day.ToString())
                .Replace("{M}", currentTime.Month.ToString())
                .Replace("{Y}", currentTime.Year.ToString())
                .Replace("{ms}", currentTime.Millisecond.ToString())
                .Replace("{s}", currentTime.Second.ToString())
                .Replace("{m}", currentTime.Minute.ToString())
                .Replace("{h}", currentTime.Hour.ToString())
                .Replace("{head}", logHeader)
                .Replace("{msg}", logMessage);
        }

        /// <summary>
        /// Logs to the screen with the default level
        /// </summary>
        public void Log(string message) 
        {
            string logFormatFilled = FillLogFormat(defaultLevel.ToString(), message);
            Console.WriteLine(logFormatFilled);
        }

        /// <summary>
        /// Logs to the screen with a custom level
        /// </summary>
        public void Log(LogLevel level, string message)
        {
            string logFormatFilled = FillLogFormat(level.ToString(), message);
            Console.WriteLine(logFormatFilled);
        }

        /// <summary>
        /// Logs to the screen with a custom header
        /// </summary>
        public void Log(string header, string message)
        {
            string logFormatFilled = FillLogFormat(header, message);
            Console.WriteLine(logFormatFilled);
        }

        /// <summary>
        /// Logs to the screen with the info verbose
        /// </summary>
        public void Verbose(string message)
        {
            string logFormatFilled = FillLogFormat(LogLevel.VERBOSE.ToString(), message);
            Console.WriteLine(logFormatFilled);
        }

        /// <summary>
        /// Logs to the screen with the info level
        /// </summary>
        public void Info(string message)
        {
            string logFormatFilled = FillLogFormat(LogLevel.INFO.ToString(), message);
            Console.WriteLine(logFormatFilled);
        }

        /// <summary>
        /// Logs to the screen with the warn level
        /// </summary>
        public void Warn(string message)
        {
            string logFormatFilled = FillLogFormat(LogLevel.WARN.ToString(), message);
            Console.WriteLine(logFormatFilled);
        }

        /// <summary>
        /// Logs to the screen with the error level
        /// </summary>
        public void Error(string message)
        {
            string logFormatFilled = FillLogFormat(LogLevel.ERROR.ToString(), message);
            Console.WriteLine(logFormatFilled);
        }

        /// <summary>
        /// Logs to the screen with the fatal level
        /// </summary>
        public void Fatal(string message)
        {
            string logFormatFilled = FillLogFormat(LogLevel.FATAL.ToString(), message);
            Console.WriteLine(logFormatFilled);
        }
    }
}
