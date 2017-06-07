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

using System.Threading;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers
{
    public class BaseDataFetcher : IBaseDataFetcher
    {
        private readonly IRoutesBaseDataFetcher routesBaseDataFetcher;
        private readonly IExhibitsBaseDataFetcher exhibitsBaseDataFetcher;
        private readonly IDataToRemoveFetcher dataToRemoveFetcher;

        public BaseDataFetcher(IRoutesBaseDataFetcher routesBaseDataFetcher, IExhibitsBaseDataFetcher exhibitsBaseDataFetcher, IDataToRemoveFetcher dataToRemoveFetcher)
        {
            this.routesBaseDataFetcher = routesBaseDataFetcher;
            this.exhibitsBaseDataFetcher = exhibitsBaseDataFetcher;
            this.dataToRemoveFetcher = dataToRemoveFetcher;
        }

        public async Task<bool> IsDatabaseUpToDate()
        {
            bool anyExhibitChanged = await exhibitsBaseDataFetcher.AnyExhibitChanged();
            bool anyRouteChanged = await routesBaseDataFetcher.AnyRouteChanged();

            return !(anyExhibitChanged || anyRouteChanged);
        }

        public async Task FetchBaseDataIntoDatabase(CancellationToken token, IProgressListener listener)
        {
            double totalSteps = await exhibitsBaseDataFetcher.FetchNeededDataForExhibits();
            totalSteps += await routesBaseDataFetcher.FetchNeededDataForRoutes();

            if (token.IsCancellationRequested)
            {
                return;
            }

            listener.SetMaxProgress(totalSteps);

            await exhibitsBaseDataFetcher.FetchMediaData(token, listener);
            if (token.IsCancellationRequested)
            {
                return;
            }
            await exhibitsBaseDataFetcher.FetchMediaData(token, listener);
            if (token.IsCancellationRequested)
            {
                return;
            }
            await dataToRemoveFetcher.FetchDataToDelete(token);
            if (token.IsCancellationRequested)
            {
                return;
            }

            using (var transaction = DbManager.StartTransaction())
            {
                exhibitsBaseDataFetcher.ProcessExhibits(listener);
                if (token.IsCancellationRequested)
                {
                    transaction.Rollback();
                    return;
                }
                routesBaseDataFetcher.ProcessRoutes(listener);
                if (token.IsCancellationRequested)
                {
                    transaction.Rollback();
                    return;
                }
                dataToRemoveFetcher.CleaupRemovedData();
                if (token.IsCancellationRequested)
                {
                    transaction.Rollback();
                    return;
                }
            }
        }
    }
}