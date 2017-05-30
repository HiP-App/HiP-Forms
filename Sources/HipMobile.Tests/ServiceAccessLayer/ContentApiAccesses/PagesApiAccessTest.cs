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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileTests.ServiceAccessLayer.ContentApiAccesses
{
    [TestFixture]
    public class PagesApiAccessTest
    {

        private IContentApiClient contentApiSubstitute;

        [Test, Category("UnitTest")]
        public async void GetPages_AppetizerPage()
        {
            var sut = CreateSystemUnderTest();
            string jsonReturn = "{" +
                                "total: 1," +
                                "items: [" +
                                "{" +
                                "\"exhibitId\": 17," +
                                "\"id\": 1," +
                                "\"type\": \"appetizerpage\"," +
                                "\"text\": \"Test Text\"," +
                                "\"image\": 42," +
                                "\"status\": \"Test Status\"," +
                                "\"timestamp\": \"2017-05-29T10:10:10.10+00:00\"," +
                                "\"additionalInformationPages\": [4, 5]," +
                                "}" +
                                "]" +
                                "}";
            contentApiSubstitute.GetResponseFromUrl(null).ReturnsForAnyArgs(jsonReturn);

            var pages = await sut.GetPages();
            Assert.AreEqual(1, pages.Total);
            Assert.AreEqual(1, pages.Items.Count);

            var firstPage = pages.Items[0];
            Assert.AreEqual(1, firstPage.Id);
            Assert.AreEqual(17, firstPage.ExhibitId);
            Assert.AreEqual("Test Text", firstPage.Text);
            Assert.AreEqual(42, firstPage.Image);
            Assert.AreEqual(PageTypeDto.AppetizerPage, firstPage.Type);

            var additionalInformationPages = firstPage.AdditionalInformationPages;
            Assert.AreEqual(2, additionalInformationPages.Count);
            Assert.AreEqual(4, additionalInformationPages[0]);
            Assert.AreEqual(5, additionalInformationPages[1]);

            Assert.AreEqual("Test Status", firstPage.Status);
            Shared.Helpers.AssertionHelper.AreEqual(new DateTimeOffset(2017, 5, 29, 10, 10, 10, 10, TimeSpan.Zero), firstPage.Timestamp);
        }

        [Test, Category("UnitTest")]
        public async void GetPages_ImagePage()
        {
            var sut = CreateSystemUnderTest();
            string jsonReturn = "{" +
                                "total: 1," +
                                "items: [" +
                                "{" +
                                "\"exhibitId\": 17," +
                                "\"id\": 1," +
                                "\"type\": \"imagepage\"," +
                                "\"image\": 42," +
                                "\"audio\": 17," +
                                "\"status\": \"Test Status\"," +
                                "\"timestamp\": \"2017-05-29T10:10:10.10+00:00\"," +
                                "\"additionalInformationPages\": [4, 5]" +
                                "}" +
                                "]" +
                                "}";
            contentApiSubstitute.GetResponseFromUrl(null).ReturnsForAnyArgs(jsonReturn);

            var pages = await sut.GetPages();
            Assert.AreEqual(1, pages.Total);
            Assert.AreEqual(1, pages.Items.Count);

            var firstPage = pages.Items[0];
            Assert.AreEqual(1, firstPage.Id);
            Assert.AreEqual(17, firstPage.ExhibitId);
            Assert.AreEqual(42, firstPage.Image);
            Assert.AreEqual(17, firstPage.Audio);
            Assert.AreEqual(PageTypeDto.ImagePage, firstPage.Type);

            var additionalInformationPages = firstPage.AdditionalInformationPages;
            Assert.AreEqual(2, additionalInformationPages.Count);
            Assert.AreEqual(4, additionalInformationPages[0]);
            Assert.AreEqual(5, additionalInformationPages[1]);

            Assert.AreEqual("Test Status", firstPage.Status);
            Shared.Helpers.AssertionHelper.AreEqual(new DateTimeOffset(2017, 5, 29, 10, 10, 10, 20, TimeSpan.Zero), firstPage.Timestamp);
        }

        [Test, Category("UnitTest")]
        public async void GetPages_TextPage()
        {
            var sut = CreateSystemUnderTest();
            string jsonReturn = "{" +
                                "total: 1," +
                                "items: [" +
                                "{" +
                                "\"exhibitId\": 17," +
                                "\"id\": 1," +
                                "\"type\": \"textpage\"," +
                                "\"title\": \"Test Title\"," +
                                "\"text\": \"Test Text\"," +
                                "\"audio\": 17," +
                                "\"status\": \"Test Status\"," +
                                "\"timestamp\": \"2017-05-29T10:10:10.10+00:00\"," +
                                "\"additionalInformationPages\": [4, 5]" +
                                "}" +
                                "]" +
                                "}";
            contentApiSubstitute.GetResponseFromUrl(null).ReturnsForAnyArgs(jsonReturn);

            var pages = await sut.GetPages();
            Assert.AreEqual(1, pages.Total);
            Assert.AreEqual(1, pages.Items.Count);

            var firstPage = pages.Items[0];
            Assert.AreEqual(1, firstPage.Id);
            Assert.AreEqual(17, firstPage.ExhibitId);

            Assert.AreEqual("Test Text", firstPage.Text);
            Assert.AreEqual("Test Title", firstPage.Title);
            Assert.AreEqual(17, firstPage.Audio);
            Assert.AreEqual(PageTypeDto.TextPage, firstPage.Type);

            var additionalInformationPages = firstPage.AdditionalInformationPages;
            Assert.AreEqual(2, additionalInformationPages.Count);
            Assert.AreEqual(4, additionalInformationPages[0]);
            Assert.AreEqual(5, additionalInformationPages[1]);

            Assert.AreEqual("Test Status", firstPage.Status);
            Shared.Helpers.AssertionHelper.AreEqual(new DateTimeOffset(2017, 5, 29, 10, 10, 10, 10, TimeSpan.Zero), firstPage.Timestamp);
        }

        [Test, Category("UnitTest")]
        public async void GetPages_SliderPage()
        {
            var sut = CreateSystemUnderTest();
            string jsonReturn = "{" +
                                "total: 1," +
                                "items: [" +
                                "{" +
                                "\"exhibitId\": 17," +
                                "\"id\": 1," +
                                "\"type\": \"sliderpage\"," +
                                "\"title\": \"Test Title\"," +
                                "\"text\": \"Test Text\"," +
                                "\"audio\": 17," +
                                "\"images\": [" +
                                "  {" +
                                "    \"date\": 2492029999," +
                                "    \"image\": 42" +
                                "  }," +
                                "  {" +
                                "    \"date\": 3492029999," +
                                "    \"image\": 43" +
                                "  }" +
                                "]," +
                                "\"hideYearNumbers\": true," +
                                "\"status\": \"Test Status\"," +
                                "\"timestamp\": \"2017-05-29T10:10:10.10+00:00\"," +
                                "\"additionalInformationPages\": [4, 5]" +
                                "}" +
                                "]" +
                                "}";
            contentApiSubstitute.GetResponseFromUrl(null).ReturnsForAnyArgs(jsonReturn);

            var pages = await sut.GetPages();
            Assert.AreEqual(1, pages.Total);
            Assert.AreEqual(1, pages.Items.Count);

            var firstPage = pages.Items[0];
            Assert.AreEqual(1, firstPage.Id);
            Assert.AreEqual(17, firstPage.ExhibitId);

            Assert.AreEqual("Test Text", firstPage.Text);
            Assert.AreEqual("Test Title", firstPage.Title);
            Assert.AreEqual(17, firstPage.Audio);

            var images = firstPage.Images;
            Assert.AreEqual(2, images.Count);
            Assert.AreEqual(2492029999, images[0].Date);
            Assert.AreEqual(42, images[0].Image);
            Assert.AreEqual(3492029999, images[1].Date);
            Assert.AreEqual(43, images[1].Image);

            var additionalInformationPages = firstPage.AdditionalInformationPages;
            Assert.AreEqual(2, additionalInformationPages.Count);
            Assert.AreEqual(4, additionalInformationPages[0]);
            Assert.AreEqual(5, additionalInformationPages[1]);

            Assert.AreEqual(true, firstPage.HideYearNumbers);
            Assert.AreEqual(PageTypeDto.SliderPage, firstPage.Type);
            Assert.AreEqual("Test Status", firstPage.Status);
            Shared.Helpers.AssertionHelper.AreEqual(new DateTimeOffset(2017, 5, 29, 10, 10, 10, 10, TimeSpan.Zero), firstPage.Timestamp);
        }

        #region HelperMethods

        private PagesApiAccess CreateSystemUnderTest()
        {
            contentApiSubstitute = Substitute.For<IContentApiClient>();

            return new PagesApiAccess(contentApiSubstitute);
        }

        #endregion

    }
}