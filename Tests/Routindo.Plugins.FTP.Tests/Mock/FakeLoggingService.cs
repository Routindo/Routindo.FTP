using System;
using Routindo.Contract.Services;

namespace Routindo.Plugins.FTP.Tests.Mock
{
    public class FakeLoggingService : ILoggingService
    {
        private readonly string _name;
        private readonly Type _type;

        private void Log(string level, string message)
        {
            Console.WriteLine(_type != null
                ? $"[{level.ToUpper().PadRight(5)}][{DateTime.Now:G}][{_type.Name}][{_name}] {string.Format(message)}"
                : $"[{level.ToUpper().PadRight(5)}][{DateTime.Now:G}][{_name}] {string.Format(message)}");
        }

        private void Log<T>(string level, T value)
        {
            Log(level, value.ToString());
            //Console.WriteLine(_type != null
            //    ? $"[{level.ToUpper().PadRight(5)}][{DateTime.Now:G}][{_type.Name}][{_name}] {value}"
            //    : $"[{level.ToUpper().PadRight(5)}][{DateTime.Now:G}][{_name}] {value}");
        }

        private void Log(string level, string message, params object[] args)
        {
            var messageWithArgs = string.Format(message, args);
            Log(level, messageWithArgs);
        }

        private void Log(string level, Exception exception, string message, params object[] args)
        {
            var messageWithArgs = string.Format(message, args);
            Console.WriteLine(
                _type != null
                    ? $"[{level.ToUpper().PadRight(5)}][{DateTime.Now:G}][{_type.Name}][{_name}] {exception} {Environment.NewLine} {messageWithArgs}"
                    : $"[{level.ToUpper().PadRight(5)}][{DateTime.Now:G}][{_name}] {exception} {Environment.NewLine} {messageWithArgs}");
        }

        public FakeLoggingService(string name, Type type = null)
        {
            _name = name;
            _type = type;
        }

        public void Trace(string message)
        {
            Log(nameof(Trace), message);
        }

        public void Trace<T>(T value)
        {
            Log(nameof(Trace), value);
        }

        public void Trace(string message, params object[] args)
        {
            Log(nameof(Trace), message, args);
        }

        public void Trace(Exception exception, string message, params object[] args)
        {
            Log(nameof(Trace), exception, message, args);
        }

        public void Debug(string message)
        {
            Log(nameof(Debug), message);
        }

        public void Debug<T>(T value)
        {
            Log(nameof(Debug), value);
        }

        public void Debug(string message, params object[] args)
        {
            Log(nameof(Debug), message, args);
        }

        public void Debug(Exception exception, string message, params object[] args)
        {
            Log(nameof(Debug), exception, message, args);
        }

        public void Info(string message)
        {
            Log(nameof(Info), message);
        }

        public void Info<T>(T value)
        {
            Log(nameof(Info), value);
        }

        public void Info(string message, params object[] args)
        {
            Log(nameof(Info), message, args);
        }

        public void Info(Exception exception, string message, params object[] args)
        {
            Log(nameof(Info), exception, message, args);
        }

        public void Warn(string message)
        {
            Log(nameof(Warn), message);
        }

        public void Warn<T>(T value)
        {
            Log(nameof(Warn), value);
        }

        public void Warn(string message, params object[] args)
        {
            Log(nameof(Warn), message, args);
        }

        public void Warn(Exception exception, string message, params object[] args)
        {
            Log(nameof(Warn), exception, message, args);
        }

        public void Error(string message)
        {
            Log(nameof(Error), message);
        }

        public void Error<T>(T value)
        {
            Log(nameof(Error), value);
        }

        public void Error(string message, params object[] args)
        {
            Log(nameof(Error), message, args);
        }

        public void Error(Exception exception, string message, params object[] args)
        {
            Log(nameof(Error), exception, message, args);
        }

        public void Fatal(string message)
        {
            Log(nameof(Fatal), message);
        }

        public void Fatal<T>(T value)
        {
            Log(nameof(Fatal), value);
        }

        public void Fatal(string message, params object[] args)
        {
            Log(nameof(Fatal), message, args);
        }

        public void Fatal(Exception exception, string message, params object[] args)
        {
            Log(nameof(Fatal), exception, message, args);
        }
    }
}