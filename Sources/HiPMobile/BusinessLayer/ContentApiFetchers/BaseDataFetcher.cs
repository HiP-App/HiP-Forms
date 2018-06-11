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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            bool anyExhibitChanged = await exhibitsBaseDataFetcher.AnyExhibitChanged(DbManager.DataAccess);
            bool anyRouteChanged = await routesBaseDataFetcher.AnyRouteChanged(DbManager.DataAccess);

            return !(anyExhibitChanged || anyRouteChanged);
        }

        public async Task FetchBaseDataIntoDatabase(CancellationToken token, IProgressListener listener)
        {
            var routes = DbManager.DataAccess.Routes().GetRoutes().ToDictionary(x => x.IdForRestApi, x => x.Timestamp);
            var exhibits = DbManager.DataAccess.Exhibits().GetExhibits().ToDictionary(x => x.IdForRestApi, x => x.Timestamp);

            double totalSteps = await exhibitsBaseDataFetcher.FetchNeededDataForExhibits(exhibits);
            totalSteps += await routesBaseDataFetcher.FetchNeededDataForRoutes(routes);

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

            await routesBaseDataFetcher.FetchMediaData(token, listener);
            if (token.IsCancellationRequested)
            {
                return;
            }

            var exhibitsMediaToFilePath = await exhibitsBaseDataFetcher.WriteMediaToDiskAsync();
            var routesMediaToFilePath = await routesBaseDataFetcher.WriteMediaToDiskAsync();

            var cancelled = DbManager.InTransaction(transaction =>
            {
                exhibitsBaseDataFetcher.ProcessExhibits(listener, transaction.DataAccess, exhibitsMediaToFilePath);
                if (token.IsCancellationRequested)
                {
                    transaction.Rollback();
                    return true;
                }

                routesBaseDataFetcher.ProcessRoutes(listener, transaction.DataAccess, routesMediaToFilePath);
                if (token.IsCancellationRequested)
                {
                    transaction.Rollback();
                    return true;
                }

                return false;
            });

            if (!cancelled)
            {
                await dataToRemoveFetcher.FetchDataToDelete(token);
                DbManager.InTransaction(transaction => { dataToRemoveFetcher.CleanupRemovedData(transaction.DataAccess); });
                await dataToRemoveFetcher.PruneMediaFilesAsync(DbManager.DataAccess);
            }
        }
    }
}