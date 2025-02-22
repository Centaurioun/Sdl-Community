﻿using System;
using GoogleCloudTranslationProvider.Models;
using Newtonsoft.Json;
using Sdl.LanguagePlatform.TranslationMemoryApi;

namespace GoogleCloudTranslationProvider.Studio
{
	[TranslationProviderFactory(Id = Constants.Provider_TranslationProviderFactory,
								Name = Constants.Provider_TranslationProviderFactory,
								Description = Constants.Provider_TranslationProviderFactory)]
	public class TranslationProviderFactory : ITranslationProviderFactory
	{
		public ITranslationProvider CreateTranslationProvider(Uri translationProviderUri, string translationProviderState, ITranslationProviderCredentialStore credentialStore)
		{
			if (!SupportsTranslationProviderUri(translationProviderUri))
			{
				throw new Exception(PluginResources.UriNotSupportedMessage);
			}

 			var translationOptions = JsonConvert.DeserializeObject<TranslationOptions>(translationProviderState);

			if (translationOptions.SelectedGoogleVersion is not ApiVersion.V2)
			{
                AppInitializer.TranslationOptions[translationOptions.Id] = translationOptions;
                return new TranslationProvider(translationOptions);
			}

			if ((credentialStore.GetCredential(translationProviderUri) ??
				 credentialStore.GetCredential(new Uri(Constants.GoogleTranslationFullScheme)))
				is not TranslationProviderCredential credentials)
			{
				throw new TranslationProviderAuthenticationException();
			}

			credentials = new TranslationProviderCredential(credentials.Credential, credentials.Persist);
			translationOptions.ApiKey = credentials.Credential;
			translationOptions.PersistGoogleKey = credentials.Persist;

			AppInitializer.TranslationOptions[translationOptions.Id] = translationOptions;
			return new TranslationProvider(translationOptions);
		}

		public bool SupportsTranslationProviderUri(Uri translationProviderUri)
			=> translationProviderUri switch
			{
				null => throw new ArgumentNullException(PluginResources.UriNotSupportedMessage),
				_ => translationProviderUri.Scheme.Contains(Constants.GoogleTranslationScheme)
			};

		public TranslationProviderInfo GetTranslationProviderInfo(Uri translationProviderUri, string translationProviderState)
			=> new()
			{
				TranslationMethod = TranslationMethod.MachineTranslation,
				Name = Constants.GoogleNaming_FullName
			};
	}
}