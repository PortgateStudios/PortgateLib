using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using UnityEngine;

namespace PortgateLib.Logger
{
	public static class Logger
	{
		public static string LogDirectory
		{
			get
			{
				var logDirectory = Path.Combine(Application.persistentDataPath, LOG_FOLDER_NAME);
				if (!Directory.Exists(logDirectory))
				{
					Directory.CreateDirectory(logDirectory);
				}
				return logDirectory;
			}
		}

		private static readonly string LOG_FOLDER_NAME = "Logs";
		private static readonly int MAX_LOG_COUNT = 30;            // How many logs will be stored without being deleted
		private static readonly int MESSAGE_SPACING = 96;          // How many right spaces (left aligned) the log message will have
		private static readonly int STACK_TRACE_SPACING = 128;     // How many right spaces (left aligned) the log stack trace will have
		private static readonly int MAX_ERROR_PER_SECOND = 30;     // How many errors/exceptions are logged per second before turning off logging.

		private static int errorCountInCurrentSecond;
		private static int currentSecond;

		private static bool IsSpammingErrors
		{
			get { return errorCountInCurrentSecond > MAX_ERROR_PER_SECOND; }
		}

		private static string _logFile;
		private static string LogFile
		{
			get
			{
				if (_logFile == null)
				{
					_logFile = CreateLogFile();
				}
				return _logFile;
			}
		}

		private static string CreateLogFile()
		{
			var now = DateTime.Now;
			var logFileName = $"{now.Year:0000}.{now.Month:00}.{now.Day:00} - {now.Hour:00}.{now.Minute:00}.{now.Second:00}.log";
			return Path.Combine(LogDirectory, logFileName);
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void Do()
		{
			Application.logMessageReceived += OnLogMessageReceived;

			var header = $"{Formatter.AlignLeftBetweenBrackets("DATE", 12)} " +
							$"{Formatter.AlignLeftBetweenBrackets("TIME", 10)} " +
							$"{Formatter.AlignLeftBetweenBrackets("TYPE", 11)} " +
							$"{Formatter.AlignLeftBetweenBrackets("MESSAGE", MESSAGE_SPACING)} " +
							$"{Formatter.AlignLeftBetweenBrackets("STACK TRACE", STACK_TRACE_SPACING)}";

			File.AppendAllText(LogFile, header);
			DeleteLogsIfMaximumExceeded();
		}

		private static void OnLogMessageReceived(string log, string stackTrace, LogType type)
		{
			if (type == LogType.Error || type == LogType.Exception)
			{
				UpdateErrorCounter();
			}

			if (IsSpammingErrors)
			{
				AppendLog("Shutting down Logger due to Exception/Error spamming!", "", LogType.Error);
				Application.logMessageReceived -= OnLogMessageReceived;
			}
			else
			{
				AppendLog(log, stackTrace, type);
			}
		}

		private static void UpdateErrorCounter()
		{
			var nowSecond = DateTime.Now.Second;
			if (currentSecond == nowSecond)
			{
				errorCountInCurrentSecond++;
			}
			else
			{
				errorCountInCurrentSecond = 1;
				currentSecond = nowSecond;
			}
		}

		private static void AppendLog(string log, string stackTrace, LogType type)
		{
			var now = DateTime.Now;
			var stringBuilder = new StringBuilder();

			// Date
			stringBuilder.Append($"[ {now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture)} ]");
			// Time
			stringBuilder.Append($" [ {now.ToLongTimeString()} ]");
			// Type
			stringBuilder.Append($" {Formatter.AlignLeftBetweenBrackets(type.ToString(), 11)}");
			// Message
			stringBuilder.Append($" {Formatter.AlignLeftBetweenBrackets(log, Math.Max(log.Length + 2, MESSAGE_SPACING))}");
			// Stacktrace
			stackTrace = stackTrace.Replace("\n", " ");
			stringBuilder.Append($" {Formatter.AlignLeftBetweenBrackets(stackTrace, Math.Max(log.Length + 2, STACK_TRACE_SPACING))}");

			File.AppendAllText(LogFile, $"\n{stringBuilder}");
		}

		private static void DeleteLogsIfMaximumExceeded()
		{
			var files = Directory.GetFiles(LogDirectory).OrderBy(fileName => fileName).ToArray();
			var filesToDelete = Mathf.Clamp(files.Length - MAX_LOG_COUNT, 0, int.MaxValue);
			for (var i = 0; i < filesToDelete; i++)
			{
				File.Delete(files[i]);
			}
		}
	}
}