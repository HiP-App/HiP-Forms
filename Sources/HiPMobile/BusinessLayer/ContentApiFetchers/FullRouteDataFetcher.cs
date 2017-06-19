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

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers
{
    class FullRouteDataFetcher : IFullRouteDataFetcher
    {
        private readonly IRoutesApiAccess routesApiAccess;
        private readonly IMediaDataFetcher mediaDataFetcher;
        private readonly IFullExhibitDataFetcher fullExhibitDataFetcher;

        public FullRouteDataFetcher (IRoutesApiAccess routesApiAccess, IMediaDataFetcher mediaDataFetcher, IFullExhibitDataFetcher fullExhibitDataFetcher)
        {
            this.routesApiAccess = routesApiAccess;
            this.mediaDataFetcher = mediaDataFetcher;
            this.fullExhibitDataFetcher = fullExhibitDataFetcher;
        }

        private RouteDto routeDto;
        private IList<int?> requiredMedia;
        private IList<Exhibit> remainingExhibits;

        public async Task LoadFullRouteDataIntoDatabase (string routeId, int idForRestApi, CancellationToken token, IProgressListener listener)
        {
            requiredMedia = new List<int?> ();
            remainingExhibits = ExhibitManager.GetExhibits ().ToList ().FindAll (x => !x.DetailsDataLoaded);

            double totalSteps = await FetchNeededMediaForFullRoute (idForRestApi);  // This is actually just the audio file
            if (token.IsCancellationRequested)
                return;

            listener.SetMaxProgress (totalSteps);   // This is certainly not all since there is also the data for the missing exhibits to be downloaded. How to estimate this? Pass from FullExhibits outside?

            await FetchMediaData (token, listener);
            if (token.IsCancellationRequested)
                return;

            using (var transaction = DbManager.StartTransaction ())
            {
                ProcessRoute (routeId, token, listener);    // Should this method be async to work with the token below?
                if (token.IsCancellationRequested)
                    transaction.Rollback ();
            }
        }
        
        private async Task<int> FetchNeededMediaForFullRoute (int idForRestApi)
        {
            routeDto = (await routesApiAccess.GetRoutes (new List<int> {idForRestApi})).Items.First();  // This could already be useful in the upper method

            if (routeDto.Audio.HasValue)
                requiredMedia.Add (routeDto.Audio);

            return requiredMedia.Count;
        }

        private void ProcessRoute(string routeId, CancellationToken token, IProgressListener listener)
        {
            var fetchedMedia = mediaDataFetcher.CombineMediasAndFiles();
            if (token.IsCancellationRequested)
                return;

            Route route = RouteManager.GetRoute (routeId);

            AddAudioToRoute (route, routeDto.Audio, fetchedMedia);
            listener.ProgressOneStep ();

            AddFullExhibitsToRoute (route, token, listener);

            
        }

        private async Task FetchMediaData(CancellationToken token, IProgressListener listener)
        {
            await mediaDataFetcher.FetchMedias(requiredMedia, token, listener);
        }

       private void AddAudioToRoute(Route dbRoute, int? mediaId, FetchedMediaData fetchedMediaData)
       {
            if (mediaId.HasValue)
            {
                var audio = fetchedMediaData.Audios.SingleOrDefault(x => x.IdForRestApi == mediaId);

                if (audio != null)
                    dbRoute.Audio = audio;
            }
            else
            {
                // No backup for audio? -> Discard this branch
            }
        }

        private async void AddFullExhibitsToRoute (Route route, CancellationToken token, IProgressListener listener)
        {
            foreach (var waypoint in route.Waypoints)
            {
                var dbExhibit = remainingExhibits.SingleOrDefault (x => x.IdForRestApi == waypoint.Exhibit.IdForRestApi);

                if (dbExhibit == null)
                    continue;

                await fullExhibitDataFetcher.FetchFullExhibitDataIntoDatabase (dbExhibit.Id, dbExhibit.IdForRestApi, token, listener);
                if (token.IsCancellationRequested)
                    return;
                listener.ProgressOneStep ();
            }
        }
    }
}
