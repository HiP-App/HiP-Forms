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
using System.Linq;
using NUnit.Framework;
using PaderbornUniversity.SILab.Hip.Mobile.HipMobileTests.TestHelpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileTests.Helpers
{
    [TestFixture]
    public class ObservableTest
    {
        [Test, Category("UnitTest")]
        public void TestMultipleObservers()
        {
            var obs = Enumerable.Range(0, 10).Select(ignored => new CachingObserver<bool>()).ToList();
            IObservable<bool> observable = new Observable<bool>(true);
            obs.ForEach(observer => observable.Subscribe(observer));
            obs.ForEach(observer => Assert.IsTrue(observer.Last));
        }
        
        [Test, Category("UnitTest")]
        public void TestUpdate()
        {
            var obs = Enumerable.Range(0, 10).Select(ignored => new CachingObserver<bool>()).ToList();
            var observable = new Observable<bool>(false);
            obs.ForEach(observer => observable.Subscribe(observer));

            observable.Current = true;
            obs.ForEach(observer => Assert.IsTrue(observer.Last));
        }
        
        [Test, Category("UnitTest")]
        public void TestDispose()
        {
            var obs = Enumerable.Range(0, 10).Select(ignored => new CachingObserver<bool>()).ToList();
            var observable = new Observable<bool>(true);
            var disposers = obs.Select(observer => observable.Subscribe(observer)).ToList();

            disposers.ForEach(disposer => disposer.Dispose());
            observable.Current = false;
            
            // No observer should have gotten the update
            obs.ForEach(observer => Assert.IsTrue(observer.Last));
        }
    }
}