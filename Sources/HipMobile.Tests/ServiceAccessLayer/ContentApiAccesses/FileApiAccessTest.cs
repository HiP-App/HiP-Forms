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

using System.Text;
using NSubstitute;
using NUnit.Framework;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileTests.ServiceAccessLayer.ContentApiAccesses
{
    [TestFixture]
    public class FileApiAccessTest
    {
        private IContentApiClient contentApiSubstitute;

        [Test, Category("UnitTest")]
        public async void GetFile_Test()
        {
            var sut = CreateSystemUnderTest();
            string jsonReturn = "ThisIsATestByteArray";
            var byteData = Encoding.UTF8.GetBytes(jsonReturn);
            contentApiSubstitute.GetResponseFromUrlAsBytes(null).ReturnsForAnyArgs(byteData);

            var file = await sut.GetFile(42);
            Assert.AreEqual(42, file.MediaId);

            var data = file.Data;
            Assert.AreEqual(84, data[0]);
            Assert.AreEqual(104, data[1]);
            Assert.AreEqual(105, data[2]);
            Assert.AreEqual(115, data[3]);
            Assert.AreEqual(73, data[4]);
            Assert.AreEqual(115, data[5]);
            Assert.AreEqual(65, data[6]);
            Assert.AreEqual(84, data[7]);
            Assert.AreEqual(101, data[8]);
            Assert.AreEqual(115, data[9]);
            Assert.AreEqual(116, data[10]);
            Assert.AreEqual(66, data[11]);
            Assert.AreEqual(121, data[12]);
            Assert.AreEqual(116, data[13]);
            Assert.AreEqual(101, data[14]);
            Assert.AreEqual(65, data[15]);
            Assert.AreEqual(114, data[16]);
            Assert.AreEqual(114, data[17]);
            Assert.AreEqual(97, data[18]);
            Assert.AreEqual(121, data[19]);
        }

        #region HelperMethods

        private FileApiAccess CreateSystemUnderTest()
        {
            contentApiSubstitute = Substitute.For<IContentApiClient>();

            return new FileApiAccess(contentApiSubstitute);
        }

        #endregion
    }
}