﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Sdl.Community.SdlTmAnonymizer.Controls.ProgressDialog;
using Sdl.Community.SdlTmAnonymizer.Model;
using Sdl.Community.SdlTmAnonymizer.Services;
using Sdl.Community.SdlTmAnonymizer.View;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.SdlTmAnonymizer.ViewModel
{
	public class SelectServersWindowViewModel : ViewModelBase, IDisposable
	{
		private List<TmFile> _translationMemories;
		private readonly SettingsService _settingsService;
		private readonly TranslationProviderServer _translationProviderServer;
		private readonly BackgroundWorker _backgroundWorker;
		private readonly Window _controlWindow;

		public SelectServersWindowViewModel(Window controlWindow, SettingsService settingsService, TranslationProviderServer translationProviderServer)
		{
			_controlWindow = controlWindow;

			_settingsService = settingsService;
			_translationProviderServer = translationProviderServer;

			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.DoWork += BackgroundWorker_DoWork;
			_backgroundWorker.RunWorkerAsync();
		}

		public List<TmFile> TranslationMemories
		{
			get => _translationMemories ?? (_translationMemories = new List<TmFile>());
			set
			{
				if (_translationMemories == value)
				{
					return;
				}

				_translationMemories = value;

				OnPropertyChanged(nameof(TranslationMemories));
			}
		}

		private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				var settings = new ProgressDialogSettings(_controlWindow, true, true, true);
				var result = ProgressDialog.Execute("Loading data...", () =>
				{
					GetServerTms(ProgressDialog.Current);

				}, settings);

				RefreshView();

				if (result.Cancelled)
				{
					throw new Exception("Process cancelled." + "\r\n\r\n" + result.Error);
				}

				if (result.OperationFailed)
				{
					throw new Exception("Process failed." + "\r\n\r\n" + result.Error);
				}
			});
		}

		private void GetServerTms(ProgressDialogContext context)
		{
			var translationMemories = _translationProviderServer.GetTranslationMemories(TranslationMemoryProperties.None);

			foreach (var tm in translationMemories)
			{
				if (context.CheckCancellationPending())
				{
					break;
				}

				var tmPath = tm.ParentResourceGroupPath == "/" ? "" : tm.ParentResourceGroupPath;
				var path = tmPath + "/" + tm.Name;
				var tmAlreadyExist = TranslationMemories.Any(t => t.Path.Equals(path));

				context.Report(path);

				if (!tmAlreadyExist)
				{
					var serverTm = new TmFile
					{
						Path = path,
						Name = tm.Name,
						Description = tm.Description,
						TranslationUnits = tm.GetTranslationUnitCount(),
						IsServerTm = true,
						TmLanguageDirections = new List<TmLanguageDirection>(),
					};

					foreach (var languageDirection in tm.LanguageDirections)
					{
						serverTm.TmLanguageDirections.Add(
							new TmLanguageDirection
							{
								Source = languageDirection.SourceLanguage,
								Target = languageDirection.TargetLanguage,
								TranslationUnitsCount = languageDirection.GetTranslationUnitCount()
							});
					}

					Application.Current.Dispatcher.Invoke(() =>
					{
						TranslationMemories.Add(serverTm);
						RefreshView();
					});
				}
			}
		}

		private void RefreshView()
		{
			OnPropertyChanged(nameof(TranslationMemories));
			((SelectServersWindow)_controlWindow).Refresh();
		}

		public void Dispose()
		{
			if (_backgroundWorker != null)
			{
				_backgroundWorker.DoWork -= BackgroundWorker_DoWork;
				_backgroundWorker.Dispose();
			}
		}
	}
}
