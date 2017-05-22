﻿// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
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
using Microsoft.Practices.Unity;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.Helpers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiAccesses.Contracts;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.ServiceAccessLayer.ContentApiDtos;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.ContentApiFetchers {
    public class RoutesBaseDataFetcher : IRoutesBaseDataFetcher{

        [Dependency]
        public RouteConverter RouteConverter { private get; set; }

        private readonly IRoutesApiAccess routesApiAccess;

        private readonly IMediaDataFetcher mediaDataFetcher;

        private IList<int> requiredRouteImages;
        private IDictionary<RouteDto, Route> updatedRoutes;
        private IList<RouteDto> newRoutes;
        private IList<RouteDto> fetchedChangedRoutes;

        public RoutesBaseDataFetcher (IMediaDataFetcher mediaDataFetcher, IRoutesApiAccess routesApiAccess)
        {
            this.mediaDataFetcher = mediaDataFetcher;
            this.routesApiAccess = routesApiAccess;
        }

        public async Task<int> FetchNeededDataForRoutes()
        {
            requiredRouteImages = new List<int>();
            updatedRoutes = new Dictionary<RouteDto, Route>();
            newRoutes = new List<RouteDto>();

            var dbRoutes = RouteManager.GetRoutes().ToList();
            if (fetchedChangedRoutes == null)
            {
                await AnyRouteChanged();
            }
            foreach (var routeDto in fetchedChangedRoutes)
            {

                var dbRoute = dbRoutes.SingleOrDefault(x => x.IdForRestApi == routeDto.Id);

                if (dbRoute != null && dbRoute.UnixTimestamp != routeDto.Timestamp)
                {
                    updatedRoutes.Add(routeDto, dbRoute);
                    requiredRouteImages.Add(routeDto.Image);
                }
                else
                {
                    newRoutes.Add(routeDto);
                    requiredRouteImages.Add(routeDto.Image);
                }
            }

            return requiredRouteImages.Count + fetchedChangedRoutes.Count;
        }

        public async Task ProcessRoutes(CancellationToken token, IProgressListener listener)
        {
            var fetchedMedia = await mediaDataFetcher.FetchMedias(requiredRouteImages, token, listener);
            if (token.IsCancellationRequested)
            {
                return;
            }
            ProcessUpdatedRoutes(fetchedMedia, listener);
            ProcessNewRoutes(fetchedMedia, listener);
        }

        private void ProcessUpdatedRoutes(FetchedMediaData fetchedMediaData, IProgressListener listener)
        {
            foreach (var routePair in updatedRoutes)
            {
                var routeDto = routePair.Key;
                var dbRoute = routePair.Value;

                RouteConverter.Convert(routeDto, dbRoute);

                AddImageToRoute(dbRoute, routeDto.Image, fetchedMediaData);
                //TODO: If route content was already downloaded 
                //-> Show dialog whether to download new data or do it directly depending on setting

                listener.ProgressOneStep();
            }
        }

        private void ProcessNewRoutes(FetchedMediaData fetchedMediaData, IProgressListener listener)
        {
            foreach (var routeDto in newRoutes)
            {
                var dbRoute = RouteConverter.Convert(routeDto);

                AddImageToRoute(dbRoute, routeDto.Image, fetchedMediaData);

                listener.ProgressOneStep();
            }
        }

        private void AddImageToRoute(Route dbRoute, int mediaId, FetchedMediaData fetchedMediaData)
        {
            var image = fetchedMediaData.Images.SingleOrDefault(x => x.IdForRestApi == mediaId);

            if (image != null)
            {
                dbRoute.Image = image;
            }
        }

        public async Task<bool> AnyRouteChanged()
        {
            var dbRoutes = RouteManager.GetRoutes().ToList();
            RoutesDto changedRoutes;
            if (dbRoutes.Any())
            {
                var latestTimestamp = dbRoutes.Max(x => x.UnixTimestamp);
                changedRoutes = await routesApiAccess.GetRoutes(latestTimestamp);
            }
            else
            {
                changedRoutes = await routesApiAccess.GetRoutes();
            }

            fetchedChangedRoutes = changedRoutes.Items;
            return fetchedChangedRoutes.Any();
        }

    }
}