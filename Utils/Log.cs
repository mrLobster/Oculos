using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using static System.Console;

namespace Bergfall.Oculos.Utils
{
    public static class Log
    {
        public static void WriteLine(string message)
        {
            Console.WriteLine("(Thread {0}): {1}", Thread.CurrentThread.ManagedThreadId, message);
        }

        public static void WriteLine(string message, string filename)
        {
            File.AppendAllText(filename, message);
        }
        public static void WriteLine(int data)
        {
            WriteLine("Value = " + data);
        }



        public static void DisplayString(string text)
        {
            Console.WriteLine("String length: {0}", text.Length);
            foreach (char c in text)
            {
                if (c < 32)
                {
                    Console.WriteLine("<{0}> U+{1:x4}", (int)c);
                }
                else if (c > 127)
                {
                    Console.WriteLine("(Possibly non-printable) U+{0:x4}", (int)c);
                }
                else
                {
                    Console.WriteLine("{0} U+{1:x4}", c, (int)c);
                }
            }
        }

        public static void Debug(string message)
        {
            WriteLine(message);
        }

        public static void Debug(object message)
        {
            Debug(message.ToString());
        }

        public static void Write(object message)
        {
            write(message.ToString());
        }

        public static async void WriteToFile(IEnumerable<object> items)
        {
            using (StringWriter sw = new StringWriter())
            {
                foreach (var msg in items)
                {
                    await sw.WriteLineAsync(msg.ToString()).ConfigureAwait(false);
                }
                write(sw.ToString());
            }
        }

        private static async void write(string message)
        {
            string dir = Directory.GetCurrentDirectory() + @"\log.txt";

            using (StreamWriter sw = new StreamWriter(File.OpenWrite(dir)))
            {
                await sw.WriteAsync(message).ConfigureAwait(false);
            }
        }

        public static void Error(string message)
        {
            ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
        }
    }

}
/// <summary>
/// Default logger is to Console.WriteLine
/// 
/// Made public so its testable
/// </summary>
public class ConsoleLogger : ILog
{
    const string DEBUG = "DEBUG: ";
    const string ERROR = "ERROR: ";
    const string FATAL = "FATAL: ";
    const string INFO = "INFO: ";
    const string WARN = "WARN: ";

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugLogger"/> class.
    /// </summary>
    public ConsoleLogger(string type)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DebugLogger"/> class.
    /// </summary>
    public ConsoleLogger(Type type)
    {
    }

    public bool IsDebugEnabled { get; set; }

    /// <summary>
    /// Logs the specified message.
    /// </summary>
    private static void Log(object message, Exception exception)
    {
        var msg = message?.ToString() ?? string.Empty;
        if (exception != null)
        {
            msg += ", Exception: " + exception.Message;
        }
        Console.WriteLine(msg);
    }

    /// <summary>
    /// Logs the format.
    /// </summary>
    private static void LogFormat(object message, params object[] args)
    {
        string msg = message?.ToString() ?? string.Empty;
        Console.WriteLine(msg, args);
    }

    /// <summary>
    /// Logs the specified message.
    /// </summary>
    private static void Log(object message)
    {
        string msg = message?.ToString() ?? string.Empty;
        Console.WriteLine(msg);
    }

    public void Debug(object message, Exception exception)
    {
        Log(DEBUG + message, exception);
    }

    public void Debug(object message)
    {
        Log(DEBUG + message);
    }

    public void DebugFormat(string format, params object[] args)
    {
        LogFormat(DEBUG + format, args);
    }

    public void Error(object message, Exception exception)
    {
        Log(ERROR + message, exception);
    }

    public void Error(object message)
    {
        Log(ERROR + message);
    }

    public void ErrorFormat(string format, params object[] args)
    {
        LogFormat(ERROR + format, args);
    }

    public void Fatal(object message, Exception exception)
    {
        Log(FATAL + message, exception);
    }

    public void Fatal(object message)
    {
        Log(FATAL + message);
    }

    public void FatalFormat(string format, params object[] args)
    {
        LogFormat(FATAL + format, args);
    }

    public void Info(object message, Exception exception)
    {
        Log(INFO + message, exception);
    }

    public void Info(object message)
    {
        Log(INFO + message);
    }

    public void InfoFormat(string format, params object[] args)
    {
        LogFormat(INFO + format, args);
    }

    public void Warn(object message, Exception exception)
    {
        Log(WARN + message, exception);
    }

    public void Warn(object message)
    {
        Log(WARN + message);
    }

    public void WarnFormat(string format, params object[] args)
    {
        LogFormat(WARN + format, args);
    }
}
public interface ILog
{
    /// <summary>
    /// Gets or sets a value indicating whether this instance is debug enabled.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is debug enabled; otherwise, <c>false</c>.
    /// </value>
    bool IsDebugEnabled { get; }

    /// <summary>
    /// Logs a Debug message.
    /// </summary>
    /// <param name="message">The message.</param>
    void Debug(object message);

    /// <summary>
    /// Logs a Debug message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    void Debug(object message, Exception exception);

    /// <summary>
    /// Logs a Debug format message.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    void DebugFormat(string format, params object[] args);

    /// <summary>
    /// Logs a Error message.
    /// </summary>
    /// <param name="message">The message.</param>
    void Error(object message);

    /// <summary>
    /// Logs a Error message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    void Error(object message, Exception exception);

    /// <summary>
    /// Logs a Error format message.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    void ErrorFormat(string format, params object[] args);

    /// <summary>
    /// Logs a Fatal message.
    /// </summary>
    /// <param name="message">The message.</param>
    void Fatal(object message);

    /// <summary>
    /// Logs a Fatal message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    void Fatal(object message, Exception exception);

    /// <summary>
    /// Logs a Error format message.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    void FatalFormat(string format, params object[] args);

    /// <summary>
    /// Logs an Info message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    void Info(object message);

    /// <summary>
    /// Logs an Info message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    void Info(object message, Exception exception);

    /// <summary>
    /// Logs an Info format message.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    void InfoFormat(string format, params object[] args);

    /// <summary>
    /// Logs a Warning message.
    /// </summary>
    /// <param name="message">The message.</param>
    void Warn(object message);

    /// <summary>
    /// Logs a Warning message and exception.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="exception">The exception.</param>
    void Warn(object message, Exception exception);

    /// <summary>
    /// Logs a Warning format message.
    /// </summary>
    /// <param name="format">The format.</param>
    /// <param name="args">The args.</param>
    void WarnFormat(string format, params object[] args);
}