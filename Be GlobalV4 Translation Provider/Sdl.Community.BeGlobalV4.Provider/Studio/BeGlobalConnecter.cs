﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Newtonsoft.Json;
using RestSharp;
using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Service;
using Sdl.LanguagePlatform.Core;

namespace Sdl.Community.BeGlobalV4.Provider.Studio
{
	public class BeGlobalConnecter
	{  
		public string ClientId { get; set; }
		public string ClientSecret { get; set; }

		public BeGlobalConnecter(string clientId,string clientSecret)
		{
			ClientId = clientId;
			ClientSecret = clientSecret;
		}

		public string Translate(LanguagePair languageDirection, string sourcetext)
		{
			const string tagOption = @"xml";
			var targetLanguage = languageDirection.TargetCulture.ThreeLetterISOLanguageName;
			var sourceLanguage = languageDirection.SourceCulture.ThreeLetterISOLanguageName;
			var translatedText = string.Empty;

			var beGlobalTranslator = new BeGlobalV4Translator("https://translate-api.sdlbeglobal.com",ClientId,ClientSecret,sourceLanguage,targetLanguage, "genericnmt", false);
			translatedText = beGlobalTranslator.TranslateText(sourcetext);
			//try
			//{
			//	var client = new RestClient(@"https://api.deepl.com/v1")
			//	{
			//		UserAgent = "SDL Trados 2019 (v" + _pluginVersion + ",id" + _identifier + ")"
			//	};
			//	var request = new RestRequest("translate", Method.POST);

			//	//search for words like this <word> 
			//	var rgx = new Regex("(\\<\\w+[üäåëöøßşÿÄÅÆĞ]*[^\\d\\W\\\\/\\\\]+\\>)");
			//	var words = rgx.Matches(sourcetext);

			//	if (words.Count > 0)
			//	{
			//		var matchesIndexes = GetMatchesIndexes(sourcetext, words);
			//		sourcetext = ReplaceCharacters(matchesIndexes, sourcetext);
			//	}

			//	//search for spaces
			//	var spaceRgx = new Regex("[\\s]+");
			//	var spaces = spaceRgx.Matches(sourcetext);

			//	if (spaces.Count > 0)
			//	{
			//		var matchesIndexes = GetMatchesIndexes(sourcetext, spaces);
			//		sourcetext = EncodeSpaces(matchesIndexes, sourcetext);
			//	}
			//	//sourcetext = HttpUtility.HtmlEncode(sourcetext);
			//	//sourcetext = Uri.EscapeDataString(sourcetext);

			//	request.AddParameter("text", sourcetext);
			//	request.AddParameter("source_lang", sourceLanguage);
			//	request.AddParameter("target_lang", targetLanguage);
			//	//adding this resolve line breaks issue and missing ##login##
			//	request.AddParameter("preserve_formatting", 1);
			//	//tag handling cause issues on uppercase words
			//	request.AddParameter("tag_handling", tagOption);
			//	//if we add this the formattiong is not right
			//	//request.AddParameter("split_sentences", 0);
			//	request.AddParameter("auth_key", ApiKey);

			//	var response = client.Execute(request).Content;
			//	var translatedObject = JsonConvert.DeserializeObject<TranslationResponse>(response);
			//	if (translatedObject != null)
			//	{
			//		translatedText = translatedObject.Translations[0].Text;
			//		translatedText = HttpUtility.UrlDecode(translatedText);
			//		if (words.Count > 0)
			//		{
			//			// used to decode < > characters
			//			translatedText = HttpUtility.HtmlDecode(translatedText);
			//		}
			//	}
			//}
			//catch (WebException e)
			//{
			//	var eReason = Helpers.ProcessWebException(e);
			//	throw new Exception(eReason);
			//}

			return translatedText;
		}

		private string EncodeSpaces(int[] matchesIndexes, string sourceText)
		{
			var spaceRgx = new Regex("([\\s]+){2}");
			var finalText = new StringBuilder();
			var splitedText = sourceText.SplitAt(matchesIndexes).ToList();

			foreach (var text in splitedText)
			{
				var hasMultipleSpace = spaceRgx.IsMatch(text);
				var containsTab = text.Contains('\t');
				if (hasMultipleSpace || containsTab)
				{
					var encodedSpace = Uri.EscapeDataString(text);
					finalText.Append(encodedSpace);
				}
				else
				{
					finalText.Append(text);
				}

			}
			return finalText.ToString();
		}

		private int[] GetMatchesIndexes(string sourcetext, MatchCollection matches)
		{
			var indexes = new List<int>();
			foreach (Match match in matches)
			{
				if (match.Index.Equals(0))
				{
					indexes.Add(match.Length);
				}
				else
				{
					var remainingText = sourcetext.Substring(match.Index + match.Length);
					if (!string.IsNullOrEmpty(remainingText))
					{
						indexes.Add(match.Index);
						indexes.Add(match.Index + match.Length);
					}
					else
					{
						indexes.Add(match.Index);
					}
				}
			}
			return indexes.ToArray();
		}

		private string ReplaceCharacters(int[] indexes, string sourceText)
		{
			var splitedText = sourceText.SplitAt(indexes).ToList();
			var positions = new List<int>();
			for (var i = 0; i < splitedText.Count; i++)
			{
				if (!splitedText[i].Contains("tg"))
				{
					positions.Add(i);
				}
			}

			foreach (var position in positions)
			{
				var originalString = splitedText[position];
				var start = Regex.Replace(originalString, "<", "&lt;");
				var finalString = Regex.Replace(start, ">", "&gt;");
				splitedText[position] = finalString;
			}
			var finalText = string.Empty;
			foreach (var text in splitedText)
			{
				finalText += text;
			}

			return finalText;
		}
	}
}
