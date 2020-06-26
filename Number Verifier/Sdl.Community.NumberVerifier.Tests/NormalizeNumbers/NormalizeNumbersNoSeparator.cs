﻿using Moq;
using Sdl.Community.NumberVerifier.Interfaces;
using Sdl.Community.NumberVerifier.Model;
using Sdl.Community.NumberVerifier.Tests.Utilities;
using Sdl.FileTypeSupport.Framework.BilingualApi;
using Xunit;

namespace Sdl.Community.NumberVerifier.Tests.NormalizeNumbers
{
	public class NormalizeNumbersNoSeparator
    {
        
        [Theory]
        [InlineData("1,55", " ", ",", false)]
        public void CheckIfNoSeparatorMethodIsCalled(string text, string thousandSep, string decimalSep, bool noSeparator)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.SpaceCommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);

            var normalizedNumber = numberVerifierMain.NormalizeNumber(new SeparatorModel
				{
					MatchValue = text,
					ThousandSeparators = thousandSep,
					DecimalSeparators = decimalSep,
					NoSeparator = noSeparator,
					CustomSeparators = string.Empty
				});

            var methodsMock = new Mock<INumberVerifierMethods>(MockBehavior.Strict);
            methodsMock.Verify(s => s.NormalizeNumberNoSeparator(thousandSep, decimalSep, normalizedNumber), Times.Never);

        }

        [Theory]
        [InlineData("1,55", " ", ",", true)]
        public string NormalizeNoSeparatorNumbers(string text, string thousandSep, string decimalSep, bool noSeparator)
        {
            var numberVerifierSettings = SourceSettings.SourceSettingsAndAllowLocalization.SpaceCommaPeriod();
            numberVerifierSettings.Setup(d => d.SourceDecimalComma).Returns(true);

            NumberVerifierLocalizationsSettings.InitSeparators(numberVerifierSettings);
            var numberVerifierMain = new NumberVerifierMain(numberVerifierSettings.Object);

            //run initialize method in order to set chosen separators
            var docPropMock = new Mock<IDocumentProperties>();
            numberVerifierMain.Initialize(docPropMock.Object);
			
            var normalizedNumber = numberVerifierMain.NormalizeNumber(new SeparatorModel
			{
				MatchValue = text,
				ThousandSeparators = thousandSep,
				DecimalSeparators = decimalSep,
				NoSeparator = noSeparator,
				CustomSeparators = string.Empty
			});

            return normalizedNumber;
        }

        [Theory]
        [InlineData("1,55", " ", ",", true)]
        public void NotNormalizeDecimalNumbers(string text, string thousandSep, string decimalSep, bool noSeparator)
        {
            var normalizedNumber = NormalizeNoSeparatorNumbers(text, thousandSep, decimalSep, noSeparator);

            Assert.Equal("1d55", normalizedNumber);
        }

        [Theory]
        [InlineData("1,234.56", ",", ".", true)]
        public void NormalizeThousandsNumberNoSeparatorSelected(string text, string thousandSep, string decimalSep, bool noSeparator)
        {
            var normalizedNumber = NormalizeNoSeparatorNumbers(text, thousandSep, decimalSep, noSeparator);

            Assert.Equal("1t234d56", normalizedNumber);
        }
    }
}