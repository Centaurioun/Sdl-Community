﻿using System;
using Sdl.Community.ApplyTMTemplate.ViewModels;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.ApplyTMTemplate.Models
{
	public class TranslationMemory : ModelBase
	{
		private string _sourceStatus;
		private string _targetStatus;
		private string _sourceStatusToolTip;
		private string _targetStatusToolTip;
		private bool _isSelected;

		public TranslationMemory(FileBasedTranslationMemory tm)
		{
			_sourceStatusToolTip = "Nothing processed yet";
			_targetStatusToolTip = "Nothing processed yet";
			_isSelected = false;
			_sourceStatus = "";
			_targetStatus = "";
			Tm = tm;
		}

		public string SourceStatus
		{
			get => _sourceStatus;
			set
			{
				_sourceStatus = value;
				OnPropertyChanged();
			}
		}

		public string Icon => @"../Resources/FileBasedTM.ico";

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged();
			}
		}

		public string SourceStatusToolTip
		{
			get => _sourceStatusToolTip;
			set
			{
				_sourceStatusToolTip = value;
				OnPropertyChanged();
			}
		}

		public string Name => Tm.Name;

		public FileBasedTranslationMemory Tm { get; }

		public string TargetStatus
		{
			get => _targetStatus;
			set
			{
				_targetStatus = value;
				OnPropertyChanged();
			}
		}

		public string TargetStatusToolTip
		{
			get => _targetStatusToolTip;
			set
			{
				_targetStatusToolTip = value;
				OnPropertyChanged();
			}
		}

		public void MarkSourceModified()
		{
			SourceStatus = "../Resources/Checked.ico";
			SourceStatusToolTip = "Template applied on Source language";
		}

		public void MarkSourceNotModified()
		{
			SourceStatus = "../Resources/Unchecked.ico";
			SourceStatusToolTip = "Source language doesn't correspond with any of the template's languages and was not modified";
		}

		public void MarkTargetModified()
		{
			TargetStatus = "../Resources/Checked.ico";
			TargetStatusToolTip = "Template applied on Target language";
		}

		public void MarkTargetNotModified()
		{
			TargetStatus = "../Resources/Unchecked.ico";
			TargetStatusToolTip = "Target language doesn't correspond with any of the template's languages and was not modified";
		}

		public void MarkTmCorrupted()
		{
			SourceStatus = "../Resources/Error.ico";
			TargetStatus = "../Resources/Error.ico";
			SourceStatusToolTip = "This TM is corrupted or the file is not a TM";
			TargetStatusToolTip = "This TM is corrupted or the file is not a TM";;
		}

		public void ApplyTemplate(LanguageResourceBundle languageResourceBundle)
		{
			var langDirOfTm = Tm.LanguageDirection;

			try
			{
				if (langDirOfTm.SourceLanguage.Equals(languageResourceBundle.Language))
				{
					Tm.LanguageResourceBundles.Add(languageResourceBundle);
					Tm.Save();
					MarkSourceModified();
				}
				else
				{
					MarkSourceNotModified();
				}

				if (langDirOfTm.TargetLanguage.Equals(languageResourceBundle.Language))
				{
					Tm.LanguageResourceBundles.Add(languageResourceBundle);
					Tm.Save();
					MarkTargetModified();
				}
				else
				{
					MarkTargetNotModified();
				}
			}
			catch (Exception)
			{
				MarkTmCorrupted();
			}

		}
	}
}