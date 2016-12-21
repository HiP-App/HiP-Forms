using System;
using de.upb.hip.mobile.pcl.BusinessLayer.InteractiveSources;
using NSubstitute;
using NUnit.Framework;

namespace HipMobile.Tests.BusinessLayer.InteractiveSources
{
    [TestFixture]
    public class InteractiveSourcesParserTest
    {
        [Test, Category("UnitTest")]
        public void Parse_NoSources()
        {
            var sut = CreateSystemUnderTest();
            var textToParse = "This is a test";

            var parsingResult = sut.Parse (textToParse);

            Assert.AreEqual (textToParse, parsingResult.TextWithSubstitutes);
            Assert.AreEqual (0, parsingResult.Sources.Count);
        }

        [Test, Category("UnitTest")]
        public void Parse_OneSource()
        {
            var sut = CreateSystemUnderTest();
            var textToParse = "This<fn>Source1</fn> is a test";

            var parsingResult = sut.Parse(textToParse);

            Assert.AreEqual(textToParse.Replace ("<fn>Source1</fn>", SubstituteText), parsingResult.TextWithSubstitutes);
            Assert.AreEqual(1, parsingResult.Sources.Count);

            AssertSource (parsingResult.Sources [0], 0, "Source1", 4);
        }

        [Test, Category("UnitTest")]
        public void Parse_TwoSources()
        {
            var sut = CreateSystemUnderTest();
            var textToParse = "This<fn>Source1</fn> is a test and another test <fn>Source2</fn>";

            var parsingResult = sut.Parse(textToParse);

            Assert.AreEqual (textToParse.Replace ("<fn>Source1</fn>", SubstituteText)
                                        .Replace ("<fn>Source2</fn>", SubstituteText), parsingResult.TextWithSubstitutes);
            Assert.AreEqual(2, parsingResult.Sources.Count);

            AssertSource(parsingResult.Sources[0], 0, "Source1", 4);
            AssertSource(parsingResult.Sources[1], 1, "Source2", 38);
        }

        [Test, Category("UnitTest")]
        public void Parse_Null ()
        {
            var sut = CreateSystemUnderTest ();

            Assert.Catch<ArgumentNullException> (() => sut.Parse (null));
        }

        #region HelperMethods

        private void AssertSource (Source source, int expectedNumberInSubtitles, string expectedText, int expectedStartIndex)
        {
            Assert.AreEqual (SubstituteText, source.SubstituteText);
            Assert.AreEqual (expectedNumberInSubtitles, source.NumberInSubtitles);
            Assert.AreEqual (expectedText, source.Text);
            Assert.AreEqual (expectedStartIndex, source.StartIndex);
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
