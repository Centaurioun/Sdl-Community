﻿using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace Sdl.Community.DeepLMTProvider
{
	public static class Log
	{
		public static void Setup()
		{
			var config = LogManager.Configuration ?? new LoggingConfiguration();

			var logDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SDL Community",
				"DeepLLogs");
			Directory.CreateDirectory(logDirectoryPath);

			var target = new FileTarget
			{
				Name = "DeepL",
				FileName = Path.Combine(logDirectoryPath, "DeeplLogs.txt"),
				Layout = "${logger}: ${longdate} ${level} ${message}  ${exception}"
			};

			config.AddTarget(target);
			config.AddRuleForAllLevels(target, "*DeepL*");

			LogManager.ReconfigExistingLoggers();
		}
	}
}