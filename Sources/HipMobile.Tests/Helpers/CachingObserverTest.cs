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

using NUnit.Framework;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileTests.Helpers
{
    [TestFixture]
    public class CachingObserverTest
    {
        [Test, Category("UnitTest")]
        public void Test()
        {
            var observable = new Observable<bool>(false);
            var caching = new CachingObserver<bool>();
            observable.Subscribe(caching);
            Assert.IsFalse(caching.Last);
            observable.Current = true;
            Assert.IsTrue(caching.Last);
        }
    }
}