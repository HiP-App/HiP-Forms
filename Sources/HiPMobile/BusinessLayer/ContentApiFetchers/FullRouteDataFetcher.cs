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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Common;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers
{
    public class FullRouteDataFetcher : IFullRouteDataFetcher
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
        private List<Exhibit> missingExhibitsForRoute;

        public async Task FetchFullDownloadableDataIntoDatabase (string routeId, int idForRestApi, CancellationToken token, IProgressListener listener, bool calledFromRouteFetcher)
        {
            routeDto = (await routesApiAccess.GetRoutes(new List<int> { idForRestApi })).Items.First(); 
            
            IList<Exhibit> allMissingExhibits = ExhibitManager.GetExhibits ().ToList ().FindAll (x => !x.DetailsDataLoaded);    // Exhibits not fully loaded yet
            missingExhibitsForRoute = allMissingExhibits.ToList ().FindAll (x => routeDto.Exhibits.Contains (x.IdForRestApi));  // Select those part of the route
            
            double totalSteps = FetchNeededMediaForFullRoute ();
            if (token.IsCancellationRequested)
                return;

            foreach (var exhibit in missingExhibitsForRoute)
            {
                totalSteps += await fullExhibitDataFetcher.FetchNeededMediaForFullExhibit (exhibit.IdForRestApi);
            }

            listener.SetMaxProgress (totalSteps);
            
            using (var transaction = DbManager.StartTransaction ())
            {
                await ProcessRoute (routeId, token, listener);
                if (token.IsCancellationRequested)
                    transaction.Rollback ();
            }
        }
        
        private int FetchNeededMediaForFullRoute ()
        {
            requiredMedia = new List<int?>();

            if (routeDto.Audio.HasValue)
                requiredMedia.Add (routeDto.Audio);

            return requiredMedia.Count;
        }

        private async Task ProcessRoute(string routeId, CancellationToken token, IProgressListener listener)
        {
            await FetchMediaData (token, listener);
            var fetchedMedia = mediaDataFetcher.CombineMediasAndFiles();
            if (token.IsCancellationRequested)
                return;

            var route = RouteManager.GetRoute (routeId);

            AddAudioToRoute (route, routeDto.Audio, fetchedMedia, listener);

            AddFullExhibitsToRoute (route, token, listener);
        }

        private async Task FetchMediaData(CancellationToken token, IProgressListener listener)
        {
            await mediaDataFetcher.FetchMedias(requiredMedia, token, listener);
        }

       private void AddAudioToRoute(Route dbRoute, int? mediaId, FetchedMediaData fetchedMediaData, IProgressListener listener)
       {
           if (!mediaId.HasValue)
               return;

           var audio = fetchedMediaData.Audios.SingleOrDefault(x => x.IdForRestApi == mediaId);
           if (audio == null)
               return;

           dbRoute.Audio = audio;
       }

        private async void AddFullExhibitsToRoute (Route route, CancellationToken token, IProgressListener listener)
        {
            foreach (var waypoint in route.Waypoints)
            {
                var dbExhibit = missingExhibitsForRoute.SingleOrDefault (x => x.IdForRestApi == waypoint.Exhibit.IdForRestApi);

                if (dbExhibit == null)
                    continue;

                await fullExhibitDataFetcher.FetchFullDownloadableDataIntoDatabase (dbExhibit.Id, dbExhibit.IdForRestApi, token, listener, true);
                if (token.IsCancellationRequested)
                    return;
            }
        }
    }
}
