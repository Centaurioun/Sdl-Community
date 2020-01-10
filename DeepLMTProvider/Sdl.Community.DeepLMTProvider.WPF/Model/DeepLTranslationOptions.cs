﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace Sdl.Community.DeepLMTProvider.WPF.Model
{
	public class DeepLTranslationOptions
	{
		public string ApiKey { get; set; }
		public string ResendDraftsParameter
		{
			get => GetStringParameter("resenddrafts");
			set => SetStringParameter("resenddrafts", value);
		}
		public bool ResendDrafts
		{
			get => ResendDraftsParameter != null && Convert.ToBoolean(ResendDraftsParameter);
			set => ResendDraftsParameter = value.ToString();
		}
		public bool SendPlainText
		{
			get => SendPlainTextParameter != null && Convert.ToBoolean(SendPlainTextParameter);
			set => SendPlainTextParameter = value.ToString();
		}
		public string SendPlainTextParameter
		{
			get => GetStringParameter("sendPlain");
			set => SetStringParameter("sendPlain", value);
		}

		public string Identifier { get; set; }

		private string GetStringParameter(string p)
		{
			var paramString = _uriBuilder[p];
			return paramString;
		}
		public Dictionary<string, string> LanguagesSupported { get; set; } = new Dictionary<string, string>();


		private void SetStringParameter(string p, string value)
		{
			_uriBuilder[p] = value;
		}
		public Uri Uri => _uriBuilder.Uri;

		readonly TranslationProviderUriBuilder _uriBuilder;

		public DeepLTranslationOptions()
		{
			_uriBuilder = new TranslationProviderUriBuilder("deepltranslationprovider");
		}
		public DeepLTranslationOptions(Uri uri)
		{
			_uriBuilder = new TranslationProviderUriBuilder(uri);
		}
	}
}
