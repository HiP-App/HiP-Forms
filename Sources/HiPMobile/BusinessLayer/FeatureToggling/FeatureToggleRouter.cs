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
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.FeatureToggleApiAccess;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.FeatureToggling
{
    public class FeatureToggleRouter : IFeatureToggleRouter
    {
        /// <summary>
        /// Download enabled features for the current user and
        /// create a feature toggle router from it. If the download fails,
        /// default values defined in <see cref="FeatureConfiguration"/> are used.
        /// 
        /// Waits 5 seconds at maximum for the download to complete.
        /// </summary>
        /// <returns></returns>
        public static async Task<FeatureToggleRouter> Create()
        {
            var enabledFeatureIds = await FetchEnabledFeatureIds();
            return new FeatureToggleRouter(enabledFeatureIds);
        }

        private static async Task<IList<int>> FetchEnabledFeatureIds(IList<int> fallbackEnabledFeatureIds = null)
        {
            IList<int> enabledFeatureIds;
            try
            {
                var featureDtosTask = IoCManager.Resolve<IFeatureToggleApiAccess>().GetEnabledFeaturesAsync();
                if (await Task.WhenAny(featureDtosTask, Task.Delay(5000)) == featureDtosTask)
                {
                    enabledFeatureIds = featureDtosTask.Result.Select(it => it.Id).ToList();
                }
                else
                {
                    throw new TimeoutException();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                enabledFeatureIds = fallbackEnabledFeatureIds ??
                                    FeatureConfiguration.DefaultEnabledFeatureIds.Select(it => (int) it).ToList();
            }

            return enabledFeatureIds;
        }

        private IList<int> enabledFeatureIds;
        private readonly Dictionary<int, Observable<bool>> enabledFeatureObservables = new Dictionary<int, Observable<bool>>();

        private FeatureToggleRouter(IList<int> enabledFeatureIds)
        {
            this.enabledFeatureIds = enabledFeatureIds;
        }

        public async Task RefreshEnabledFeaturesAsync()
        {
            var newEnabledFeatureIds = await FetchEnabledFeatureIds(enabledFeatureIds);

            lock (this)
            {
                var oldEnabledFeatureIds = enabledFeatureIds;

                foreach (var disabled in oldEnabledFeatureIds.Except(newEnabledFeatureIds))
                {
                    if (enabledFeatureObservables.TryGetValue(disabled, out var observable))
                    {
                        observable.Current = false;
                    }
                    else
                    {
                        enabledFeatureObservables[disabled] = new Observable<bool>(false);
                    }
                }

                foreach (var enabled in newEnabledFeatureIds.Except(oldEnabledFeatureIds))
                {
                    if (enabledFeatureObservables.TryGetValue(enabled, out var observable))
                    {
                        observable.Current = true;
                    }
                    else
                    {
                        enabledFeatureObservables[enabled] = new Observable<bool>(true);
                    }
                }

                enabledFeatureIds = newEnabledFeatureIds;
            }
        }

        public IObservable<bool> IsFeatureEnabled(FeatureId featureId)
        {
            lock (this)
            {
                if (enabledFeatureObservables.TryGetValue((int) featureId, out var observable))
                {
                    return observable;
                }

                var defaultFeatureObservable = new Observable<bool>(enabledFeatureIds.Contains((int) featureId));
                enabledFeatureObservables[(int) featureId] = defaultFeatureObservable;
                return defaultFeatureObservable;
            }
        }
    }
}