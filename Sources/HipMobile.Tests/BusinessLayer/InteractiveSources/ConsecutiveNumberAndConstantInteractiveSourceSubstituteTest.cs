﻿// Copyright (C) 2016 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//       http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.ComponentModel;
using de.upb.hip.mobile.pcl.BusinessLayer.InteractiveSources;
using NUnit.Framework;

namespace HipMobile.Tests.BusinessLayer.InteractiveSources {

    [TestFixture]
    public class ConsecutiveNumberAndConstantInteractiveSourceSubstituteTest
    {
        [Test, NUnit.Framework.Category("UnitTest")]
        public void NextSubstitute_CalledOnce()
        {
            var sut = CreateSystemUnderTest();

            var substitute = sut.NextSubstitute ();

            Assert.AreEqual ($"[{Substitute} {Start}]", substitute);
        }

        [Test, NUnit.Framework.Category("UnitTest")]
        public void NextSubstitute_CalledMultipleTimes()
        {
            var sut = CreateSystemUnderTest();

            var firstSubstitute = sut.NextSubstitute();
            var secondSubstitute = sut.NextSubstitute();

            Assert.AreEqual($"[{Substitute} {Start}]", firstSubstitute);
            Assert.AreEqual($"[{Substitute} {Start+1}]", secondSubstitute);
        }

        #region HelperMethods

        private const string Substitute = "Test";
        private const int Start = 0;

        private ConsecutiveNumberAndConstantInteractiveSourceSubstitute CreateSystemUnderTest ()
        {
            return new ConsecutiveNumberAndConstantInteractiveSourceSubstitute (Start, Substitute);
        }

        #endregion

    }
}