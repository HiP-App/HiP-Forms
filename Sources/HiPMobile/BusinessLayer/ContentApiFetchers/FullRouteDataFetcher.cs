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

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers
{
    public class FullRouteDataFetcher : IFullRouteDataFetcher
    {
        private readonly IRoutesApiAccess routesApiAccess;
        private readonly IMediaDataFetcher mediaDataFetcher;
        private readonly IFullExhibitDataFetcher fullExhibitDataFetcher;

        public FullRouteDataFetcher(IRoutesApiAccess routesApiAccess, IMediaDataFetcher mediaDataFetcher, IFullExhibitDataFetcher fullExhibitDataFetcher)
        {
            this.routesApiAccess = routesApiAccess;
            this.mediaDataFetcher = mediaDataFetcher;
            this.fullExhibitDataFetcher = fullExhibitDataFetcher;
        }

        private RouteDto routeDto;
        private Route route;
        private IList<int?> requiredMedia;
        private List<Exhibit> missingExhibitsForRoute;
        private IList<ExhibitPagesAndMediaContainer> pagesAndMediaForMissingExhibits;

        public async Task FetchFullDownloadableDataIntoDatabase(
            string routeId, int idForRestApi, CancellationToken token, IProgressListener listener)
        {
            routeDto = (await routesApiAccess.GetRoutes(new List<int> { idForRestApi })).Items.First();
            if (token.IsCancellationRequested)
                return;

            route = DbManager.DataAccess.Routes().GetRoute(routeId);
            pagesAndMediaForMissingExhibits = new List<ExhibitPagesAndMediaContainer>();

            var allMissingExhibits = DbManager.DataAccess.Exhibits().GetExhibits().ToList().FindAll(x => !x.DetailsDataLoaded); // Exhibits not fully loaded yet
            missingExhibitsForRoute = allMissingExhibits.ToList().FindAll(x => routeDto.Exhibits.Contains(x.IdForRestApi)); // Select those part of the route

            double totalSteps = FetchNeededMediaForFullRoute();

            foreach (var exhibit in missingExhibitsForRoute) // Fetch media for missing exhibits and save them for later download
            {
                var pagesAndRequiredMediaForExhibit = await fullExhibitDataFetcher.FetchPagesAndMediaForExhibitFromRouteFetcher(exhibit.IdForRestApi);
                if (token.IsCancellationRequested)
                    return;

                pagesAndMediaForMissingExhibits.Add(pagesAndRequiredMediaForExhibit);
                totalSteps += pagesAndRequiredMediaForExhibit.RequiredMedia.Count;
            }

            listener.SetMaxProgress(totalSteps);

            await AddFullExhibitsToRoute(route, token, listener); // Download all missing exhibits
            if (token.IsCancellationRequested)
                return;

            await DbManager.InTransactionAsync(async transaction =>
            {
                var dataAccess = transaction.DataAccess;
                await ProcessRoute(token, listener, dataAccess); // Download audio
                if (token.IsCancellationRequested)
                    transaction.Rollback();
            });
        }

        private int FetchNeededMediaForFullRoute()
        {
            requiredMedia = new List<int?>();

            if (routeDto.Audio.HasValue && route.Audio == null)
                requiredMedia.Add(routeDto.Audio);

            return requiredMedia.Count;
        }

        private async Task ProcessRoute(CancellationToken token, IProgressListener listener, ITransactionDataAccess dataAccess)
        {
            await FetchMediaData(token, listener);
            var fetchedMedia = await mediaDataFetcher.CombineMediasAndFiles(dataAccess);
            if (token.IsCancellationRequested)
                return;

            AddAudioToRoute(route, routeDto.Audio, fetchedMedia);
        }

        private async Task FetchMediaData(CancellationToken token, IProgressListener listener)
        {
            await mediaDataFetcher.FetchMedias(requiredMedia, token, listener);
        }

        private void AddAudioToRoute(Route dbRoute, int? mediaId, FetchedMediaData fetchedMediaData)
        {
            if (!mediaId.HasValue)
                return;

            var audio = fetchedMediaData.Audios.SingleOrDefault(x => x.IdForRestApi == mediaId);
            if (audio == null)
                return;

            dbRoute.Audio = audio;
        }

        private async Task AddFullExhibitsToRoute(Route r, CancellationToken token, IProgressListener listener)
        {
            foreach (var waypoint in r.Waypoints)
            {
                var dbExhibit = missingExhibitsForRoute.SingleOrDefault(x => x.IdForRestApi == waypoint.Exhibit.IdForRestApi);

                if (dbExhibit == null)
                    continue;

                var pagesAndMediaContainerForExhibit = pagesAndMediaForMissingExhibits.SingleOrDefault(x => x.ExhibitIdForRestApi == dbExhibit.IdForRestApi);
                await fullExhibitDataFetcher.FetchFullExhibitDataIntoDatabaseWithFetchedPagesAndMedia(dbExhibit.Id, pagesAndMediaContainerForExhibit, token, listener);
                if (token.IsCancellationRequested)
                    return;
                pagesAndMediaForMissingExhibits.Remove(pagesAndMediaContainerForExhibit);
            }
        }
    }
}