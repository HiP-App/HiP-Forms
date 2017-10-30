// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using NSubstitute;
using NUnit.Framework;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.InteractiveSources;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileTests.BusinessLayer.InteractiveSources
{
    [TestFixture]
    public class InteractiveSourcesParserTest
    {
        [Test, Category("UnitTest")]
        public void Parse_NoSources()
        {
            var sut = CreateSystemUnderTest();
            var textToParse = "This is a test";

            var parsingResult = sut.Parse(textToParse);

            Assert.AreEqual(textToParse, parsingResult.TextWithSubstitutes);
            Assert.AreEqual(0, parsingResult.Sources.Count);
        }

        [Test, Category("UnitTest")]
        public void Parse_OneSource()
        {
            var sut = CreateSystemUnderTest();
            var textToParse = "This<fn>Source1</fn> is a test";

            var parsingResult = sut.Parse(textToParse);

            Assert.AreEqual(textToParse.Replace("<fn>Source1</fn>", SubstituteText), parsingResult.TextWithSubstitutes);
            Assert.AreEqual(1, parsingResult.Sources.Count);

            AssertSource(parsingResult.Sources[0], 0, "Source1", 4);
        }

        [Test, Category("UnitTest")]
        public void Parse_TwoSources()
        {
            var sut = CreateSystemUnderTest();
            var textToParse = "This<fn>Source1</fn> is a test and another test <fn>Source2</fn>";

            var parsingResult = sut.Parse(textToParse);

            Assert.AreEqual(textToParse.Replace("<fn>Source1</fn>", SubstituteText)
                                       .Replace("<fn>Source2</fn>", SubstituteText), parsingResult.TextWithSubstitutes);
            Assert.AreEqual(2, parsingResult.Sources.Count);

            AssertSource(parsingResult.Sources[0], 0, "Source1", 4);
            AssertSource(parsingResult.Sources[1], 1, "Source2", 38);
        }

        [Test, Category("UnitTest")]
        public void Parse_Null()
        {
            var sut = CreateSystemUnderTest();

            Assert.Catch<ArgumentNullException>(() => sut.Parse(null));
        }

        #region HelperMethods

        private void AssertSource(Source source, int expectedNumberInSubtitles, string expectedText, int expectedStartIndex)
        {
            Assert.AreEqual(SubstituteText, source.SubstituteText);
            Assert.AreEqual(expectedNumberInSubtitles, source.NumberInSubtitles);
            Assert.AreEqual(expectedText, source.Text);
            Assert.AreEqual(expectedStartIndex, source.StartIndex);
        }

        private const string SubstituteText = "[Test]";

        private InteractiveSourcesParser CreateSystemUnderTest()
        {
            var substitute = Substitute.For<IInteractiveSourceSubstitute>();
            substitute.NextSubstitute().ReturnsForAnyArgs(SubstituteText);

            return new InteractiveSourcesParser(substitute);
        }

        #endregion
    }
}