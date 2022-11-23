﻿using System;
using GoogleTranslatorProvider.GoogleAPI;
using GoogleTranslatorProvider.Interfaces;
using GoogleTranslatorProvider.Models;
using GoogleTranslatorProvider.Service;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.Core;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace GoogleTranslatorProvider.Studio
{
	public class Provider : ITranslationProvider
	{
		private readonly HtmlUtil _htmlUtil;

		private V2Connector _googleV2Api;
		private V3Connector _googleV3Api;

		public Provider(ITranslationOptions options, HtmlUtil htmlUtil)
		{
			Options = options;
			_htmlUtil = htmlUtil;
		}

		public ITranslationOptions Options { get; set; }


		public string Name => Options.SelectedProvider == ProviderType.GoogleTranslate
							? Options.SelectedGoogleVersion == ApiVersion.V2 ? PluginResources.GoogleBasic
																			 : PluginResources.GoogleAdvanced
							: PluginResources.Plugin_Name;

		public ProviderStatusInfo StatusInfo => new(true, PluginResources.Plugin_NiceName);
		
		public bool SupportsSearchForTranslationUnits => true;
		
		public bool SupportsTaggedInput => true;
		
		public bool SupportsTranslation => true;
		
		public bool SupportsPenalties => true;
		
		public bool IsReadOnly => true;
		
		public bool SupportsSourceConcordanceSearch => false;
		
		public bool SupportsTargetConcordanceSearch => false;
		
		public bool SupportsConcordanceSearch => false;
		
		public bool SupportsDocumentSearches => false;
		
		public bool SupportsStructureContext => false;
		
		public bool SupportsMultipleResults => false;
		
		public bool SupportsFuzzySearch => false;
		
		public bool SupportsPlaceables => false;
		
		public bool SupportsWordCounts => false;
		
		public bool SupportsFilters => false;
		
		public bool SupportsScoring => false;
		
		public bool SupportsUpdate => false;

		public TranslationMethod TranslationMethod => GTPTranslationOptions.ProviderTranslationMethod;

		public Uri Uri => Options.Uri;

		public bool SupportsLanguageDirection(LanguagePair languageDirection)
		{
			if (Options.SelectedGoogleVersion is not ApiVersion.V2)
			{
				_googleV3Api = new V3Connector(Options);
				return _googleV3Api.IsSupportedLanguage(languageDirection.SourceCulture, languageDirection.TargetCulture);
			}

			if (_googleV2Api is null)
			{
				_googleV2Api = new V2Connector(Options.ApiKey, _htmlUtil);
			}
			else
			{
				_googleV2Api.ApiKey = Options.ApiKey;
			}

			return _googleV2Api.IsSupportedLanguagePair(languageDirection.SourceCulture, languageDirection.TargetCulture);
		}

		public void LoadState(string translationProviderState)
		{
			Options = JsonConvert.DeserializeObject<GTPTranslationOptions>(translationProviderState);
		}

		public ITranslationProviderLanguageDirection GetLanguageDirection(LanguagePair languageDirection)
		{
			return new ProviderLanguageDirection(this, languageDirection, _htmlUtil);
		}

		public string SerializeState()
		{
			return JsonConvert.SerializeObject(Options);
		}

		public void RefreshStatusInfo() { }
	}
}