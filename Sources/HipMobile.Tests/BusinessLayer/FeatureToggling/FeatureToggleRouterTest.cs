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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.FeatureToggling;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.FeatureToggleApiAccess;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.FeatureToggleApiDto;

namespace PaderbornUniversity.SILab.Hip.Mobile.HipMobileTests.BusinessLayer.FeatureToggling
{
    [TestFixture]
    public class FeatureToggleRouterTest
    {
        private class DummyFeatureToggleApiAccess : IFeatureToggleApiAccess
        {
            public IList<FeatureDto> ReturnValue;

            public Task<IList<FeatureDto>> GetEnabledFeaturesAsync() => Task.FromResult(ReturnValue);
        }

        private class ThrowingFeatureToggleApiAccess : IFeatureToggleApiAccess
        {
            public Task<IList<FeatureDto>> GetEnabledFeaturesAsync()
            {
                throw new IOException();
            }
        }

        private class CachingObserver<T> : IObserver<T>
        {
            public T Last { get; private set; }
            
            public void OnCompleted()
            {
            }

            public void OnError(Exception error)
            {
            }

            public void OnNext(T value)
            {
                Last = value;
            }
        }

        [Test, Category("UnitTest")]
        public async Task TestNetworkUp()
        {
            var dummyApiAccess = new DummyFeatureToggleApiAccess
            {
                ReturnValue = new List<FeatureDto>
                {
                    new FeatureDto(0, "Test", null, new List<int>(), new List<int>())
                }
            };
            IoCManager.RegisterInstance(typeof(IFeatureToggleApiAccess), dummyApiAccess);

            IFeatureToggleRouter router = await FeatureToggleRouter.Create();

            var feature0Observer = new CachingObserver<bool>();
            var feature1Observer = new CachingObserver<bool>();
            router.IsFeatureEnabled(0).Subscribe(feature0Observer);
            router.IsFeatureEnabled((FeatureId) 1).Subscribe(feature1Observer);
            
            Assert.IsTrue(feature0Observer.Last);
            Assert.IsFalse(feature1Observer.Last);
        }

        [Test, Category("UnitTest")]
        public async Task TestNetworkDown()
        {
            var throwingApiAccess = new ThrowingFeatureToggleApiAccess();
            IoCManager.RegisterInstance(typeof(IFeatureToggleApiAccess), throwingApiAccess);

            IFeatureToggleRouter router = await FeatureToggleRouter.Create();
            
            foreach (var defaultEnabledFeatureId in FeatureConfiguration.DefaultEnabledFeatureIds)
            {
                var observer = new CachingObserver<bool>();
                router.IsFeatureEnabled(defaultEnabledFeatureId).Subscribe(observer);
                Assert.IsTrue(observer.Last);
            }

            // Now generate some random disabled features and check whether
            // the router tells us that they are disabled
            var random = new Random();
            for (var i = 0; i < 10;)
            {
                var featureId = random.Next();
                if (FeatureConfiguration.DefaultEnabledFeatureIds.Contains((FeatureId) featureId))
                {
                    continue;
                }
                
                var observer = new CachingObserver<bool>();
                router.IsFeatureEnabled((FeatureId) featureId).Subscribe(observer);
                Assert.IsFalse(observer.Last);
                i++;
            }
        }
        
        [Test, Category("UnitTest")]
        public async Task TestValueChanging()
        {
            var dummyApiAccess = new DummyFeatureToggleApiAccess
            {
                ReturnValue = new List<FeatureDto>
                {
                    new FeatureDto(0, "Test", null, new List<int>(), new List<int>())
                }
            };
            IoCManager.RegisterInstance(typeof(IFeatureToggleApiAccess), dummyApiAccess);

            IFeatureToggleRouter router = await FeatureToggleRouter.Create();

            var observer = new CachingObserver<bool>();
            router.IsFeatureEnabled(0).Subscribe(observer);
            Assert.IsTrue(observer.Last);

            // Disable feature
            dummyApiAccess.ReturnValue = new List<FeatureDto>();
            await router.RefreshEnabledFeaturesAsync();
            Assert.IsFalse(observer.Last);
        }
    }
}